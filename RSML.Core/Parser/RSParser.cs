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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using RSML.Core.Actions;
using RSML.Core.Exceptions;
using RSML.Core.Language;


namespace RSML.Core.Parser
{

	/// <summary>
	/// Represents a parser for <strong>Red Sea Markup Language</strong>.
	/// </summary>
	public sealed class RSParser
	{

		private readonly Dictionary<string, SpecialAction> specialActions = [];
		private List<string> content;

		// Content cache is lazy so we only have to worry about it when
		// the content is accessed via the property with the same name.
		private string? cachedContent = null;
		private bool cacheNeedsRebuild = false;

		/// <summary>
		/// The amount of lines of the content.
		/// </summary>
		public int Lines => content.Count;

		/// <summary>
		/// The RSML content to evaluate.
		/// </summary>
		public string Content
		{

			get
			{

				if (cachedContent is null || cacheNeedsRebuild)
					cachedContent = String.Join('\n', content);

				return cachedContent;

			}
			set
			{

				if (value.Trim() == "")
					throw new ArgumentException("Content consists of nothing but whitespace", nameof(value));

				content = value.Split('\n').ToList();
				InformCacheRebuild();

			}

		}

		/// <summary>
		/// The primary operator's symbol.
		/// </summary>
		public string PrimaryOperatorSymbol { get; set; }

		/// <summary>
		/// The secondary operator's symbol.
		/// </summary>
		public string? SecondaryOperatorSymbol { get; set; }

		/// <summary>
		/// The tertiary operator's symbol.
		/// </summary>
		public string? TertiaryOperatorSymbol { get; set; }

		/// <summary>
		/// The secondary operator's action.
		/// </summary>
		public OperatorAction? SecondaryOperatorAction { get; set; }

		/// <summary>
		/// The tertiary operator's action.
		/// </summary>
		public OperatorAction? TertiaryOperatorAction { get; set; }

		/// <summary>
		/// The special actions.
		/// </summary>
		public ReadOnlyDictionary<string, SpecialAction> SpecialActions => specialActions.AsReadOnly();

		/// <summary>
		/// Initializes a RSML parser with a defined primary operator and some content to
		/// evaluate.
		/// </summary>
		/// <param name="content">The string the parser should evaluate. This can be changed later.</param>
		/// <param name="primaryOperator">The symbol for the primary operator</param>
		public RSParser(string content, string primaryOperator)
		{

			this.content = content.Split('\n').ToList();
			PrimaryOperatorSymbol = primaryOperator;

		}

		/// <summary>
		/// Initializes a RSML parser from a set of properties that include operator
		/// symbols and actions.
		/// </summary>
		/// <param name="content">The string the parser should evaluate. This can be changed later.</param>
		/// <param name="properties">A set of properties for the parser</param>
		public RSParser(string content, ParserProperties properties)
		{

			this.content = content.Split('\n').ToList();

			PrimaryOperatorSymbol = properties.PrimaryOperatorSymbol;
			SecondaryOperatorSymbol = properties.SecondaryOperatorSymbol;
			TertiaryOperatorSymbol = properties.TertiaryOperatorSymbol;

			SecondaryOperatorAction = properties.SecondaryOperatorAction;
			TertiaryOperatorAction = properties.TertiaryOperatorAction;

		}

		/// <summary>
		/// Initializes a RSML parser from a language standard.
		/// </summary>
		/// <param name="content">The string the parser should evaluate. This can be changed later.</param>
		/// <param name="languageStandard">The RSML language standard to use.</param>
		public RSParser(string content, LanguageStandard languageStandard)
		{

			this.content = content.Split('\n').ToList();

			PrimaryOperatorSymbol = languageStandard.Properties.PrimaryOperatorSymbol;
			SecondaryOperatorSymbol = languageStandard.Properties.SecondaryOperatorSymbol;
			TertiaryOperatorSymbol = languageStandard.Properties.TertiaryOperatorSymbol;

			SecondaryOperatorAction = languageStandard.Properties.SecondaryOperatorAction;
			TertiaryOperatorAction = languageStandard.Properties.TertiaryOperatorAction;

			specialActions = languageStandard.SpecialActions;

		}

		/// <summary>
		/// Activates the <see cref="cacheNeedsRebuild" /> flag.
		/// This method does not trigger the rebuild itself as the cache is lazy.
		/// </summary>
		private void InformCacheRebuild() => cacheNeedsRebuild = true;

		/// <summary>
		/// Handles a call to a special action.
		/// </summary>
		/// <param name="line">A line of RSML, containing a special action</param>
		/// <returns></returns>
		/// <exception cref="InvalidRSMLSyntax">A line containing a special action must have at least 2 characters</exception>
		/// <exception cref="UndefinedActionException">Action is undefined but used</exception>
		private byte HandleSpecialActionCall(string line)
		{

			if (line.Length < 2)
				throw new InvalidRSMLSyntax("A line containing a special action must have at least 2 characters");

			string[] splitLine = line[1..].Split(' ');
			string actionName = splitLine[0];
			string actionArgument = splitLine.Length < 2 ? "" : splitLine[1];

			if (!SpecialActions.TryGetValue(actionName, out var action))
				throw new UndefinedActionException("Action is undefined but used");

			return action.Invoke(this, actionArgument);

		}

		/// <summary>
		/// Handles a call to an operator's action.
		/// </summary>
		/// <param name="operatorType">The operator type</param>
		/// <param name="line">A line of RSML containing a logic path</param>
		/// <param name="properties">Evaluation properties</param>
		/// <returns>The operator's right side or <c>null</c> if there was no match</returns>
		/// <exception cref="InvalidRSMLSyntax">
		/// <list type="bullet">A line containing a logic path must be splittable in 2 tokens: LEFT and RIGHT</list>
		/// <list type="bullet">A logic path's return value must be enclosed in double quotes</list>
		/// </exception>
		private string? HandleOperatorAction(OperatorType operatorType, string line, ref EvaluationProperties properties)
		{

			string[] splitLine = line.Split(operatorType == OperatorType.Primary
												? PrimaryOperatorSymbol
												: (operatorType == OperatorType.Secondary)
													? SecondaryOperatorSymbol
													: TertiaryOperatorSymbol
											);

			if (splitLine.Length < 2)
				throw new InvalidRSMLSyntax("A line containing a logic path must be splittable in 2 tokens: LEFT and RIGHT");

			string left = splitLine[0].TrimEnd(); // we had already trimmed the start, remember

			if (left == "any" && properties.ExpandAnyIntoRegularExpression)
				left = ".+";

			// actual evaluation
			if (!Regex.IsMatch(properties.RuntimeIdentifier, $"^{left}$"))
				return null; // no bueno

			// we only trim if necssary
			// we're both avoiding heap allocs
			// but also avoiding somewhat expensive operations
			string right = splitLine[1].Trim(); // but this we gon' have to fully trim

			if (right.Length < 3 || !(right.StartsWith('"') && right.EndsWith('"')))
				throw new InvalidRSMLSyntax("A logic path's return value must be enclosed in double quotes");

			string actualReturnValue = right[1..^1]; // this trims the quotes

			switch (operatorType)
			{

				case OperatorType.Secondary:
					SecondaryOperatorAction!.Invoke(this, actualReturnValue);
					break;

				case OperatorType.Tertiary:
					TertiaryOperatorAction!.Invoke(this, actualReturnValue);
					break;

			}

			return actualReturnValue;

		}

		// todo: the evaluation below me doesn't givew a shit about properties so we gon' have to cook that up
		// ^ VERY IMPORTANT !!!1!1!!1!1 :o
		// todo: also reminder i dont fucking need to overload this - i can overload it in the RSML package afterwards not here
		public EvaluationResult Evaluate(EvaluationProperties properties)
		{

			if (SecondaryOperatorSymbol is null || TertiaryOperatorSymbol is null)
				throw new UndefinedOperatorException("All operators must have a symbol by this point");

			if (SecondaryOperatorAction is null || TertiaryOperatorAction is null)
				throw new UndefinedOperatorException("All operator's actions must be defined at this point");

			StringBuilder builder = new("--START-OF-STREAM--\n");

			foreach (string rawLine in content)
			{

				if (GetCommentType(rawLine, out var line) is not null)
					continue; // this is a comment - null means no comment type because it's not a comment

				if (line is null)
					continue; // this won't happen but it makes .NET happy sooooo

				if (line.StartsWith("@EndAll"))
					return new();

				if (line.StartsWith('@'))
				{

					var result = HandleSpecialActionCall(line);

					switch (result)
					{

						case SpecialActionBehavior.Success:
							break;

						case SpecialActionBehavior.Error:
							// always use 1 for errors, no matter the default case - it may change in the future
							// but this one won't
							_ = builder.AppendLine(
								$"Special action returned error code 1 in line '{line}'."
							);
							break;

						case SpecialActionBehavior.StopEvaluation:
							return new();

						case SpecialActionBehavior.ResetSpecials:
							specialActions.Clear();
							break;

						case SpecialActionBehavior.ResetOperators:
							SecondaryOperatorAction = (_, _) => { };
							TertiaryOperatorAction = (_, _) => { };

							// we are going to reset to official-25's operators
							// todo: document this later maybe i dunno

							var official25 = LanguageStandard.Official25;
							PrimaryOperatorSymbol = official25.Properties.PrimaryOperatorSymbol;
							SecondaryOperatorSymbol = official25.Properties.SecondaryOperatorSymbol;
							TertiaryOperatorSymbol = official25.Properties.TertiaryOperatorSymbol;
							break;

						default:
							// anything else is an error tho it's recommended to use 1 as error
							// as we may add other special behaviors
							_ = builder.AppendLine(
								$"""
								--SECTION--
								Special action returned an error code ({result}) in line '{line}'. If you're the creator of the file,
								please strictly use code 1 for errors in the future.
								--SECTION--
								"""
							);
							break;

					}

					continue;

				}

				if (line.Contains(PrimaryOperatorSymbol))
				{

					var val = HandleOperatorAction(OperatorType.Primary, line, ref properties);


					if (val is not null)
					{

						var builderValue = builder.ToString();
						return new(val, builderValue.Length != 20 ? builderValue : "");
						// 20 is the length of --START-OF-END--\n 
						// yes my lazy ass hardcoded it cry harder

					}

					continue;

				}

				if (line.Contains(SecondaryOperatorSymbol))
					_ = HandleOperatorAction(OperatorType.Secondary, line, ref properties);

				else if (line.Contains(TertiaryOperatorSymbol))
					_ = HandleOperatorAction(OperatorType.Tertiary, line, ref properties);

				continue;

			}

			return new(); // nothing found

		}

		// todo: fix workflows (after major release, it happens)

		/// <summary>
		/// Inserts a line at a 0-based line number (0 = first line).
		/// </summary>
		/// <param name="lineNumber">The line number to insert the line at</param>
		/// <param name="line">The line to insert</param>
		/// <exception cref="ArgumentException">Insertion at a negative line number or insertion of a blank line</exception>
		public void InsertLineAt(int lineNumber, string line)
		{

			if (lineNumber < 0)
				throw new ArgumentException(
					"Insertion before 0 is not supported.",
					nameof(lineNumber)
				);

			if (String.IsNullOrWhiteSpace(line))
				throw new ArgumentException(
					"Insertion of blank lines is not supported.",
					nameof(line)
				);

			line = line.Trim();

			while (content.Count < lineNumber)
				content.Add("# ");

			content.Insert(lineNumber, line);
			InformCacheRebuild();

		}

		/// <summary>
		/// Inserts a line at a (-1)-based line number (-1 = first line), as this method
		/// treats the index as the "relative before", not the actual line.
		/// </summary>
		/// <param name="lineNumber">The line number to insert the line below</param>
		/// <param name="line">The line to insert</param>
		public void InsertLineBefore(int lineNumber, string line) => InsertLineAt(--lineNumber, line);

		/// <summary>
		/// Inserts a line at a 1-based line number (1 = first line), as this method
		/// treats the index as the "relative after", not the actual line.
		/// </summary>
		/// <param name="lineNumber">The line number to insert the line above</param>
		/// <param name="line">The line to insert</param>
		public void InsertLineAfter(int lineNumber, string line) => InsertLineAt(++lineNumber, line);

		/// <summary>
		/// Inserts lines from a dictionary of <strong>Line Number &lt;=&gt; Line</strong>.
		/// </summary>
		/// <param name="lines">The dictionary containing those lines</param>
		/// <remarks>
		/// This method sorts the dictionary that's provided.
		/// If it's already sorted, please use <see cref="InsertLines(SortedDictionary{Int32, String})"/> instead.
		/// </remarks>
		public void InsertLines(Dictionary<int, string> lines)
		{

			foreach (var kvp in lines.OrderBy(kv => kv.Key))
				InsertLineAt(kvp.Key, kvp.Value);

		}

		/// <summary>
		/// Inserts lines from a sorted dictionary of <strong>Line Number &lt;=&gt; Line</strong>.
		/// </summary>
		/// <param name="lines">The sorted dictionary containing those lines</param>
		public void InsertLines(SortedDictionary<int, string> lines)
		{

			foreach (var kvp in lines)
				InsertLineAt(kvp.Key, kvp.Value);

		}

		/// <summary>
		/// Gets the type of comment of a line of RSML, taking into consideration
		/// the parser's properties, or <c>null</c> if the line is not a comment.
		/// </summary>
		/// <param name="line">A line of RSML</param>
		/// <returns>The type of comment of the line, or <c>null</c> if the line is not a comment</returns>
		/// <exception cref="UndefinedOperatorException" />
		public CommentType? GetCommentType(string line)
		{

			if (SecondaryOperatorSymbol is null || TertiaryOperatorSymbol is null)
				throw new UndefinedOperatorException("All operators must be defined at this stage.");

			if (String.IsNullOrWhiteSpace(line))
				return CommentType.Whitespace;

			line = line.TrimStart();

			if (line.StartsWith('#'))
				return CommentType.Explicit;

			if (!line.StartsWith('#')
				&& !line.StartsWith('@')
				&& !line.Contains(PrimaryOperatorSymbol)
				&& !line.Contains(SecondaryOperatorSymbol)
				&& !line.Contains(TertiaryOperatorSymbol))
				return CommentType.Implicit;

			return null;

		}

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
		private CommentType? GetCommentType(string line, out string? trimmedLine)
		{

			if (SecondaryOperatorSymbol is null || TertiaryOperatorSymbol is null)
				throw new UndefinedOperatorException("All operators must be defined at this stage.");

			if (String.IsNullOrWhiteSpace(line))
			{

				trimmedLine = null;
				return CommentType.Whitespace;

			}

			line = line.TrimStart();

			if (line.StartsWith('#'))
			{

				trimmedLine = line;
				return CommentType.Explicit;

			}

			if (!line.StartsWith('#')
				&& !line.StartsWith('@')
				&& !line.Contains(PrimaryOperatorSymbol)
				&& !line.Contains(SecondaryOperatorSymbol)
				&& !line.Contains(TertiaryOperatorSymbol))
			{

				trimmedLine = line;
				return CommentType.Implicit;

			}

			trimmedLine = line;
			return null;

		}

		/// <summary>
		/// Removes explicit comments (ones marked with <c>#</c> at the start) from the
		/// content that should be evaluated. Implicit comments are kept as they are.
		/// </summary>
		/// <returns>
		/// The comments, as <strong>Line Number &lt;=&gt; Line</strong>. They can be
		/// appended back afterwards (see <see cref="InsertLines(Dictionary{Int32, String})" />).
		/// </returns>
		/// <remarks>
		/// This method only improves performance when dealing with comment-heavy content.
		/// Otherwise, it'll actually have a small performance cost.
		/// </remarks>
		public Dictionary<int, string> RemoveExplicitComments()
		{

			Dictionary<int, string> comments = [];

			for (int i = content.Count - 1; i >= 0; i--)
			{

				if (content[i].TrimStart().StartsWith('#'))
				{

					comments[i] = content[i];
					content.RemoveAt(i);

				}

			}

			InformCacheRebuild();
			return comments;

		}

		/// <summary>
		/// Removes implicit non-whitespace comments from the
		/// content that should be evaluated. Explicit and whitespace comments are kept
		/// as they are.
		/// </summary>
		/// <returns>
		/// The comments, as <strong>Line Number &lt;=&gt; Line</strong>. They can be
		/// appended back afterwards (see <see cref="InsertLines(Dictionary{Int32, String})" />).
		/// </returns>
		/// <exception cref="UndefinedOperatorException" />
		/// <remarks>
		/// This method only improves performance when dealing with implicit comment-heavy content.
		/// Otherwise, it'll actually have a small performance cost.
		/// </remarks>
		public Dictionary<int, string> RemoveImplicitNonWhitespaceComments()
		{

			if (SecondaryOperatorSymbol is null || TertiaryOperatorSymbol is null)
				throw new UndefinedOperatorException("All operators must be defined at this stage.");

			Dictionary<int, string> comments = [];

			for (int i = content.Count - 1; i >= 0; i--)
			{

				if (!content[i].TrimStart().StartsWith('#')
					&& !content[i].TrimStart().StartsWith('@')
					&& !content[i].Contains(PrimaryOperatorSymbol)
					&& !content[i].Contains(SecondaryOperatorSymbol)
					&& !content[i].Contains(TertiaryOperatorSymbol))
				{

					comments[i] = content[i];
					content.RemoveAt(i);

				}

			}

			InformCacheRebuild();
			return comments;

		}

		/// <summary>
		/// Removes implicit whitespace comments from the
		/// content that should be evaluated. Explicit and non-whitespace comments are kept
		/// as they are.
		/// </summary>
		/// <returns>
		/// The comments, as <strong>Line Number &lt;=&gt; Line</strong>. They can be
		/// appended back afterwards (see <see cref="InsertLines(Dictionary{Int32, String})" />).
		/// </returns>
		/// <exception cref="UndefinedOperatorException" />
		/// <remarks>
		/// This method only improves performance when dealing with content that contains a lot of blank
		/// lines. Otherwise, it'll actually have a small performance cost.
		/// </remarks>
		public Dictionary<int, string> RemoveImplicitWhitespaceComments()
		{

			Dictionary<int, string> comments = [];

			for (int i = content.Count - 1; i >= 0; i--)
			{

				if (String.IsNullOrWhiteSpace(content[i]))
				{

					comments[i] = content[i];
					content.RemoveAt(i);

				}

			}

			InformCacheRebuild();
			return comments;

		}

		/// <summary>
		/// Formats implicit comments as explicit ones.
		/// </summary>
		/// <param name="applyToWhitespace">
		/// Whether to also apply this to whitespace comments (blank lines)
		/// </param>
		public void FormatImplicitCommentsAsExplicit(bool applyToWhitespace)
		{

			for (int i = content.Count - 1; i >= 0; i--)
			{

				var commentType = GetCommentType(content[i]);

				if (commentType == CommentType.Implicit)
				{

					content[i] = $"# {content[i]}";
					continue;

				}

				if (commentType == CommentType.Whitespace && applyToWhitespace)
					content[i] = $"# {content[i]}";

			}

			InformCacheRebuild();

		}

		/// <summary>
		/// Reverses the logic order of the parser (this is done by reversing
		/// the logic order of the content).
		/// </summary>
		/// <remarks>
		/// Since this quite literally reverses the order of the logic paths,
		/// not the order at which the parser reads them, the operation is expensive and
		/// should only be used if necessary. It's likely for a future update to include a way to
		/// actually start evaluating at the bottom, which would be more performant
		/// than this.
		/// </remarks>
		public void ReverseLogicOrder()
		{

			content.Reverse();
			InformCacheRebuild();

		}

		/// <summary>
		/// Registers a special action.
		/// </summary>
		/// <param name="name">The name of the action</param>
		/// <param name="action">THe actual action itself</param>
		public void RegisterSpecialAction(string name, SpecialAction action) => specialActions[name] = action;

	}

}
