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
using System.Runtime.InteropServices;

using RSML.Actions;
using RSML.Exceptions;


namespace RSML.Parser
{

	/// <summary>
	/// Represents a parser for <strong>Red Sea Markup Language</strong>.
	/// </summary>
	public class RsParser
	{

		private const string returnOp1 = "return";
		private const string returnOp2 = "returnif";
		private const string returnOp3 = "->";
		private const string returnOp4 = "=>";

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
		public EvaluationResult Evaluate(string rid) => Evaluate(new EvaluationProperties(rid, false));

		/// <summary>
		/// Evaluates the RSML document given a custom RID and an <c>expandAny</c> flag.
		/// </summary>
		/// <param name="rid">The RID to use</param>
		/// <param name="expandAny"><c>true</c> if <c>any</c> should be expanded into <c>.+</c></param>
		/// <returns>A result</returns>
		public EvaluationResult Evaluate(string rid, bool expandAny) => Evaluate(new EvaluationProperties(rid, expandAny));

		/// <summary>
		/// Evaluates the RSML document given an <c>expandAny</c> flag and a strictness level.
		/// The RID will be the machine's RID.
		/// </summary>
		/// <param name="expandAny"><c>true</c> if <c>any</c> should be expanded into <c>.+</c></param>
		/// <returns>A result</returns>
		public EvaluationResult Evaluate(bool expandAny) => Evaluate(new EvaluationProperties(RuntimeInformation.RuntimeIdentifier, expandAny));

		/// <summary>
		/// Evaluates a RSML document given some properties.
		/// </summary>
		/// <param name="properties">The set of properties to feed into the evaluation</param>
		/// <returns>An evaluation result</returns>
		/// <exception cref="UndefinedOperatorException">All operators and their actions must be defined</exception>
		public EvaluationResult Evaluate(EvaluationProperties properties)
		{

			// todo: code this
			throw new NotImplementedException("This method is being worked on");

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
