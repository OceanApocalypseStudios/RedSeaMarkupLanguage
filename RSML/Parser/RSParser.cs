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

		/// <summary>
		/// The data that's being parsed.
		/// </summary>
		protected string content;

		/// <summary>
		/// Whether or not a multiline comment is currently being evaluated.
		/// </summary>
		protected bool insideMultiLineComment;

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
		protected string? HandleAction(string line, OperatorType operatorType = OperatorType.Primary)
		{

			if (secondaryAction is null || tertiaryAction is null)
			{

				throw new UndefinedActionException("All actions must be defined for them to be handled, even if only primary ones are used.");

			}

			// split the line
			string[] splitLine = line.Split(operatorType == OperatorType.Primary ?
											primaryActionDelimiter : (operatorType == OperatorType.Secondary) ?
											secondaryActionDelimiter : tertiaryActionDelimiter);

			// not enough tokens, fuckersss
			if (splitLine.Length < 2) return null; // ignore it like a comment

			// get system name
			string systemName = splitLine[0].Trim();

			// quick evaluation
			if (!Regex.IsMatch(RuntimeInformation.RuntimeIdentifier, $"^{systemName}$")) return null;

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

		/// <summary>
		/// Evaluate RSML.
		/// </summary>
		/// <param name="linesepChar">The line separation string to use (defaults to system line separation)</param>
		/// <returns>The evaluated result (only for primary action; if there's a secondary/tertiary match, it's ignored) or null (no primary matches)</returns>
		public string? EvaluateRSML(string? linesepChar = null)
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

					string? actionReturnValue = HandleAction(line);

					if (actionReturnValue is not null)
					{

						return actionReturnValue;

					}

				}
#pragma warning disable IDE0058 // expression value unused
				else if (line.Contains(secondaryActionDelimiter))
				{

					HandleAction(line, OperatorType.Secondary);

				}
				else if (line.Contains(tertiaryActionDelimiter))
				{

					HandleAction(line, OperatorType.Tertiary);

				}
#pragma warning restore
				else continue;

			}

			return null; // no matches

		}

	}

}
