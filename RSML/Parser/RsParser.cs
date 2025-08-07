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

using RSML.Actions;
using RSML.Exceptions;
using RSML.Language;
using RSML.Machine;
using RSML.Reader;
using RSML.Tokenization;


namespace RSML.Parser
{

	/// <summary>
	/// Represents a parser for <strong>Red Sea Markup Language</strong>.
	/// </summary>
	public class RsParser
	{

		/// <summary>
		/// The current API version of the library.
		/// </summary>
		public const string ApiVersion = "2.0.0";

		/// <summary>
		/// The RSML content to evaluate.
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// The special actions of the parser.
		/// </summary>
		public Dictionary<string, SpecialAction> SpecialActions { get; }

		/// <summary>
		/// Initializes a RSML parser.
		/// </summary>
		/// <param name="content">The span of characters the parser should evaluate. This can be changed later.</param>
		public RsParser(ReadOnlySpan<char> content)
		{

			Content = content.ToString();
			SpecialActions = [ ];

		}

		/// <summary>
		/// Initializes a RSML parser.
		/// </summary>
		/// <param name="content">The string the parser should evaluate. This can be changed later.</param>
		public RsParser(string content)
		{

			Content = content;
			SpecialActions = [ ];

		}

		/// <summary>
		/// Handles a call to a special action.
		/// </summary>
		/// <param name="name">The name of the special action</param>
		/// <param name="arg">An argument of the special action or null if empty string</param>
		/// <returns></returns>
		/// <exception cref="InvalidRsmlSyntax">A line containing a special action must have at least 2 characters</exception>
		/// <exception cref="UndefinedActionException">Action is undefined but used</exception>
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

			bool systemNameMatches = tokens[1].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword  => machine.SystemName is not null,
				_                           => machine.SystemName == tokens[1].Value

			};

			bool systemVersionMatches = tokens[2].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword  => machine.SystemVersion != -1,
				_                           => machine.SystemVersion.ToString() == tokens[2].Value

			};

			bool architectureMatches = tokens[3].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                           => machine.ProcessorArchitecture == tokens[3].Value

			};

			return (systemNameMatches && systemVersionMatches && architectureMatches);

		}

		private static bool HandleLogicPath_Complex(RsToken[] tokens, in LocalMachine machine, bool isLinux)
		{

			if (isLinux)
				return HandleLogicPath_Complex_Linux(tokens, machine);

			bool systemNameMatches = tokens[1].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword  => machine.SystemName is not null,
				_                           => machine.SystemName == tokens[1].Value

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[2].Type)
			{

				case RsTokenType.Equals:
					systemVersionMatches = machine.SystemVersion.ToString() == tokens[3].Value;

					break;

				case RsTokenType.Different:
					systemVersionMatches = machine.SystemVersion.ToString() != tokens[3].Value;

					break;

				case RsTokenType.GreaterOrEqualsThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion >= versionNum;

					break;

				case RsTokenType.LessOrEqualsThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion <= versionNum;

					break;

				case RsTokenType.GreaterThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion > versionNum;

					break;

				case RsTokenType.LessThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion < versionNum;

					break;

				default:
					systemVersionMatches = false;

					break;

			}


			bool architectureMatches = tokens[4].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                           => machine.ProcessorArchitecture == tokens[3].Value

			};

			return (systemNameMatches && systemVersionMatches && architectureMatches);

		}

		private static bool HandleLogicPath_Simple_Linux(RsToken[] tokens, in LocalMachine machine)
		{

			bool systemNameMatches = tokens[1].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword  => machine.DistroName is not null,
				_ => machine.SystemName == tokens[1].Value ||
					 machine.DistroName == tokens[1].Value ||
					 machine.DistroFamily == tokens[1].Value

			};

			bool systemVersionMatches = tokens[2].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword  => machine.SystemVersion != -1,
				_                           => machine.SystemVersion.ToString() == tokens[2].Value

			};

			bool architectureMatches = tokens[3].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                           => machine.ProcessorArchitecture == tokens[3].Value

			};

			return (systemNameMatches && systemVersionMatches && architectureMatches);

		}

		private static bool HandleLogicPath_Complex_Linux(RsToken[] tokens, in LocalMachine machine)
		{

			bool systemNameMatches = tokens[1].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword  => machine.DistroName is not null,
				_ => machine.SystemName == tokens[1].Value ||
					 machine.DistroName == tokens[1].Value ||
					 machine.DistroFamily == tokens[1].Value

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[2].Type)
			{

				case RsTokenType.Equals:
					systemVersionMatches = machine.SystemVersion.ToString() == tokens[3].Value;

					break;

				case RsTokenType.Different:
					systemVersionMatches = machine.SystemVersion.ToString() != tokens[3].Value;

					break;

				case RsTokenType.GreaterOrEqualsThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion >= versionNum;

					break;

				case RsTokenType.LessOrEqualsThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion <= versionNum;

					break;

				case RsTokenType.GreaterThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion > versionNum;

					break;

				case RsTokenType.LessThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion < versionNum;

					break;

				default:
					systemVersionMatches = false;

					break;

			}


			bool architectureMatches = tokens[4].Type switch
			{

				RsTokenType.WildcardKeyword => true,
				RsTokenType.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                           => machine.ProcessorArchitecture == tokens[3].Value

			};

			return (systemNameMatches && systemVersionMatches && architectureMatches);

		}


		/// <summary>
		/// Evaluates the RSML document using the default properties and the machine's RID.
		/// </summary>
		/// <returns>A result</returns>
		public EvaluationResult Evaluate() => Evaluate(new LocalMachine());

		/// <summary>
		/// Evaluates a RSML document given some properties.
		/// </summary>
		/// <param name="machineData">The machine's data</param>
		/// <returns>An evaluation result</returns>
		public EvaluationResult Evaluate(LocalMachine machineData)
		{

			RsReader reader = new(Content);
			RsLexer lexer = new();

			while (reader.TryReadAndTokenizeLine(lexer, out var tokens))
			{

				// we basically do length-based checks
				/*
				 * Possible Lengths of tokens:
				 *	3 - Comment (#, Text, EOL)
				 *	4 - Special Action (@, Name, Arg, EOL)
				 *	6 - Logic Path (Op, Sys, Version Major = ANY, Arch, RetVal, EOL)
				 *	7 - Logic Path (Op, Sys, Version Major, Arch, RetVal, EOL)
				 *
				 */

				switch (tokens.Length)
				{

					case 3:
						// comment, ignore it
						continue;

					case 4:
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
								SpecialActions.Clear();

								continue;

							default:
								throw new ActionErrorException(
									$"A special action returned error code {result}. In the future, please use 1 to signal errors."
								);

						}

					case 6:
						if (HandleLogicPath_Simple(tokens, machineData, machineData.SystemName == "linux"))
						{

							if (tokens[0].Type == RsTokenType.ThrowErrorOperator)
								throw new UserRaisedException("Error-throw operator used.");

							return new(tokens[5].Value);

						}

						continue;

					case 7:
						if (HandleLogicPath_Complex(tokens, machineData, machineData.SystemName == "linux"))
						{

							if (tokens[0].Type == RsTokenType.ThrowErrorOperator)
								throw new UserRaisedException("Error-throw operator used.");

							return new(tokens[6].Value);

						}

						continue;

					default:
						if (tokens[0].Type == RsTokenType.CommentSymbol)
							continue; // it's somehow a comment

						throw new InvalidRsmlSyntax("Unexpected error: invalid line tokenized successfully.");

				}

			}

			return new(); // no matches

		}

		/// <summary>
		/// Checks if a given line of RSML is a comment.
		/// </summary>
		/// <param name="line">The line</param>
		/// <returns><c>true</c> if comment</returns>
		public static bool IsComment(ReadOnlySpan<char> line) =>
			line.TrimStart()[0] == '#' && !(line.IsEmpty || line.IsWhiteSpace() || line.IsNewLinesOnly());

		/// <summary>
		/// Checks if a given line of RSML is a comment.
		/// </summary>
		/// <param name="line">The line</param>
		/// <returns><c>true</c> if comment</returns>
		public static bool IsComment(string line) => IsComment(line.AsSpan());

		// ReSharper disable once GrammarMistakeInComment
		/// <summary>
		/// Registers a special action.
		/// </summary>
		/// <param name="name">The name of the action</param>
		/// <param name="action">The actual action itself</param>
		public void RegisterSpecialAction(string name, SpecialAction action) => SpecialActions[name] = action;

	}

}
