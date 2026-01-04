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

using OceanApocalypseStudios.RSML.Analyzer;
using OceanApocalypseStudios.RSML.Analyzer.Semantics;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Middlewares;
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
        public Evaluator(ReadOnlySpan<char> content) => Content = new(content);

        /// <summary>
        /// Creates a new instance of a RSML evaluator.
        /// </summary>
        /// <param name="content">The document</param>
        public Evaluator(string content) => Content = new(content);

        /// <summary>
        /// Creates a new instance of a RSML evaluator.
        /// </summary>
        /// <param name="content">The document</param>
        public Evaluator(char[] content) => Content = new(content);

        /// <summary>
        /// Creates a new instance of a RSML evaluator.
        /// </summary>
        /// <param name="content">The document</param>
        public Evaluator(ReadOnlyMemory<char> content) => Content = new(content);

        /// <summary>
        /// Creates a new instance of a RSML evaluator.
        /// </summary>
        /// <param name="content">The document</param>
        public Evaluator(ReadOnlySpan<byte> content) => Content = new(content);

        /// <summary>
        /// Creates a new instance of a RSML evaluator.
        /// </summary>
        /// <param name="content">The document</param>
        public Evaluator(byte[] content) => Content = new(content);

        /// <inheritdoc />
        public DualTextBuffer Content { get; }

		/// <summary>
		/// The amount of loaded middlewares.
		/// </summary>
		public int LoadedMiddlewaresCount => evaluatorMiddlewares.Count;

		/// <inheritdoc />
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull(ApiVersion);

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

		/// <inheritdoc />
		public EvaluationResult Evaluate() => Evaluate(new());

		/// <inheritdoc />
		public EvaluationResult Evaluate(LocalMachine machineData)
		{

			if (Content.Length == 0)
				return new();

			while (Content.CaretPosition < Content.Length || Content.BufferNumber == 2) // dont stop just cuz we swap buffers
			{

				var line = Content.ReadLine();

				if (line.IsEmpty)
					continue;

				Content.SwapBuffer();
				Content.Text = line;

				var tokens = Lexer.TokenizeLine(Content);
				Normalizer.NormalizeLine(ref tokens, out _);
				Validator.ValidateLine(tokens, Content);

				if (tokens[tokens.Last()].Kind == TokenKind.Eol)
					tokens.Remove(tokens.Last());

				Content.SwapBuffer();
				int i = Content.CaretPosition;
				Content.SwapBuffer();
				MiddlewareContext ctx = new(i, tokens, Content.Text);

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
						break;

					case 2:
						// comment, ignore it
						break;

					case 3:
						switch (Content[tokens[1].BufferRange].Span)
						{

							case "Void":
								break;

							case "ThrowError":
								throw new ActionErrorException("A special action returned an error code.");

							case "EndAll":
								return new();

							default:
								throw new ActionErrorException("Unrecognized special action.");

						}

						break;

					case 5:
						if (HandleLogicPath_Simple(tokens, Content, machineData, machineData.SystemName == "linux"))
						{

							return tokens[0].Kind == TokenKind.ThrowErrorOperator
									   ? throw new UserRaisedException("Error-throw operator used.")
									   : new(Content[tokens[4].BufferRange].Span.ToString());

						}

						break;

					case 6:
						if (HandleLogicPath_Complex(tokens, Content, machineData, machineData.SystemName == "linux"))
						{

							return tokens[0].Kind == TokenKind.ThrowErrorOperator
									   ? throw new UserRaisedException("Error-throw operator used.")
									   : new(Content[tokens[5].BufferRange].Span.ToString());

						}

						break;

					default:
						if (tokens[0].Kind == TokenKind.CommentSymbol)
							break; // it's somehow a comment

						throw new InvalidRsmlSyntax("Unexpected error: invalid line tokenized successfully.");

				}

				Content.SwapBuffer(); // back to buffer 1 so we don't infinite loop, and we don't operate on the same line everytime lol

			}

			return new(); // no matches

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

		private static bool HandleLogicPath_Complex(SyntaxLine tokens, DualTextBuffer context, in LocalMachine machine, bool isLinux)
		{

			if (isLinux)
				return HandleLogicPath_Complex_Linux(tokens, context, machine);

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.SystemName is not null,
				_                         => context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(machine.SystemName)

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[2].Kind)
			{

				case TokenKind.EqualTo:
					systemVersionMatches = context[tokens[3].BufferRange].IsEquals(machine.StringifiedSystemVersion);

					break;

				case TokenKind.NotEqualTo:
					systemVersionMatches = !context[tokens[3].BufferRange].IsEquals(machine.StringifiedSystemVersion);

					break;

				case TokenKind.GreaterThanOrEqualTo:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = machine.SystemVersion >= versionNum;

					break;

				case TokenKind.LessThanOrEqualTo:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = machine.SystemVersion <= versionNum;

					break;

				case TokenKind.GreaterThan:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = machine.SystemVersion > versionNum;

					break;

				case TokenKind.LessThan:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
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
				_                         => context[tokens[4].BufferRange].IsAsciiEqualsIgnoreCase(machine.StringifiedSystemVersion)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Complex_Linux(SyntaxLine tokens, DualTextBuffer context, in LocalMachine machine)
		{

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.DistroName is not null,
				_ => context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(machine.SystemName) ||
					 context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(machine.DistroName) ||
					 context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(machine.DistroFamily)

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[2].Kind)
			{

				case TokenKind.EqualTo:
					systemVersionMatches = context[tokens[3].BufferRange].IsEquals(machine.StringifiedSystemVersion);

					break;

				case TokenKind.NotEqualTo:
					systemVersionMatches = !context[tokens[3].BufferRange].IsEquals(machine.StringifiedSystemVersion);

					break;

				case TokenKind.GreaterThanOrEqualTo:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = machine.SystemVersion >= versionNum;

					break;

				case TokenKind.LessThanOrEqualTo:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = machine.SystemVersion <= versionNum;

					break;

				case TokenKind.GreaterThan:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = machine.SystemVersion > versionNum;

					break;

				case TokenKind.LessThan:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
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
				_                         => context[tokens[4].BufferRange].IsAsciiEqualsIgnoreCase(machine.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Simple(SyntaxLine tokens, DualTextBuffer context, in LocalMachine machine, bool isLinux)
		{

			if (isLinux)
				return HandleLogicPath_Simple_Linux(tokens, context, machine);

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.SystemName is not null,
				_                         => context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(machine.SystemName)

			};

			bool systemVersionMatches = tokens[2].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.SystemVersion != -1,
				_                         => context[tokens[2].BufferRange].IsEquals(machine.StringifiedSystemVersion)

			};

			bool architectureMatches = tokens[3].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                         => context[tokens[3].BufferRange].IsAsciiEqualsIgnoreCase(machine.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Simple_Linux(SyntaxLine tokens, DualTextBuffer context, in LocalMachine machine)
		{

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.DistroName is not null,
				_ => context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(machine.SystemName) ||
					 context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(machine.DistroName) ||
					 context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(machine.DistroFamily)

			};

			bool systemVersionMatches = tokens[2].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.SystemVersion != -1,
				_                         => context[tokens[2].BufferRange].IsEquals(machine.StringifiedSystemVersion)

			};

			bool architectureMatches = tokens[3].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                         => context[tokens[3].BufferRange].IsAsciiEqualsIgnoreCase(machine.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

	}

}
