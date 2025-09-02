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

using OceanApocalypseStudios.RSML.Analyzer.Semantics;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Middlewares;
using OceanApocalypseStudios.RSML.Reader;
using OceanApocalypseStudios.RSML.Toolchain.Compliance;

using LocalMachine = OceanApocalypseStudios.RSML.Machine.LocalMachine;


namespace OceanApocalypseStudios.RSML.Evaluation
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
						switch (tokens[1].Value)
						{

							case "Void":
								continue;

							case "ThrowError":
								throw new ActionErrorException("A special action returned an error code.");

							case "EndAll":
								return new();

							default:
								throw new ActionErrorException("Unrecognized special action.");

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

				case TokenKind.EqualTo:
					systemVersionMatches = machine.StringifiedSystemVersion == tokens[3].Value;

					break;

				case TokenKind.NotEqualTo:
					systemVersionMatches = machine.StringifiedSystemVersion != tokens[3].Value;

					break;

				case TokenKind.GreaterThanOrEqualTo:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion >= versionNum;

					break;

				case TokenKind.LessThanOrEqualTo:
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

				case TokenKind.EqualTo:
					systemVersionMatches = machine.StringifiedSystemVersion == tokens[3].Value;

					break;

				case TokenKind.NotEqualTo:
					systemVersionMatches = machine.StringifiedSystemVersion != tokens[3].Value;

					break;

				case TokenKind.GreaterThanOrEqualTo:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion >= versionNum;

					break;

				case TokenKind.LessThanOrEqualTo:
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
