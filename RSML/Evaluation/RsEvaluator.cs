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
using System.Linq;

using RSML.Actions;
using RSML.Exceptions;
using RSML.Machine;
using RSML.Reader;
using RSML.Semantics;
using RSML.Tokenization;


namespace RSML.Evaluation
{

	/// <summary>
	/// The officially maintained RSML evaluator that evaluates a document and returns a match's value, in case one was found.
	/// </summary>
	public sealed class RsEvaluator : IEvaluator
	{

		private static readonly RsToken wildcard = new(RsTokenType.WildcardKeyword, "any");
		private static readonly RsToken defined = new(RsTokenType.DefinedKeyword, "defined");
		private static readonly RsToken eol = new(RsTokenType.Eol, Environment.NewLine);

		/// <inheritdoc/>
		public string? StandardizedVersion => "2.0.0";

		/// <inheritdoc/>
		public string Content { get; set; }

		/// <inheritdoc/>
		public Dictionary<string, SpecialAction> SpecialActions { get; }

		/// <summary>
		/// Creates a new instance of a RSML evaluator.
		/// </summary>
		/// <param name="content">The document</param>
		public RsEvaluator(ReadOnlySpan<char> content)
		{

			Content = $"{content.ToString().ReplaceLineEndings()}{Environment.NewLine}";
			SpecialActions = [];

		}

		/// <summary>
		/// Creates a new instance of a RSML evaluator.
		/// </summary>
		/// <param name="content">The document</param>
		public RsEvaluator(string content)
		{

			Content = $"{content.ReplaceLineEndings()}{Environment.NewLine}";
			SpecialActions = [];

		}

		private byte HandleSpecialActionCall(string name, string? arg)
		{

			if (name == "EndAll")
				return SpecialActionBehavior.StopEvaluation;

			if (!SpecialActions.TryGetValue(name, out var action))
				throw new UndefinedActionException("Action is undefined but used");

			return action.Invoke(this, arg ?? "");

		}

		private static bool HandleLogicPath_Simple(RsToken[] tokens, in LocalMachine machine, bool isLinux)
		{

			if (isLinux)
				return HandleLogicPath_Simple_Linux(tokens, machine);

			bool systemNameMatches = tokens[ 1 ].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword => machine.SystemName is not null,
				_ => machine.SystemName == tokens[ 1 ].Value

			};

			bool systemVersionMatches = tokens[ 2 ].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword => machine.SystemVersion != -1,
				_ => machine.SystemVersion.ToString() == tokens[ 2 ].Value

			};

			bool architectureMatches = tokens[ 3 ].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword => machine.ProcessorArchitecture is not null,
				_ => machine.ProcessorArchitecture == tokens[ 3 ].Value

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Complex(RsToken[] tokens, in LocalMachine machine, bool isLinux)
		{

			if (isLinux)
				return HandleLogicPath_Complex_Linux(tokens, machine);

			bool systemNameMatches = tokens[ 1 ].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword => machine.SystemName is not null,
				_ => machine.SystemName == tokens[ 1 ].Value

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[ 2 ].Type)
			{

				case RsTokenType.Equals:
					systemVersionMatches = machine.SystemVersion.ToString() == tokens[ 3 ].Value;

					break;

				case RsTokenType.Different:
					systemVersionMatches = machine.SystemVersion.ToString() != tokens[ 3 ].Value;

					break;

				case RsTokenType.GreaterOrEqualsThan:
					if (Int32.TryParse(tokens[ 3 ].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion >= versionNum;

					break;

				case RsTokenType.LessOrEqualsThan:
					if (Int32.TryParse(tokens[ 3 ].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion <= versionNum;

					break;

				case RsTokenType.GreaterThan:
					if (Int32.TryParse(tokens[ 3 ].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion > versionNum;

					break;

				case RsTokenType.LessThan:
					if (Int32.TryParse(tokens[ 3 ].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion < versionNum;

					break;

				default:
					systemVersionMatches = false;

					break;

			}

			bool architectureMatches = tokens[ 4 ].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword => machine.ProcessorArchitecture is not null,
				_ => machine.ProcessorArchitecture == tokens[ 3 ].Value

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Simple_Linux(RsToken[] tokens, in LocalMachine machine)
		{

			bool systemNameMatches = tokens[ 1 ].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword => machine.DistroName is not null,
				_ => machine.SystemName == tokens[ 1 ].Value ||
					 machine.DistroName == tokens[ 1 ].Value ||
					 machine.DistroFamily == tokens[ 1 ].Value

			};

			bool systemVersionMatches = tokens[ 2 ].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword => machine.SystemVersion != -1,
				_ => machine.SystemVersion.ToString() == tokens[ 2 ].Value

			};

			bool architectureMatches = tokens[ 3 ].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword => machine.ProcessorArchitecture is not null,
				_ => machine.ProcessorArchitecture == tokens[ 3 ].Value

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Complex_Linux(RsToken[] tokens, in LocalMachine machine)
		{

			bool systemNameMatches = tokens[ 1 ].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword => machine.DistroName is not null,
				_ => machine.SystemName == tokens[ 1 ].Value ||
					 machine.DistroName == tokens[ 1 ].Value ||
					 machine.DistroFamily == tokens[ 1 ].Value

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[ 2 ].Type)
			{

				case RsTokenType.Equals:
					systemVersionMatches = machine.SystemVersion.ToString() == tokens[ 3 ].Value;

					break;

				case RsTokenType.Different:
					systemVersionMatches = machine.SystemVersion.ToString() != tokens[ 3 ].Value;

					break;

				case RsTokenType.GreaterOrEqualsThan:
					if (Int32.TryParse(tokens[ 3 ].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion >= versionNum;

					break;

				case RsTokenType.LessOrEqualsThan:
					if (Int32.TryParse(tokens[ 3 ].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion <= versionNum;

					break;

				case RsTokenType.GreaterThan:
					if (Int32.TryParse(tokens[ 3 ].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion > versionNum;

					break;

				case RsTokenType.LessThan:
					if (Int32.TryParse(tokens[ 3 ].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion < versionNum;

					break;

				default:
					systemVersionMatches = false;

					break;

			}


			bool architectureMatches = tokens[ 4 ].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword => machine.ProcessorArchitecture is not null,
				_ => machine.ProcessorArchitecture == tokens[ 3 ].Value

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static RsToken[] NormalizeTokens(IEnumerable<RsToken> tokens)
		{

			var actualTokens = tokens.ToArray();

			return actualTokens[ 0 ].Type is RsTokenType.ReturnOperator or RsTokenType.ThrowErrorOperator
				? actualTokens.Length switch
				{

					// eol matters
					3 => [ actualTokens[ 0 ], wildcard, wildcard, wildcard, actualTokens[ 1 ], eol ],
					4 => [ actualTokens[ 0 ], actualTokens[ 1 ], wildcard, wildcard, actualTokens[ 2 ], eol ],
					5 => [ actualTokens[ 0 ], actualTokens[ 1 ], wildcard, actualTokens[ 2 ], actualTokens[ 3 ], eol ],
					6 => [ actualTokens[ 0 ], actualTokens[ 1 ], actualTokens[ 2 ], actualTokens[ 3 ], actualTokens[ 4 ], eol ],
					7 => [ actualTokens[ 0 ], actualTokens[ 1 ], actualTokens[ 2 ], actualTokens[ 3 ], actualTokens[ 4 ], actualTokens[ 5 ], eol ],
					_ => [],

				}
				: actualTokens;

		}

		/// <inheritdoc/>
		public EvaluationResult Evaluate() => Evaluate(new LocalMachine());

		/// <inheritdoc/>
		public EvaluationResult Evaluate(LocalMachine machineData) => Evaluate(machineData, new RsReader(Content), new RsLexer(), new RsValidator());

		/// <inheritdoc/>
		public EvaluationResult Evaluate(LocalMachine machineData, IReader? reader, ILexer? lexer, IValidator? validator)
		{

			if (Content.Length == 0)
				return new();

			reader ??= new RsReader(Content);
			lexer ??= new RsLexer();
			validator ??= new RsValidator();

			if (reader.StandardizedVersion != StandardizedVersion || lexer.StandardizedVersion != StandardizedVersion || validator.StandardizedVersion != StandardizedVersion)
				throw new ArgumentException("One of the specified components does not respect the same language standard version as the evaluator's.");

			while (reader.TryTokenizeNextLine(lexer, out var rawTokens))
			{

				var tokens = NormalizeTokens(rawTokens);
				validator.ValidateLine(tokens.AsSpan()); // line validation

				if (tokens[ ^1 ].Type == RsTokenType.Eol)
					tokens = tokens[ ..^1 ];

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
						byte result = HandleSpecialActionCall(tokens[ 1 ].Value, tokens[ 2 ].Value);

						switch (result)
						{

							case SpecialActionBehavior.NoBehavior:
								continue;

							case SpecialActionBehavior.Error:
								throw new ActionStandardErrorException("A special action returned error code 1.");

							case SpecialActionBehavior.StopEvaluation:
								return new();

							case SpecialActionBehavior.ResetSpecials:
								SpecialActions.Clear();

								continue;

							default:
								throw new ActionErrorException(
									$"A special action returned error code {result}. In the future, please use 1 to signal errors."
								);

						}

					case 5:
						if (HandleLogicPath_Simple(tokens, machineData, machineData.SystemName == "linux"))
						{

							return tokens[ 0 ].Type == RsTokenType.ThrowErrorOperator
								? throw new UserRaisedException("Error-throw operator used.")
								: new(tokens[ 4 ].Value);

						}

						continue;

					case 6:
						if (HandleLogicPath_Complex(tokens, machineData, machineData.SystemName == "linux"))
						{

							return tokens[ 0 ].Type == RsTokenType.ThrowErrorOperator
								? throw new UserRaisedException("Error-throw operator used.")
								: new(tokens[ 5 ].Value);
						}

						continue;

					default:
						if (tokens[ 0 ].Type == RsTokenType.CommentSymbol)
							continue; // it's somehow a comment

						throw new InvalidRsmlSyntax("Unexpected error: invalid line tokenized successfully.");

				}

			}

			return new(); // no matches

		}

		/// <inheritdoc/>
		public bool IsComment(ReadOnlySpan<char> line) =>
			line.TrimStart()[ 0 ] == '#' && !(line.IsEmpty || line.IsWhiteSpace() || line.IsNewLinesOnly());

		/// <inheritdoc/>
		public bool IsComment(string line) => IsComment(line.AsSpan());

		/// <inheritdoc/>
		public void RegisterSpecialAction(string name, SpecialAction action)
		{

			if (name.StartsWith('@') || name.StartsWith('#') || name == "EndAll")
				throw new ArgumentException("The name of the special action must not be EndAll or start with @ or #.", nameof(name));

			SpecialActions[ name ] = action;

		}

	}

}
