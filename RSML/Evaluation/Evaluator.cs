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
		private readonly List<Middleware> evaluatorMiddlewares = [ ];

		private readonly Dictionary<string, SpecialAction> specialActions = [ ];

		/// <summary>
		/// Creates a new instance of a RSML evaluator.
		/// </summary>
		/// <param name="content">The document</param>
		public Evaluator(ReadOnlySpan<char> content) { Content = content.ToString(); }

		/// <summary>
		/// Creates a new instance of a RSML evaluator.
		/// </summary>
		/// <param name="content">The document</param>
		public Evaluator(string content) { Content = content; }

		/// <summary>
		/// The amount of loaded middlewares.
		/// </summary>
		public int LoadedMiddlewaresCount => evaluatorMiddlewares.Count;

		/// <inheritdoc />
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull(ApiVersion);

		/// <inheritdoc />
		public string Content { get; set; }

		/// <inheritdoc />
		public ReadOnlyDictionary<string, SpecialAction> SpecialActions => specialActions.AsReadOnly();

		/// <inheritdoc />
		public EvaluationResult Evaluate() => Evaluate(new());

		/// <inheritdoc />
		public EvaluationResult Evaluate(LocalMachine machineData) => Evaluate(machineData, null);

		/// <inheritdoc />
		public EvaluationResult Evaluate(
			LocalMachine machineData,
			IReader? reader
		)
		{

			if (Content.Length == 0)
				return new();

			reader ??= new RsmlReader(Content);

			if (evaluatorMiddlewares.Count == 0)
				return Evaluate_NoMiddleware(machineData, reader);

			while (reader.TryTokenizeNextLine(out var rawTokens))
			{

				var syntaxTokens = rawTokens as SyntaxToken[] ?? rawTokens.ToArray();

				var normalizedTokens = Normalizer.NormalizeLine(syntaxTokens, out _);
				var tokens = normalizedTokens as SyntaxToken[] ?? normalizedTokens.ToArray();

				Validator.ValidateLine(tokens.AsSpan()); // line validation

				if (tokens[^1].Kind == TokenKind.Eol)
					tokens = tokens.AsSpan()[..^1].ToArray();

				MiddlewareContext ctx = new(reader.CurrentBufferIndex, tokens, syntaxTokens, null);

				// ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
				foreach (var middleware in evaluatorMiddlewares)
				{

					if (!middleware(ctx))
						return new(); // stop loop

				}

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
						// literally nothing
						continue;

					case 2:
						// comment, ignore it
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

		/// <inheritdoc />
		public static bool IsComment(ReadOnlySpan<char> line) =>
			line.TrimStart()[0] == '#' && !(line.IsEmpty || line.IsWhiteSpace() || line.IsNewLinesOnly());

		/// <inheritdoc />
		public static bool IsComment(string line) => IsComment(line.AsSpan());

		/// <inheritdoc />
		public IEvaluator RegisterSpecialAction(string name, SpecialAction action)
		{

			if (name.StartsWith('@') || name.StartsWith('#') || name == "EndAll")
				throw new ArgumentException("The name of the special action must not be EndAll or start with @ or #.", nameof(name));

			specialActions[name] = action;

			return this;

		}

		/// <summary>
		/// Binds a middleware to the evaluator.
		/// </summary>
		/// <param name="middleware">The middleware</param>
		/// <returns>The evaluator (fluent API)</returns>
		public Evaluator BindMiddleware(Middleware middleware)
		{

			evaluatorMiddlewares.Add(middleware);

			return this;

		}

		/// <summary>
		/// Unbinds a middleware from the evaluator.
		/// </summary>
		/// <param name="middleware">The middleware</param>
		/// <param name="removed"><c>true</c> if successful, <c>false</c> if failed or middleware was not bound</param>
		/// <returns>The evaluator (fluent API)</returns>
		public Evaluator UnbindMiddleware(Middleware middleware, out bool removed)
		{

			removed = evaluatorMiddlewares.Remove(middleware);

			return this;

		}

		/// <summary>
		/// Evaluates RSML with no middleware interference.
		/// Middlewares are slow so this is the best solution.
		/// This method is automatically ran instead of the default one if there are no middlewares.
		/// </summary>
		private EvaluationResult Evaluate_NoMiddleware(
			LocalMachine machineData,
			IReader reader
		)
		{

			while (reader.TryTokenizeNextLine(out var rawTokens))
			{

				var normalizedTokens = Normalizer.NormalizeLine(rawTokens, out _);
				var tokens = (normalizedTokens as SyntaxToken[] ?? normalizedTokens.ToArray()).AsSpan();

				Validator.ValidateLine(tokens); // line validation

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
						if (tokens[1].Value == "EndAll")
							continue;

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

		private byte HandleSpecialActionCall(string name, string arg)
		{

			if (name == "EndAll")
				return SpecialActionBehavior.StopEvaluation;

			return specialActions.TryGetValue(name, out var action)
					   ? action(this, arg)
					   : throw new UndefinedActionException("Action is undefined but used");

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
				_                         => machine.StringifiedSystemVersion == tokens[2].Value

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
					systemVersionMatches = machine.StringifiedSystemVersion == tokens[3].Value;

					break;

				case TokenKind.Different:
					systemVersionMatches = machine.StringifiedSystemVersion != tokens[3].Value;

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
				_                         => machine.StringifiedSystemVersion == tokens[2].Value

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
					systemVersionMatches = machine.StringifiedSystemVersion == tokens[3].Value;

					break;

				case TokenKind.Different:
					systemVersionMatches = machine.StringifiedSystemVersion != tokens[3].Value;

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
