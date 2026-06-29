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
 *													 (MF366)
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

using OceanApocalypseStudios.RSML.Analyzer.Semantics;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Host;


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
		public DualTextBuffer Content { get; }

		/// <inheritdoc />
		public static bool IsComment(ReadOnlySpan<char> line) => line.TrimStart()[0] == '#' && !(line.IsEmpty || line.IsWhiteSpace() || line.IsNewLinesOnly());

		/// <inheritdoc />
		public static bool IsComment(string line) => IsComment(line.AsSpan());

		/// <inheritdoc />
		public EvaluationResult Evaluate() => Evaluate(new());

		/// <inheritdoc />
		public EvaluationResult Evaluate(HostInfo hostInfo)
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
								if (tokens[2].BufferRange.Start.Equals(^1) && tokens[2].BufferRange.End.Equals(0))
									throw new ActionErrorException($"Special action returned error code");

								throw new ActionErrorException($"Special action returned error code : {Content[tokens[2].BufferRange].Span}");

							case "EndAll":
								if (tokens[2].BufferRange.Start.Equals(^1) && tokens[2].BufferRange.End.Equals(0))
									return new();

								return Content[tokens[2].BufferRange].Span.IsEmpty ? new() : new(Content[tokens[2].BufferRange].Span);

							default:
								throw new ActionErrorException("Unrecognized special action (possibly a non-standard action)");

						}

						break;

					case 5:
						bool isMatchSimple = HandleLogicPath_Simple(tokens, Content, hostInfo, hostInfo.IsLinux);

						if (isMatchSimple)
						{

							switch (tokens[0].Kind)
							{

								case TokenKind.ReturnOperator:
									return new(Content[tokens[4].BufferRange].Span.ToString());

								case TokenKind.ThrowErrorOperator:
									throw new UserRaisedException("Error-throwing operator was used");

								case TokenKind.ReverseReturnOperator:
									break; // do nothing

							}

						}

						if (!isMatchSimple && tokens[0].Kind == TokenKind.ReverseReturnOperator) // HandleLogicPath_Simple returned false
							return new(Content[tokens[4].BufferRange].Span.ToString());

						break;

					case 6:
						bool isMatchComplex = HandleLogicPath_Complex(tokens, Content, hostInfo, hostInfo.IsLinux);
						
						if (isMatchComplex)
						{

							switch (tokens[0].Kind)
							{

								case TokenKind.ReturnOperator:
									return new(Content[tokens[5].BufferRange].Span.ToString());

								case TokenKind.ThrowErrorOperator:
									throw new UserRaisedException("Error-throwing operator was used");

								case TokenKind.ReverseReturnOperator:
									break; // do nothing

							}

						}

						if (!isMatchComplex && tokens[0].Kind == TokenKind.ReverseReturnOperator) // HandleLogicPath_Complex returned false
							return new(Content[tokens[5].BufferRange].Span.ToString());

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

		private static bool HandleLogicPath_Complex(
			SyntaxLine tokens,
			DualTextBuffer context,
			in HostInfo hostInfo,
			bool isLinux
		)
		{

			if (isLinux)
				return HandleLogicPath_Complex_Linux(tokens, context, hostInfo);

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword  => true,
				TokenKind.DefinedKeyword   => hostInfo.SystemName is not null,
				TokenKind.UndefinedKeyword => hostInfo.SystemName is null,
				_                          => context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(hostInfo.SystemName)

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[2].Kind)
			{

				case TokenKind.EqualTo:
					systemVersionMatches = context[tokens[3].BufferRange].IsEquals(hostInfo.StringifiedSystemVersion);

					break;

				case TokenKind.NotEqualTo:
					systemVersionMatches = !context[tokens[3].BufferRange].IsEquals(hostInfo.StringifiedSystemVersion);

					break;

				case TokenKind.GreaterThanOrEqualTo:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = hostInfo.SystemVersion >= versionNum;

					break;

				case TokenKind.LessThanOrEqualTo:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = hostInfo.SystemVersion <= versionNum;

					break;

				case TokenKind.GreaterThan:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = hostInfo.SystemVersion > versionNum;

					break;

				case TokenKind.LessThan:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = hostInfo.SystemVersion < versionNum;

					break;

				case TokenKind.Integer:
					if (Int32.TryParse(context[tokens[2].BufferRange].Span, out int minVersionNum) && Int32.TryParse(context[tokens[3].BufferRange].Span, out int maxVersionNum))
						systemVersionMatches = hostInfo.SystemVersion >= minVersionNum && hostInfo.SystemVersion <= maxVersionNum;

					break;

				default:
					systemVersionMatches = false;

					break;

			}

			bool architectureMatches = tokens[4].Kind switch
			{

				TokenKind.WildcardKeyword  => true,
				TokenKind.DefinedKeyword   => hostInfo.ProcessorArchitecture is not null,
				TokenKind.UndefinedKeyword => hostInfo.ProcessorArchitecture is null,
				_                          => context[tokens[4].BufferRange].IsAsciiEqualsIgnoreCase(hostInfo.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Complex_Linux(
			SyntaxLine tokens,
			DualTextBuffer context,
			in HostInfo hostInfo
		)
		{

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword  => true,
				TokenKind.DefinedKeyword   => hostInfo.DistroName is not null && hostInfo.DistroFamily is not null,
				TokenKind.UndefinedKeyword => hostInfo.DistroName is null && hostInfo.DistroFamily is null,
				_ => context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(hostInfo.SystemName) ||
					 context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(hostInfo.DistroName) ||
					 context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(hostInfo.DistroFamily)

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[2].Kind)
			{

				case TokenKind.EqualTo:
					systemVersionMatches = context[tokens[3].BufferRange].IsEquals(hostInfo.StringifiedSystemVersion);

					break;

				case TokenKind.NotEqualTo:
					systemVersionMatches = !context[tokens[3].BufferRange].IsEquals(hostInfo.StringifiedSystemVersion);

					break;

				case TokenKind.GreaterThanOrEqualTo:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = hostInfo.SystemVersion >= versionNum;

					break;

				case TokenKind.LessThanOrEqualTo:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = hostInfo.SystemVersion <= versionNum;

					break;

				case TokenKind.GreaterThan:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = hostInfo.SystemVersion > versionNum;

					break;

				case TokenKind.LessThan:
					if (Int32.TryParse(context[tokens[3].BufferRange].Span, out versionNum))
						systemVersionMatches = hostInfo.SystemVersion < versionNum;

					break;

				case TokenKind.Integer:
					if (Int32.TryParse(context[tokens[2].BufferRange].Span, out int minVersionNum) && Int32.TryParse(context[tokens[3].BufferRange].Span, out int maxVersionNum))
						systemVersionMatches = hostInfo.SystemVersion >= minVersionNum && hostInfo.SystemVersion <= maxVersionNum;

					break;

				default:
					systemVersionMatches = false;

					break;

			}


			bool architectureMatches = tokens[4].Kind switch
			{

				TokenKind.WildcardKeyword  => true,
				TokenKind.DefinedKeyword   => hostInfo.ProcessorArchitecture is not null,
				TokenKind.UndefinedKeyword => hostInfo.ProcessorArchitecture is null,
				_                          => context[tokens[4].BufferRange].IsAsciiEqualsIgnoreCase(hostInfo.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Simple(
			SyntaxLine tokens,
			DualTextBuffer context,
			in HostInfo hostInfo,
			bool isLinux
		)
		{

			if (isLinux)
				return HandleLogicPath_Simple_Linux(tokens, context, hostInfo);

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword  => true,
				TokenKind.DefinedKeyword   => hostInfo.SystemName is not null,
				TokenKind.UndefinedKeyword => hostInfo.SystemName is null,
				_                          => context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(hostInfo.SystemName)

			};

			bool systemVersionMatches = tokens[2].Kind switch
			{

				TokenKind.WildcardKeyword  => true,
				TokenKind.DefinedKeyword   => hostInfo.SystemVersion != -1,
				TokenKind.UndefinedKeyword => hostInfo.SystemVersion == -1,
				_                          => context[tokens[2].BufferRange].IsEquals(hostInfo.StringifiedSystemVersion)

			};

			bool architectureMatches = tokens[3].Kind switch
			{

				TokenKind.WildcardKeyword  => true,
				TokenKind.DefinedKeyword   => hostInfo.ProcessorArchitecture is not null,
				TokenKind.UndefinedKeyword => hostInfo.ProcessorArchitecture is null,
				_                          => context[tokens[3].BufferRange].IsAsciiEqualsIgnoreCase(hostInfo.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Simple_Linux(
			SyntaxLine tokens,
			DualTextBuffer context,
			in HostInfo hostInfo
		)
		{

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword  => true,
				TokenKind.DefinedKeyword   => hostInfo.DistroName is not null && hostInfo.DistroFamily is not null,
				TokenKind.UndefinedKeyword => hostInfo.DistroName is null && hostInfo.DistroFamily is null,
				_ => context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(hostInfo.SystemName) ||
					 context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(hostInfo.DistroName) ||
					 context[tokens[1].BufferRange].IsAsciiEqualsIgnoreCase(hostInfo.DistroFamily)

			};

			bool systemVersionMatches = tokens[2].Kind switch
			{

				TokenKind.WildcardKeyword  => true,
				TokenKind.DefinedKeyword   => hostInfo.SystemVersion != -1,
				TokenKind.UndefinedKeyword => hostInfo.SystemVersion == -1,
				_                          => context[tokens[2].BufferRange].IsEquals(hostInfo.StringifiedSystemVersion)

			};

			bool architectureMatches = tokens[3].Kind switch
			{

				TokenKind.WildcardKeyword  => true,
				TokenKind.DefinedKeyword   => hostInfo.ProcessorArchitecture is not null,
				TokenKind.UndefinedKeyword => hostInfo.ProcessorArchitecture is null,
				_                          => context[tokens[3].BufferRange].IsAsciiEqualsIgnoreCase(hostInfo.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

	}

}
