using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using RSML.Exceptions;


namespace RSML.Parser
{

	public class RSParser
	{

		protected Action<RSParser>? secondaryAction;
		protected Action<RSParser>? tertiaryAction;

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
		/// <param name="nameOfSpecial">The name of the special function</param>
		/// <param name="specialAction">A method that returns a 8 bit unsigned integer and 2 parameters of type <see cref="RSParser" /> and <see cref="String" />, respectively.</param>
		public void RegisterSpecialFunction(string nameOfSpecial, Func<RSParser, string, byte> specialAction) => specials.Add(nameOfSpecial, specialAction);

		/// <summary>
		/// Register a main action.
		/// Can either be secondary or tertiary.
		/// </summary>
		/// <param name="isTertiary">If false (default), set it as secondary instead of tertiary.</param>
		/// <param name="action">The action to run</param>
		public void RegisterAction(Action<RSParser> action, bool isTertiary = false)
		{

			if (isTertiary)
			{

				tertiaryAction = action;

			}
			else
			{

				secondaryAction = action;

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
		/// 
		/// </summary>
		/// <param name="line"></param>
		/// <exception cref="UndefinedSpecialException"></exception>
		protected byte HandleSpecialFunctionCall(string line)
		{

			string[] splitLine = line[1..].Split(' ');
			string functionName = splitLine[0];
			string argument = splitLine[1];

			return !(specials.ContainsKey(functionName))
				? throw new UndefinedSpecialException($"Special @{functionName} is undefined.")
				: specials[functionName](this, argument);

		}

		protected string? HandlePrimaryAction(string line)
		{

			string[] splitLine = line.Split(primaryActionDelimiter);
			int pos = 0;

			foreach (string token in splitLine)
			{

				string trimmedToken = token.Trim();

				if (pos == 0)
				{

					if (!Regex.IsMatch(RuntimeInformation.RuntimeIdentifier, trimmedToken)) return null;

				}

				// todo: cook ze rest

				++pos;

			}

		}

		/// <summary>
		/// Evaluate RSML.
		/// </summary>
		/// <param name="linesepChar">The line separation string to use (defaults to system line separation)</param>
		public string? EvaluateRSML(string? linesepChar = null)
		{

			// todo: cook dis

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

					HandlePrimaryAction(line);

				}
				else if (line.Contains(secondaryActionDelimiter))
				{

					HandleSecondaryAction(line);

				}
				else if (line.Contains(tertiaryActionDelimiter))
				{

					HandleTertiaryAction(line);

				}
				else continue;

			}

		}

	}

}
