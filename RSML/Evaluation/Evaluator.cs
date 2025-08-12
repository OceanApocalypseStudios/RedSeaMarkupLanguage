/*
 *       :::::::::   ::::::::     :::   :::   :::
 *      :+:    :+: :+:    :+:   :+:+: :+:+:  :+:
 *     +:+    +:+ +:+         +:+ +:+:+ +:+ +:+
 *    +#++:++#:  +#++:++#++  +#+  +:+  +#+ +#+
 *   +#+    +#+        +#+  +#+       +#+ +#+
 *  #+#    #+# #+#    #+#  #+#       #+# #+#
 * ###    ###  ########   ###       ### ##########
 *
 * OceanApocalypseStudios * C# * Lead Development by Matthew
 *												(MF366)
 *
 * MIT License
 *
 * Copyright (c) 2025 OceanApocalypseStudios
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using RSML.Actions;
using RSML.Analyzer.Semantics;
using RSML.Analyzer.Syntax;
using RSML.Exceptions;
using RSML.Machine;
using RSML.Middlewares;
using RSML.Reader;
using RSML.Toolchain.Compliance;


namespace RSML.Evaluation
{

	/// <summary>
	/// The officially maintained RSML evaluator that evaluates a document and returns a match's value, in case one was found.
	/// </summary>
	public sealed class Evaluator : IEvaluator
	{

		private const string ApiVersion = "2.0.0";

		/// <summary>
		/// The loaded middlewares.
		/// </summary>
		private readonly Dictionary<MiddlewareRunnerLocation, ICollection<Middleware>> evaluatorMiddlewares = [ ];

		private readonly Dictionary<string, SpecialAction> specialActions = [ ];

		/// <summary>
		/// Creates a new instance of a RSML evaluator.
		/// </summary>
		/// <param name="content">The document</param>
		public Evaluator(ReadOnlySpan<char> content) { Content = $"{content.ToString().ReplaceLineEndings()}{Environment.NewLine}"; }

		/// <summary>
		/// Creates a new instance of a RSML evaluator.
		/// </summary>
		/// <param name="content">The document</param>
		public Evaluator(string content) { Content = $"{content.ReplaceLineEndings()}{Environment.NewLine}"; }

		/// <inheritdoc />
		public SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull(ApiVersion);

		/// <inheritdoc />
		public string Content { get; set; }

		/// <inheritdoc />
		public ReadOnlyDictionary<string, SpecialAction> SpecialActions => specialActions.AsReadOnly();

		/// <inheritdoc />
		public EvaluationResult Evaluate() => Evaluate(new());

		/// <inheritdoc />
		public EvaluationResult Evaluate(LocalMachine machineData) => Evaluate(machineData, null, null, null, null);

		/// <inheritdoc />
		public EvaluationResult Evaluate(LocalMachine machineData, IReader? reader, ILexer? lexer, INormalizer? normalizer, IValidator? validator)
		{

			if (Content.Length == 0)
				return new();

			reader ??= new RsmlReader(Content);
			lexer ??= new Lexer();
			validator ??= new Validator();
			normalizer ??= new Normalizer();

			if (evaluatorMiddlewares.Count == 0)
				return Evaluate_NoMiddleware(machineData, lexer, reader, normalizer, validator);

			while (reader.TryTokenizeNextLine(lexer, out var rawTokens))
			{

				var syntaxTokens = rawTokens as SyntaxToken[] ?? rawTokens.ToArray();

				if (RunMiddlewares(MiddlewareRunnerLocation.BeforeNormalization, syntaxTokens) == MiddlewareResult.EndEvaluation)
					return new();

				var normalizedTokens = normalizer.NormalizeLine(syntaxTokens, out _);
				var tokens = normalizedTokens as SyntaxToken[] ?? normalizedTokens.ToArray();

				if (RunMiddlewares(MiddlewareRunnerLocation.BeforeValidation, tokens) == MiddlewareResult.EndEvaluation)
					return new();

				validator.ValidateLine(tokens.AsSpan()); // line validation

				if (RunMiddlewares(MiddlewareRunnerLocation.BeforeEolRemoval, tokens) == MiddlewareResult.EndEvaluation)
					return new();

				if (tokens[^1].Kind == TokenKind.Eol)
					tokens = tokens[..^1];

				// we basically do length-based checks
				/*
				 * Possible Lengths of tokens:
				 *	2 - Comment (#, Text)
				 *	3 - Special Action (@, Name, Arg)
				 *	5 - Logic Path (Op, Sys, Version Major = ANY, Arch, RetVal)
				 *	6 - Logic Path (Op, Sys, Version Major, Arch, RetVal)
				 *
				 */

				if (RunMiddlewares(MiddlewareRunnerLocation.BeforeLengthCheck, tokens) == MiddlewareResult.EndEvaluation)
					return new();

				switch (tokens.Length)
				{

					case 0:
						// literally nothing
						if (RunMiddlewares(MiddlewareRunnerLocation.ZeroLength, tokens) == MiddlewareResult.EndEvaluation)
							return new();

						continue;

					case 2:
						// comment, ignore it
						if (RunMiddlewares(MiddlewareRunnerLocation.TwoLength, tokens) == MiddlewareResult.EndEvaluation)
							return new();

						continue;

					case 3:
						if (RunMiddlewares(MiddlewareRunnerLocation.ThreeLengthBeforeSpecialCall, tokens) == MiddlewareResult.EndEvaluation)
							return new();

						byte result = HandleSpecialActionCall(tokens[1].Value, tokens[2].Value);

						switch (result)
						{

							case SpecialActionBehavior.NoBehavior:
								continue;

							case SpecialActionBehavior.Error:
								throw new ActionStandardErrorException("A special action returned error code 1.");

							case SpecialActionBehavior.StopEvaluation:
								return new();

							case SpecialActionBehavior.ResetSpecials:
								specialActions.Clear();

								continue;

							default:
								throw new ActionErrorException(
									$"A special action returned error code {result}. In the future, please use 1 to signal errors."
								);

						}

					case 5:
						if (RunMiddlewares(MiddlewareRunnerLocation.FiveLengthBeforeHandling, tokens) == MiddlewareResult.EndEvaluation)
							return new();

						if (HandleLogicPath_Simple(tokens, machineData, machineData.SystemName == "linux"))
						{

							if (RunMiddlewares(MiddlewareRunnerLocation.FiveLengthAfterHandling, tokens) == MiddlewareResult.EndEvaluation)
								return new();

							return tokens[0].Kind == TokenKind.ThrowErrorOperator
									   ? throw new UserRaisedException("Error-throw operator used.")
									   : new(tokens[4].Value);

						}

						continue;

					case 6:
						if (RunMiddlewares(MiddlewareRunnerLocation.SixLengthBeforeHandling, tokens) == MiddlewareResult.EndEvaluation)
							return new();

						if (HandleLogicPath_Complex(tokens, machineData, machineData.SystemName == "linux"))
						{

							if (RunMiddlewares(MiddlewareRunnerLocation.SixLengthAfterHandling, tokens) == MiddlewareResult.EndEvaluation)
								return new();

							return tokens[0].Kind == TokenKind.ThrowErrorOperator
									   ? throw new UserRaisedException("Error-throw operator used.")
									   : new(tokens[5].Value);

						}

						continue;

					default:
						if (RunMiddlewares(MiddlewareRunnerLocation.AnyLengthBeforeCommentCheck, tokens) == MiddlewareResult.EndEvaluation)
							return new();

						if (tokens[0].Kind == TokenKind.CommentSymbol)
							continue; // it's somehow a comment

						throw new InvalidRsmlSyntax("Unexpected error: invalid line tokenized successfully.");

				}

			}

			return new(); // no matches

		}

		/// <inheritdoc />
		public bool IsComment(ReadOnlySpan<char> line) =>
			line.TrimStart()[0] == '#' && !(line.IsEmpty || line.IsWhiteSpace() || line.IsNewLinesOnly());

		/// <inheritdoc />
		public bool IsComment(string line) => IsComment(line.AsSpan());

		/// <inheritdoc />
		public IEvaluator RegisterSpecialAction(string name, SpecialAction action)
		{

			if (name.StartsWith('@') || name.StartsWith('#') || name == "EndAll")
				throw new ArgumentException("The name of the special action must not be EndAll or start with @ or #.", nameof(name));

			specialActions[name] = action;

			return this;

		}

		/// <summary>
		/// Binds a middleware to a runner location.
		/// </summary>
		/// <param name="runnerLocation">The runner location</param>
		/// <param name="middleware">The middleware</param>
		/// <returns>The evaluator (fluent API)</returns>
		public Evaluator BindMiddleware(MiddlewareRunnerLocation runnerLocation, Middleware middleware)
		{

			if (!evaluatorMiddlewares.TryGetValue(runnerLocation, out var value))
			{

				value = [ middleware ];
				evaluatorMiddlewares.Add(runnerLocation, value);

				return this;

			}

			value.Add(middleware);

			return this;

		}

		/// <summary>
		/// Unbinds a middleware from a runner location.
		/// </summary>
		/// <param name="runnerLocation">The runner location</param>
		/// <param name="middleware">The middleware</param>
		/// <returns>The evaluator (fluent API)</returns>
		public Evaluator UnbindMiddleware(MiddlewareRunnerLocation runnerLocation, Middleware middleware)
		{

			if (evaluatorMiddlewares.TryGetValue(runnerLocation, out var value))
				_ = value.Remove(middleware);

			return this;

		}

		/// <summary>
		/// Evaluates RSML with no middleware interference.
		/// Middlewares are slow so this is the best solution.
		/// This method is automatically ran instead of the default one if there are no middlewares.
		/// </summary>
		/// <param name="machineData"></param>
		/// <param name="lexer"></param>
		/// <param name="reader"></param>
		/// <param name="normalizer"></param>
		/// <param name="validator"></param>
		/// <returns>An evaluation result</returns>
		/// <exception cref="ActionStandardErrorException"></exception>
		/// <exception cref="ActionErrorException"></exception>
		/// <exception cref="UserRaisedException"></exception>
		/// <exception cref="InvalidRsmlSyntax"></exception>
		private EvaluationResult Evaluate_NoMiddleware(
			LocalMachine machineData,
			ILexer lexer,
			IReader reader,
			INormalizer normalizer,
			IValidator validator
		)
		{

			while (reader.TryTokenizeNextLine(lexer, out var rawTokens))
			{

				var normalizedTokens = normalizer.NormalizeLine(rawTokens, out _);
				var tokens = (normalizedTokens as SyntaxToken[] ?? normalizedTokens.ToArray()).AsSpan();

				validator.ValidateLine(tokens); // line validation

				if (tokens[^1].Kind == TokenKind.Eol)
					tokens = tokens[..^1];

				// we basically do length-based checks
				/*
				 * Possible Lengths of tokens:
				 *	2 - Comment (#, Text)
				 *	3 - Special Action (@, Name, Arg)
				 *	5 - Logic Path (Op, Sys, Version Major = ANY, Arch, RetVal)
				 *	6 - Logic Path (Op, Sys, Version Major, Arch, RetVal)
				 *
				 */

				switch (tokens.Length)
				{

					case 0:
					case 2:
						continue;

					case 3:
						byte result = HandleSpecialActionCall(tokens[1].Value, tokens[2].Value);

						switch (result)
						{

							case SpecialActionBehavior.NoBehavior:
								continue;

							case SpecialActionBehavior.Error:
								throw new ActionStandardErrorException("A special action returned error code 1.");

							case SpecialActionBehavior.StopEvaluation:
								return new();

							case SpecialActionBehavior.ResetSpecials:
								specialActions.Clear();

								continue;

							default:
								throw new ActionErrorException(
									$"A special action returned error code {result}. In the future, please use 1 to signal errors."
								);

						}

					case 5:
						if (HandleLogicPath_Simple(tokens, machineData, machineData.SystemName == "linux"))
						{

							return tokens[0].Kind == TokenKind.ThrowErrorOperator
									   ? throw new UserRaisedException("Error-throw operator used.")
									   : new(tokens[4].Value);

						}

						continue;

					case 6:
						if (HandleLogicPath_Complex(tokens, machineData, machineData.SystemName == "linux"))
						{

							return tokens[0].Kind == TokenKind.ThrowErrorOperator
									   ? throw new UserRaisedException("Error-throw operator used.")
									   : new(tokens[5].Value);

						}

						continue;

					default:
						if (tokens[0].Kind == TokenKind.CommentSymbol)
							continue; // it's somehow a comment

						throw new InvalidRsmlSyntax("Unexpected error: invalid line tokenized successfully.");

				}

			}

			return new(); // no matches

		}

		private MiddlewareResult RunMiddlewares(MiddlewareRunnerLocation location, IEnumerable<SyntaxToken> tokens) =>
			evaluatorMiddlewares.TryGetValue(location, out var value)
				? value
				  .Select(middleware => middleware.Invoke(tokens))
				  .FirstOrDefault(result => result != MiddlewareResult.ContinueEvaluation
				  )
				: MiddlewareResult.ContinueEvaluation;

		private byte HandleSpecialActionCall(string name, string? arg)
		{

			if (name == "EndAll")
				return SpecialActionBehavior.StopEvaluation;

			if (!SpecialActions.TryGetValue(name, out var action))
				throw new UndefinedActionException("Action is undefined but used");

			return action.Invoke(this, arg ?? "");

		}

		private static bool HandleLogicPath_Simple(ReadOnlySpan<SyntaxToken> tokens, in LocalMachine machine, bool isLinux)
		{

			if (isLinux)
				return HandleLogicPath_Simple_Linux(tokens, machine);

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.SystemName is not null,
				_                         => machine.SystemName == tokens[1].Value

			};

			bool systemVersionMatches = tokens[2].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.SystemVersion != -1,
				_                         => machine.SystemVersion.ToString() == tokens[2].Value

			};

			bool architectureMatches = tokens[3].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                         => machine.ProcessorArchitecture == tokens[3].Value

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Complex(ReadOnlySpan<SyntaxToken> tokens, in LocalMachine machine, bool isLinux)
		{

			if (isLinux)
				return HandleLogicPath_Complex_Linux(tokens, machine);

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.SystemName is not null,
				_                         => machine.SystemName == tokens[1].Value

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[2].Kind)
			{

				case TokenKind.Equals:
					systemVersionMatches = machine.SystemVersion.ToString() == tokens[3].Value;

					break;

				case TokenKind.Different:
					systemVersionMatches = machine.SystemVersion.ToString() != tokens[3].Value;

					break;

				case TokenKind.GreaterOrEqualsThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion >= versionNum;

					break;

				case TokenKind.LessOrEqualsThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion <= versionNum;

					break;

				case TokenKind.GreaterThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion > versionNum;

					break;

				case TokenKind.LessThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion < versionNum;

					break;

				default:
					systemVersionMatches = false;

					break;

			}

			bool architectureMatches = tokens[4].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                         => machine.ProcessorArchitecture == tokens[3].Value

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Simple_Linux(ReadOnlySpan<SyntaxToken> tokens, in LocalMachine machine)
		{

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.DistroName is not null,
				_ => machine.SystemName == tokens[1].Value ||
					 machine.DistroName == tokens[1].Value ||
					 machine.DistroFamily == tokens[1].Value

			};

			bool systemVersionMatches = tokens[2].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.SystemVersion != -1,
				_                         => machine.SystemVersion.ToString() == tokens[2].Value

			};

			bool architectureMatches = tokens[3].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                         => machine.ProcessorArchitecture == tokens[3].Value

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Complex_Linux(ReadOnlySpan<SyntaxToken> tokens, in LocalMachine machine)
		{

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.DistroName is not null,
				_ => machine.SystemName == tokens[1].Value ||
					 machine.DistroName == tokens[1].Value ||
					 machine.DistroFamily == tokens[1].Value

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[2].Kind)
			{

				case TokenKind.Equals:
					systemVersionMatches = machine.SystemVersion.ToString() == tokens[3].Value;

					break;

				case TokenKind.Different:
					systemVersionMatches = machine.SystemVersion.ToString() != tokens[3].Value;

					break;

				case TokenKind.GreaterOrEqualsThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion >= versionNum;

					break;

				case TokenKind.LessOrEqualsThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion <= versionNum;

					break;

				case TokenKind.GreaterThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion > versionNum;

					break;

				case TokenKind.LessThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion < versionNum;

					break;

				default:
					systemVersionMatches = false;

					break;

			}


			bool architectureMatches = tokens[4].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                         => machine.ProcessorArchitecture == tokens[3].Value

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

	}

}
