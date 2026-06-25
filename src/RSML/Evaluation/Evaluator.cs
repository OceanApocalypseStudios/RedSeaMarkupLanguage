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

using OceanApocalypseStudios.RSML.Analyzer;
using OceanApocalypseStudios.RSML.Analyzer.Semantics;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Toolchain.Compliance;


namespace OceanApocalypseStudios.RSML.Evaluation
{

	/// <summary>
	/// The officially maintained RSML evaluator that evaluates a document and returns a match's value, in case one was found.
	/// </summary>
	public sealed class Evaluator : IEvaluator
	{

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
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull(ApiVersion);

		/// <inheritdoc />
		public DualTextBuffer Content { get; }

		/// <inheritdoc />
		public static bool IsComment(ReadOnlySpan<char> line) => line.TrimStart()[0] == '#' && !(line.IsEmpty || line.IsWhiteSpace() || line.IsNewLinesOnly());

		/// <inheritdoc />
		public static bool IsComment(string line) => IsComment(line.AsSpan());

		/// <inheritdoc />
		public EvaluationResult Evaluate() => Evaluate(new());

		/// <inheritdoc />
		public EvaluationResult Evaluate(Host host)
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

				if (tokens.GetLast().Kind == TokenKind.Eol)
					tokens.Remove(tokens.IndexOfLast);

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
								throw new ActionErrorException("Special action returned error code");

							case "EndAll":
								return new();

							default:
								throw new ActionErrorException("Unrecognized special action (possibly a non-standard action)");

						}

						break;

					case 5:
						if (HandleLogicPath_Simple(
								tokens,
								Content,
								host,
								host.IsLinux
							))
						{

							return tokens[0].Kind == TokenKind.ThrowErrorOperator
									   ? throw new UserRaisedException("Error-throwing operator was used")
									   : new(Content[tokens[4].BufferRange].Span.ToString());

						}

						break;

					case 6:
						if (HandleLogicPath_Complex(
								tokens,
								Content,
								host,
								host.IsLinux
							))
						{

							return tokens[0].Kind == TokenKind.ThrowErrorOperator
									   ? throw new UserRaisedException("Error-throwing operator was used")
									   : new(Content[tokens[5].BufferRange].Span.ToString());

						}

						break;

					default:
						if (tokens[0].Kind == TokenKind.CommentSymbol)
							break; // it's somehow a comment

						throw new InvalidRsmlSyntax("Invalid line was tokenized successfully");

				}

				Content.SwapBuffer(); // back to buffer 1 so we don't infinite loop, and we don't operate on the same line everytime lol

			}

			return new(); // no matches

		}

		private const string ApiVersion = "2.0.0";

		private static bool HandleLogicPath_Complex(
			SyntaxLine tokens,
			DualTextBuffer context,
			in Host host,
			bool isLinux
		)
		{

			if (isLinux)
				return HandleLogicPath_Complex_Linux(tokens, context, host);

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => host.SystemName is not null,
				_                         => context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(host.SystemName)

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[2].Kind)
			{

				case TokenKind.EqualTo:
					systemVersionMatches = context[tokens[3].BufferRange].IsEquals(host.StringifiedSystemVersion);

					break;

				case TokenKind.NotEqualTo:
					systemVersionMatches = !context[tokens[3].BufferRange].IsEquals(host.StringifiedSystemVersion);

					break;

				case TokenKind.GreaterThanOrEqualTo:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = host.SystemVersion >= versionNum;

					break;

				case TokenKind.LessThanOrEqualTo:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = host.SystemVersion <= versionNum;

					break;

				case TokenKind.GreaterThan:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = host.SystemVersion > versionNum;

					break;

				case TokenKind.LessThan:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = host.SystemVersion < versionNum;

					break;

				default:
					systemVersionMatches = false;

					break;

			}

			bool architectureMatches = tokens[4].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => host.ProcessorArchitecture is not null,
				_                         => context[tokens[4].BufferRange].IsAsciiEqualsIgnoreCase(host.StringifiedSystemVersion)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Complex_Linux(
			SyntaxLine tokens,
			DualTextBuffer context,
			in Host host
		)
		{

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => host.DistroName is not null,
				_ => context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(host.SystemName) ||
					 context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(host.DistroName) ||
					 context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(host.DistroFamily)

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[2].Kind)
			{

				case TokenKind.EqualTo:
					systemVersionMatches = context[tokens[3].BufferRange].IsEquals(host.StringifiedSystemVersion);

					break;

				case TokenKind.NotEqualTo:
					systemVersionMatches = !context[tokens[3].BufferRange].IsEquals(host.StringifiedSystemVersion);

					break;

				case TokenKind.GreaterThanOrEqualTo:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = host.SystemVersion >= versionNum;

					break;

				case TokenKind.LessThanOrEqualTo:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = host.SystemVersion <= versionNum;

					break;

				case TokenKind.GreaterThan:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = host.SystemVersion > versionNum;

					break;

				case TokenKind.LessThan:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = host.SystemVersion < versionNum;

					break;

				default:
					systemVersionMatches = false;

					break;

			}


			bool architectureMatches = tokens[4].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => host.ProcessorArchitecture is not null,
				_                         => context[tokens[4].BufferRange].IsAsciiEqualsIgnoreCase(host.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Simple(
			SyntaxLine tokens,
			DualTextBuffer context,
			in Host host,
			bool isLinux
		)
		{

			if (isLinux)
				return HandleLogicPath_Simple_Linux(tokens, context, host);

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => host.SystemName is not null,
				_                         => context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(host.SystemName)

			};

			bool systemVersionMatches = tokens[2].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => host.SystemVersion != -1,
				_                         => context[tokens[2].BufferRange].IsEquals(host.StringifiedSystemVersion)

			};

			bool architectureMatches = tokens[3].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => host.ProcessorArchitecture is not null,
				_                         => context[tokens[3].BufferRange].IsAsciiEqualsIgnoreCase(host.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Simple_Linux(
			SyntaxLine tokens,
			DualTextBuffer context,
			in Host host
		)
		{

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => host.DistroName is not null,
				_ => context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(host.SystemName) ||
					 context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(host.DistroName) ||
					 context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(host.DistroFamily)

			};

			bool systemVersionMatches = tokens[2].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => host.SystemVersion != -1,
				_                         => context[tokens[2].BufferRange].IsEquals(host.StringifiedSystemVersion)

			};

			bool architectureMatches = tokens[3].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => host.ProcessorArchitecture is not null,
				_                         => context[tokens[3].BufferRange].IsAsciiEqualsIgnoreCase(host.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

	}

}
