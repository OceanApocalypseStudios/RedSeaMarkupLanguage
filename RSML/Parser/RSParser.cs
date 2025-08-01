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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using RSML.Actions;
using RSML.Exceptions;
using RSML.Language;
using RSML.Reader;


namespace RSML.Parser
{

	/// <summary>
	/// Represents a parser for <strong>Red Sea Markup Language</strong>.
	/// </summary>
	public ref struct RSParser
	{

		/// <summary>
		/// The current API version of the library.
		/// </summary>
		public const string ApiVersion = "2.0.0";

		/// <summary>
		/// The RSML content to evaluate.
		/// </summary>
		public ReadOnlySpan<char> Content { get; set; }

		/// <summary>
		/// The RSML language standard to apply.
		/// </summary>
		public LanguageStandard LanguageStandard { get; set; }

		/// <summary>
		/// Initializes a RSML parser with a defined primary operator and some content to
		/// evaluate.
		/// </summary>
		/// <param name="content">The span of characters the parser should evaluate. This can be changed later.</param>
		public RSParser(ReadOnlySpan<char> content)
		{

			Content = content;
			LanguageStandard = LanguageStandard.Official25;

		}

		/// <summary>
		/// Initializes a RSML parser from a language standard.
		/// </summary>
		/// <param name="content">The span of characters the parser should evaluate. This can be changed later.</param>
		/// <param name="languageStandard">The RSML language standard to use.</param>
		public RSParser(ReadOnlySpan<char> content, LanguageStandard languageStandard)
		{

			Content = content;
			LanguageStandard = languageStandard;

		}

		/// <summary>
		/// Initializes a RSML parser from a language standard.
		/// </summary>
		/// <param name="content">The string the parser should evaluate. This can be changed later.</param>
		/// <param name="languageStandard">The RSML language standard to use.</param>
		public RSParser(string content, LanguageStandard languageStandard)
		{

			Content = content;
			LanguageStandard = languageStandard;

		}

		/// <summary>
		/// Handles a call to a special action.
		/// </summary>
		/// <param name="name">The name of the special action</param>
		/// <param name="arg">An argument of the special action or null if empty string</param>
		/// <returns></returns>
		/// <exception cref="InvalidRSMLSyntax">A line containing a special action must have at least 2 characters</exception>
		/// <exception cref="UndefinedActionException">Action is undefined but used</exception>
		private readonly byte HandleSpecialActionCall(ReadOnlySpan<char> name, ReadOnlySpan<char> arg)
		{

			if (name == "EndAll")
				return SpecialActionBehavior.StopEvaluation;

			if (!LanguageStandard.SpecialActions.TryGetValue(name.ToString(), out var action))
				throw new UndefinedActionException("Action is undefined but used");

			return action.Invoke(this, arg.ToString());

		}

		/// <summary>
		/// Handles a call to an operator's action.
		/// </summary>
		/// <param name="operatorType">The operator type</param>
		/// <param name="left">Left side of logic path</param>
		/// <param name="right">Right side of logic path</param>
		/// <param name="properties">Evaluation properties</param>
		/// <returns>The operator's right side or <c>null</c> if there was no match</returns>
		private readonly string? HandleOperatorAction(OperatorType operatorType, ReadOnlySpan<char> left, ReadOnlySpan<char> right, in EvaluationProperties properties)
		{

			if (left == "any" && properties.ExpandAnyIntoRegularExpression)
				left = ".+";

			// actual evaluation
			if (!Regex.IsMatch(properties.RuntimeIdentifier, $"^{left}$"))
				return null; // no bueno

			string actualReturnValue = right[1..^1].ToString(); // this trims the quotes

			switch (operatorType)
			{

				case OperatorType.Secondary:
					LanguageStandard.SecondaryOperatorAction!.Invoke(this, actualReturnValue);
					break;

				case OperatorType.Tertiary:
					LanguageStandard.TertiaryOperatorAction!.Invoke(this, actualReturnValue);
					break;

			}

			return actualReturnValue;

		}

		/// <summary>
		/// Evaluates the RSML document using the default properties and the machine's RID.
		/// </summary>
		/// <returns>A result</returns>
		public EvaluationResult Evaluate() => Evaluate(RuntimeInformation.RuntimeIdentifier);

		/// <summary>
		/// Evaluates the RSML document given a custom RID.
		/// </summary>
		/// <param name="rid">The RID to use</param>
		/// <returns>A result</returns>
		public EvaluationResult Evaluate(string rid) => Evaluate(
			new EvaluationProperties(rid, false)
		);

		/// <summary>
		/// Evaluates the RSML document given a custom RID and an <c>expandAny</c> flag.
		/// </summary>
		/// <param name="rid">The RID to use</param>
		/// <param name="expandAny"><c>true</c> if <c>any</c> should be expanded into <c>.+</c></param>
		/// <returns>A result</returns>
		public EvaluationResult Evaluate(string rid, bool expandAny) => Evaluate(
			new EvaluationProperties(rid, expandAny)
		);

		/// <summary>
		/// Evaluates the RSML document given an <c>expandAny</c> flag and a strictness level.
		/// The RID will be the machine's RID.
		/// </summary>
		/// <param name="expandAny"><c>true</c> if <c>any</c> should be expanded into <c>.+</c></param>
		/// <returns>A result</returns>
		public EvaluationResult Evaluate(bool expandAny) => Evaluate(
			new EvaluationProperties(RuntimeInformation.RuntimeIdentifier, expandAny)
		);

		/// <summary>
		/// Evaluates a RSML document given some properties.
		/// </summary>
		/// <param name="properties">The set of properties to feed into the evaluation</param>
		/// <returns>An evaluation result</returns>
		/// <exception cref="UndefinedOperatorException">All operators and their actions must be defined</exception>
		public EvaluationResult Evaluate(EvaluationProperties properties)
		{

			if (LanguageStandard.PrimaryOperatorSymbol.Length < 1
				|| LanguageStandard.SecondaryOperatorSymbol.Length < 1
				|| LanguageStandard.TertiaryOperatorSymbol.Length < 1)
				throw new UndefinedOperatorException("All operators must have a symbol by this point");

			if (LanguageStandard.SecondaryOperatorAction is null || LanguageStandard.TertiaryOperatorAction is null)
				throw new UndefinedOperatorException("All operator's actions must be defined at this point");

			RSReader reader = new(Content);
			RSTokenizer tokenizer = new();

			while (reader.TryReadAndTokenizeLine(tokenizer, LanguageStandard, out var tokens))
			{

				switch (tokens.Length)
				{

					case 0:
					case 1:
						// only valid for comments and eof and eol and malformed lines because:
						// special actions take 2 (handler + name) or even 3 (handler + name + arg)
						// logic paths take 3 (left + op + right)
						continue;

					case 2:
						// special action with no arg
						if (tokens[0].Type != RSTokenType.SpecialActionHandler)
							continue;

						if (tokens[1].Type != RSTokenType.SpecialActionName)
							continue;

						if (tokens[1].Value.Span == "EndAll")
							return new();

						byte result;

						try
						{

							result = HandleSpecialActionCall(tokens[1].Value.Span, "");

						}
						catch (InvalidRSMLSyntax) { throw; }
						catch (UndefinedActionException) { throw; }

						switch (result)
						{

							case SpecialActionBehavior.Success:
								break;

							case SpecialActionBehavior.Error:
								// always use 1 for errors, no matter the default case - it may change in the future
								// but this one won't
								throw new ActionErrorException("Action raised error with status code 1");

							case SpecialActionBehavior.StopEvaluation:
								return new();

							case SpecialActionBehavior.ResetSpecials:
								LanguageStandard.SpecialActions.Clear();
								break;

							case SpecialActionBehavior.ResetOperators:
								// we are going to reset to official-25's operators
								// todo: document this. later???? maybe???? i dunno...

								var official25 = LanguageStandard.Official25;
								LanguageStandard = new(
									official25.PrimaryOperatorSymbol,
									official25.SecondaryOperatorSymbol,
									official25.TertiaryOperatorSymbol,
									(_, _) => { },
									(_, _) => { },
									LanguageStandard.SpecialActions
								);

								break;

							default:
								// anything else is an error tho it's recommended to use 1 as error
								// as we may add other special behaviors
								throw new ActionErrorException("Action raised error with status code 1 - if you're the creator of the file, please strictly use code 1 for errors in the future.");

						}

						continue;

					case 3:
						// special actions with argument or like logic paths
						if (tokens[0].Type == RSTokenType.SpecialActionHandler)
						{

							if (tokens[1].Type != RSTokenType.SpecialActionName)
								continue;

							if (tokens[1].Value.Span == "EndAll")
								return new();

							byte res;

							try
							{

								res = HandleSpecialActionCall(tokens[1].Value.Span, tokens[2].Value.Span);

							}
							catch (InvalidRSMLSyntax) { throw; }
							catch (UndefinedActionException) { throw; }

							switch (res)
							{

								case SpecialActionBehavior.Success:
									break;

								case SpecialActionBehavior.Error:
									// always use 1 for errors, no matter the default case - it may change in the future
									// but this one won't
									throw new ActionErrorException("Action raised error with status code 1");

								case SpecialActionBehavior.StopEvaluation:
									return new();

								case SpecialActionBehavior.ResetSpecials:
									LanguageStandard.SpecialActions.Clear();
									break;

								case SpecialActionBehavior.ResetOperators:
									// we are going to reset to official-25's operators
									// todo: document this. later???? maybe???? i dunno...

									var official25 = LanguageStandard.Official25;
									LanguageStandard = new(
										official25.PrimaryOperatorSymbol,
										official25.SecondaryOperatorSymbol,
										official25.TertiaryOperatorSymbol,
										(_, _) => { },
										(_, _) => { },
										LanguageStandard.SpecialActions
									);

									break;

								default:
									// anything else is an error tho it's recommended to use 1 as error
									// as we may add other special behaviors
									throw new ActionErrorException("Action raised error with status code 1 - if you're the creator of the file, please strictly use code 1 for errors in the future.");

							}

							continue;

						}

						if (tokens[0].Type != RSTokenType.LogicPathLeft)
							continue;

						if (tokens[1].Type != RSTokenType.PrimaryOperator
							&& tokens[1].Type != RSTokenType.SecondaryOperator
							&& tokens[1].Type != RSTokenType.TertiaryOperator)
							continue;

						if (tokens[2].Type != RSTokenType.LogicPathRight)
							continue;

						if (tokens[2].Value.Span.Length < 3)
							throw new InvalidRSMLSyntax("Right side of logic path malformed");

						if (tokens[2].Value.Span[0] != '"' || tokens[2].Value.Span[^1] != '"')
							throw new InvalidRSMLSyntax("Right side of logic path malformed - should be enclosed in double quotes");

						var returnResult = HandleOperatorAction(
							tokens[1].Type == RSTokenType.PrimaryOperator
								? OperatorType.Primary
								: (tokens[1].Type == RSTokenType.SecondaryOperator)
									? OperatorType.Secondary
									: OperatorType.Tertiary,
							tokens[0].Value.Span,
							tokens[2].Value.Span,
							properties
						);

						if (returnResult is not null && tokens[1].Type == RSTokenType.PrimaryOperator)
							return new(returnResult);

						continue;

					default:
						continue; // 4 tokens would be unprecedent

				}

			}

			return new();

		}

		/// <summary>
		/// Gets the type of comment of a line of RSML, taking into consideration
		/// the parser's properties, or <c>null</c> if the line is not a comment.
		/// </summary>
		/// <param name="line">A line of RSML</param>
		/// <returns>The type of comment of the line, or <c>null</c> if the line is not a comment</returns>
		/// <exception cref="UndefinedOperatorException" />
		public readonly CommentType? GetCommentType(ReadOnlySpan<char> line) => GetCommentType(line, out _);

		/// <summary>
		/// Gets the type of comment of a line of RSML, taking into consideration
		/// the parser's properties, or <c>null</c> if the line is not a comment.
		/// </summary>
		/// <param name="line">A line of RSML</param>
		/// <param name="trimmedLine">
		/// Since this method trims the given line, you can get it back to avoid trimming twice,
		/// which would cost performance.
		/// </param>
		/// <returns>The type of comment of the line, or <c>null</c> if the line is not a comment</returns>
		/// <exception cref="UndefinedOperatorException" />
		private readonly CommentType? GetCommentType(ReadOnlySpan<char> line, out ReadOnlySpan<char> trimmedLine)
		{

			if (LanguageStandard.PrimaryOperatorSymbol.Length < 1 || LanguageStandard.SecondaryOperatorSymbol.Length < 1 || LanguageStandard.TertiaryOperatorSymbol.Length < 1)
				throw new UndefinedOperatorException("All operators must be defined at this stage.");

			if (line.IsEmpty || line.IsWhiteSpace() || line.IsNewLinesOnly())
			{

				trimmedLine = []; // empty
				return CommentType.Whitespace;

			}

			line = line.TrimStart();

			if (line[0] == '#')
			{

				trimmedLine = line;
				return CommentType.Explicit;

			}

			if (line[0] != '@'
				&& !line.Contains(LanguageStandard.PrimaryOperatorSymbol, StringComparison.InvariantCulture)
				&& !line.Contains(LanguageStandard.SecondaryOperatorSymbol, StringComparison.InvariantCulture)
				&& !line.Contains(LanguageStandard.TertiaryOperatorSymbol, StringComparison.InvariantCulture))
			{

				trimmedLine = line;
				return CommentType.Implicit;

			}

			trimmedLine = [];
			return null;

		}

		/// <summary>
		/// Registers a special action.
		/// </summary>
		/// <param name="name">The name of the action</param>
		/// <param name="action">THe actual action itself</param>
		public readonly void RegisterSpecialAction(string name, SpecialAction action) => LanguageStandard.SpecialActions[name] = action;

	}

}
