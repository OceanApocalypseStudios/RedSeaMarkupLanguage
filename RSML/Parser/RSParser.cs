using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using RSML.Exceptions;


namespace RSML.Parser
{

	/// <summary>
	/// The default parser for Red Sea Markup Language.
	/// </summary>
	public class RSParser
	{

		#region Actions

		/// <summary>
		/// The secondary action.
		/// </summary>
		protected Action<RSParser, string>? secondaryAction;

		/// <summary>
		/// The tertiary action.
		/// </summary>
		protected Action<RSParser, string>? tertiaryAction;

		/// <summary>
		/// A dictionary containing all of the special actions.
		/// </summary>
		protected Dictionary<string, Func<RSParser, string, byte>> specials = new();

		#endregion

		#region Operators

		/// <summary>
		/// The most important delimiter.
		/// Is used in primary actions.
		/// </summary>
		protected string primaryActionDelimiter = "->";

		/// <summary>
		/// Delimiter used in secondary actions.
		/// </summary>
		protected string secondaryActionDelimiter = "||";

		/// <summary>
		/// Delimiter used in tertiary actions.
		/// </summary>
		protected string tertiaryActionDelimiter = "^!";

		#endregion

		#region Other Fields

		/// <summary>
		/// The data that's being parsed.
		/// </summary>
		protected string content;

		/// <summary>
		/// Whether or not a multiline comment is currently being evaluated.
		/// </summary>
		protected bool insideMultiLineComment;

		#endregion

		#region Constructors

		/// <summary>
		/// Create a new parser instance from a RSML string.
		/// </summary>
		/// <param name="rsmlContents">The RSML</param>
		public RSParser(string rsmlContents)
		{

			content = rsmlContents;

		}

		/// <summary>
		/// Create a new parser instance from a RSML StringReader.
		/// </summary>
		/// <param name="rsmlReader">The RSML</param>
		public RSParser(StringReader rsmlReader)
		{

			content = rsmlReader.ReadToEnd();

		}

		/// <summary>
		/// Creates a new parser from another one's data.
		/// </summary>
		/// <param name="parser">The parser to clone</param>
		public RSParser(RSParser parser)
		{

			content = parser.ToString();

		}

		/// <summary>
		/// Creates a new parser from a document, but does not
		/// make changes to the document or to the document's linked
		/// parser.
		/// </summary>
		/// <param name="document">The document whose RSML to load</param>
		public RSParser(RSDocument document)
		{

			content = document.ToString();

		}

		#endregion

		#region Register Actions

		/// <summary>
		/// Register a new special function.
		/// </summary>
		/// <param name="nameOfSpecial">The name of the special function (without the <strong>@</strong>)</param>
		/// <param name="specialAction">A method that returns a 8 bit unsigned integer and 2 parameters of type <see cref="RSParser" /> and <see cref="String" />, respectively.</param>
		public void RegisterSpecialFunction(string nameOfSpecial, Func<RSParser, string, byte> specialAction) => specials.Add(nameOfSpecial, specialAction);

		/// <summary>
		/// Register a main action.
		/// Can either be secondary or tertiary.
		/// </summary>
		/// <param name="action">The action to run</param>
		/// <param name="operatorType">The operator to redefine</param>
		public void RegisterAction(OperatorType operatorType, Action<RSParser, string> action)
		{

			switch (operatorType)
			{

				case OperatorType.Secondary:
					secondaryAction = action;
					break;

				case OperatorType.Tertiary:
					tertiaryAction = action;
					break;

				default:
					throw new ImmutableActionException("The primary action is immutable and cannot be redefined whatsoever.");

			}

		}

		#endregion

		/// <summary>
		/// Define a operator to match a specified string.
		/// </summary>
		/// <param name="operatorType">The operator to re-define</param>
		/// <param name="newOperator">The new value to give the operator</param>
		public void DefineOperator(OperatorType operatorType, string newOperator)
		{

			switch (operatorType)
			{

				case OperatorType.Secondary:
					secondaryActionDelimiter = newOperator;
					break;

				case OperatorType.Tertiary:
					tertiaryActionDelimiter = newOperator;
					break;

				default:
					primaryActionDelimiter = newOperator;
					break;

			}

		}

		#region Handlers

		/// <summary>
		/// Handles a call to a special action/function.
		/// </summary>
		/// <param name="line">The line where the function is called</param>
		/// <exception cref="UndefinedSpecialException">The special is undefined</exception>
		protected byte HandleSpecialFunctionCall(string line)
		{

			string[] splitLine = line[1..].Split(' ');
			string functionName = splitLine[0];
			string argument = splitLine[1];

			return !(specials.TryGetValue(functionName, out var value))
				? throw new UndefinedSpecialException($"Special @{functionName} is undefined.")
				: value(this, argument);

		}

		/// <summary>
		/// Handles a RSML action.
		/// </summary>
		/// <param name="line">The RSML line</param>
		/// <param name="operatorType">The operator (primary, secondary or tertiary - all of them must be defined)</param>
		/// <returns>Null if there was no match or the return value/argument as a string (even if the action was not primary).</returns>
		/// <exception cref="UndefinedActionException">At least one action is undefined.</exception>
		protected string? HandleAction(string line, OperatorType operatorType = OperatorType.Primary) => HandleAction(line, operatorType, RuntimeInformation.RuntimeIdentifier);

		/// <summary>
		/// Handles a RSML action, using a custom RID.
		/// </summary>
		/// <param name="line">The RSML line</param>
		/// <param name="operatorType">The operator (primary, secondary or tertiary - all of them must be defined)</param>
		/// <param name="customRid">A custom RID to use as system identifier</param>
		/// <returns>Null if there was no match or the return value/argument as a string (even if the action was not primary).</returns>
		/// <exception cref="UndefinedActionException">At least one action is undefined.</exception>
		protected string? HandleAction(string line, OperatorType operatorType, string customRid) => HandleAction(line, operatorType, false, customRid);

		/// <summary>
		/// Handles a RSML action, using a custom RID.
		/// </summary>
		/// <param name="line">The RSML line</param>
		/// <param name="operatorType">The operator (primary, secondary or tertiary - all of them must be defined)</param>
		/// <param name="expandAny">Whether to expand the <strong>any</strong> RID or not</param>
		/// <param name="customRid">A custom RID to use as system identifier. If null, uses actual system RID</param>
		/// <returns>Null if there was no match or the return value/argument as a string (even if the action was not primary).</returns>
		/// <exception cref="UndefinedActionException">At least one action is undefined.</exception>
		protected string? HandleAction(string line, OperatorType operatorType, bool expandAny, string? customRid = null)
		{

			// actions are null
			if (secondaryAction is null || tertiaryAction is null)
			{

				throw new UndefinedActionException("All actions must be defined for them to be handled, even if only primary ones are used.");

			}

			customRid ??= RuntimeInformation.RuntimeIdentifier;

			// split the line
			string[] splitLine = line.Split(operatorType == OperatorType.Primary ?
											primaryActionDelimiter : (operatorType == OperatorType.Secondary) ?
											secondaryActionDelimiter : tertiaryActionDelimiter);

			// not enough tokens, fuckersss
			if (splitLine.Length < 2) return null; // ignore it like a comment

			// get system name
			string systemName = splitLine[0].Trim();

			// any is a valid RID, btw
			if (systemName == "any" && expandAny) systemName = ".+";

			// quick evaluation
			if (!Regex.IsMatch(customRid, $"^{systemName}$")) return null;

			string returnValue = splitLine[1].Trim();

			if (returnValue.Length < 3 || !(returnValue.StartsWith('"') && returnValue.EndsWith('"'))) // the quotes and the characters inside of it
			{

				return null; // commenttttttttt

			}

			string trimmedArgument = returnValue[1..^1];

			switch (operatorType)
			{

				case OperatorType.Secondary:
					secondaryAction.Invoke(this, trimmedArgument);
					break;

				case OperatorType.Tertiary:
					tertiaryAction.Invoke(this, trimmedArgument);
					break;

			}

			return trimmedArgument; // ignore the quotes

		}

		#endregion

		#region Evaluation

		/// <summary>
		/// Evaluate RSML.
		/// </summary>
		/// <param name="linesepChar">The line separation string to use (defaults to system line separation)</param>
		/// <returns>The evaluated result (only for primary action; if there's a secondary/tertiary match, it's ignored) or null (no primary matches)</returns>
		public string? EvaluateRSML(string? linesepChar = null) => EvaluateRSMLWithCustomRid(RuntimeInformation.RuntimeIdentifier, linesepChar);

		/// <summary>
		/// Evaluate RSML.
		/// </summary>
		/// <param name="expandAny">Whether to expand the "any" RID or not</param>
		/// <param name="linesepChar">The line separation string to use (defaults to system line separation)</param>
		/// <returns>The evaluated result (only for primary action; if there's a secondary/tertiary match, it's ignored) or null (no primary matches)</returns>
		public string? EvaluateRSML(bool expandAny, string? linesepChar = null) => EvaluateRSMLWithCustomRid(RuntimeInformation.RuntimeIdentifier, expandAny, linesepChar);

		/// <summary>
		/// Evaluate RSML.
		/// </summary>
		/// <param name="customRid">A custom RID to check against</param>
		/// <param name="linesepChar">The line separation string to use (defaults to system line separation)</param>
		/// <returns>The evaluated result (only for primary action; if there's a secondary/tertiary match, it's ignored) or null (no primary matches)</returns>
		public string? EvaluateRSMLWithCustomRid(string customRid, string? linesepChar = null) => EvaluateRSMLWithCustomRid(customRid, false, linesepChar);

		/// <summary>
		/// Evaluate RSML.
		/// </summary>
		/// <param name="customRid">A custom RID to check against</param>
		/// <param name="expandAny">Whether to expand <strong>any</strong> or not</param>
		/// <param name="linesepChar">The line separation string to use (defaults to system line separation)</param>
		/// <returns>The evaluated result (only for primary action; if there's a secondary/tertiary match, it's ignored) or null (no primary matches)</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "<Pending>")]
		public string? EvaluateRSMLWithCustomRid(string customRid, bool expandAny, string? linesepChar = null)
		{

			foreach (string line in content.Split(linesepChar ?? Environment.NewLine))
			{

				if (line.StartsWith("@EndAll")) return null;
				if (line.StartsWith('#')) continue;
				if (line.StartsWith('@'))
				{

					byte result = HandleSpecialFunctionCall(line);

					switch (result)
					{

						// kill code
						case 250:
							return null;

						// remove all specials* (does not remove @EndAll)
						case 251:
							specials = new();
							break;

						// reset action operators
						case 252:
							primaryActionDelimiter = "->";
							secondaryActionDelimiter = "||";
							tertiaryActionDelimiter = "^!";
							break;

					}

				}
				else if (line.Contains(primaryActionDelimiter))
				{

					string? actionReturnValue = HandleAction(line, OperatorType.Primary, expandAny, customRid);

					if (actionReturnValue is not null)
					{

						return actionReturnValue;

					}

				}

				else if (line.Contains(secondaryActionDelimiter))
				{

					HandleAction(line, OperatorType.Secondary, expandAny, customRid);

				}

				else if (line.Contains(tertiaryActionDelimiter))
				{

					HandleAction(line, OperatorType.Tertiary, expandAny, customRid);

				}

				else continue;

			}

			return null; // no matches

		}

		#endregion

		/// <summary>
		/// Gets the underlying RSML data with no evaluation.
		/// </summary>
		/// <returns>RSML data</returns>
		public override string ToString() => content;

	}

}
