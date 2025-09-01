using System;
using System.Collections.Generic;

using OceanApocalypseStudios.RSML.Actions;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Evaluation;
using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Performance.Value;
using OceanApocalypseStudios.RSML.Toolchain.Compliance;

using LocalMachine = OceanApocalypseStudios.RSML.Machine.LocalMachine;


namespace OceanApocalypseStudios.RSML.Performance.Stateless
{

	/// <summary>
	/// A stateless RSML evaluator.
	/// </summary>
	public static class StatelessEvaluator
	{

		/// <summary>
		/// The level of compliance, per feature, with the official language standard for the current API version.
		/// </summary>
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull("2.0.0");

		/// <summary>
		/// The evaluator's special action.
		/// </summary>
		public static Dictionary<string, SpecialAction> SpecialActions { get; } = [ ];

		/// <summary>
		/// Evaluates a RSML buffer.
		/// </summary>
		/// <param name="buffer">The buffer</param>
		/// <param name="machineData">The machine to use</param>
		/// <returns>An evaluation result</returns>
		/// <exception cref="ActionStandardErrorException">An action threw an error code 1</exception>
		/// <exception cref="ActionErrorException">An action threw a non-1 error code OR a throw-error operator was used</exception>
		/// <exception cref="InvalidRsmlSyntax">The RSML syntax is invalid</exception>
		public static EvaluationResult Evaluate(ReadOnlySpan<char> buffer, LocalMachine machineData)
		{

			int curIndex = 0;

			while (StatelessReader.TryTokenizeNextLine(buffer, curIndex, out var line, out curIndex))
			{

				var tokens = OptimizedNormalizer.NormalizeLine(line, out _);
				OptimizedValidator.ValidateLine(tokens); // line validation

				if (tokens[tokens.Last()].Kind == TokenKind.Eol)
					tokens.Remove(tokens.Last()); // removes last entry

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
					case 2:
						continue;

					case 3:
						if (tokens[1].Value.IsEquals("EndAll"))
							continue;

						byte result = HandleSpecialActionCall(tokens[1].Value.ToString(), tokens[2].Value.ToString());

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

							return tokens[0].Kind == TokenKind.ThrowErrorOperator
									   ? throw new ActionErrorException("Error-throw operator used.")
									   : new(tokens[4].Value.ToString());

						}

						continue;

					case 6:
						if (HandleLogicPath_Complex(tokens, machineData, machineData.SystemName == "linux"))
						{

							return tokens[0].Kind == TokenKind.ThrowErrorOperator
									   ? throw new ActionErrorException("Error-throw operator used.")
									   : new(tokens[5].Value.ToString());

						}

						continue;

					default:
						if (tokens[0].Kind == TokenKind.CommentSymbol)
							continue; // it's somehow a comment

						throw new InvalidRsmlSyntax("Unexpected error: invalid line tokenized successfully.");

				}

			}

			return new(); // no matches

		}

		private static byte HandleSpecialActionCall(string name, string arg)
		{

			if (name == "EndAll")
				return SpecialActionBehavior.StopEvaluation;

			return SpecialActions.TryGetValue(name, out var action)
					   ? action(null, arg)
					   : throw new UndefinedActionException("Action is undefined but used");

		}

		private static bool HandleLogicPath_Simple(SyntaxLine tokens, in LocalMachine machine, bool isLinux)
		{

			if (isLinux)
				return HandleLogicPath_Simple_Linux(tokens, machine);

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.SystemName is not null,
				_                         => tokens[1].Value.IsEquals(machine.SystemName)

			};

			bool systemVersionMatches = tokens[2].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.SystemVersion != -1,
				_                         => tokens[2].Value.IsEquals(machine.StringifiedSystemVersion)

			};

			bool architectureMatches = tokens[3].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                         => tokens[3].Value.IsEquals(machine.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Complex(SyntaxLine tokens, in LocalMachine machine, bool isLinux)
		{

			if (isLinux)
				return HandleLogicPath_Complex_Linux(tokens, machine);

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.SystemName is not null,
				_                         => tokens[1].Value.IsEquals(machine.SystemName)

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[2].Kind)
			{

				case TokenKind.Equals:
					systemVersionMatches = tokens[3].Value.IsEquals(machine.StringifiedSystemVersion);

					break;

				case TokenKind.Different:
					systemVersionMatches = !tokens[3].Value.IsEquals(machine.StringifiedSystemVersion);

					break;

				case TokenKind.GreaterOrEqualsThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion >= versionNum;

					break;

				case TokenKind.LessOrEqualsThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion <= versionNum;

					break;

				case TokenKind.GreaterThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion > versionNum;

					break;

				case TokenKind.LessThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion < versionNum;

					break;

				default:
					systemVersionMatches = false;

					break;

			}

			bool architectureMatches = tokens[4].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                         => tokens[3].Value.IsEquals(machine.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Simple_Linux(SyntaxLine tokens, in LocalMachine machine)
		{

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.DistroName is not null,
				_ => tokens[1].Value.IsEquals(machine.SystemName) ||
					 tokens[1].Value.IsEquals(machine.DistroName) ||
					 tokens[1].Value.IsEquals(machine.DistroFamily)

			};

			bool systemVersionMatches = tokens[2].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.SystemVersion != -1,
				_                         => tokens[2].Value.IsEquals(machine.StringifiedSystemVersion)

			};

			bool architectureMatches = tokens[3].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                         => tokens[3].Value.IsEquals(machine.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

		private static bool HandleLogicPath_Complex_Linux(SyntaxLine tokens, in LocalMachine machine)
		{

			bool systemNameMatches = tokens[1].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.DistroName is not null,
				_ => tokens[1].Value.IsEquals(machine.SystemName) ||
					 tokens[1].Value.IsEquals(machine.DistroName) ||
					 tokens[1].Value.IsEquals(machine.DistroFamily)

			};

			bool systemVersionMatches = false;
			int versionNum;

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (tokens[2].Kind)
			{

				case TokenKind.Equals:
					systemVersionMatches = tokens[3].Value.IsEquals(machine.StringifiedSystemVersion);

					break;

				case TokenKind.Different:
					systemVersionMatches = !tokens[3].Value.IsEquals(machine.StringifiedSystemVersion);

					break;

				case TokenKind.GreaterOrEqualsThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion >= versionNum;

					break;

				case TokenKind.LessOrEqualsThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion <= versionNum;

					break;

				case TokenKind.GreaterThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion > versionNum;

					break;

				case TokenKind.LessThan:
					if (Int32.TryParse(tokens[3].Value, out versionNum))
						systemVersionMatches = machine.SystemVersion < versionNum;

					break;

				default:
					systemVersionMatches = false;

					break;

			}


			bool architectureMatches = tokens[4].Kind switch
			{

				TokenKind.WildcardKeyword => true,
				TokenKind.DefinedKeyword  => machine.ProcessorArchitecture is not null,
				_                         => tokens[3].Value.IsEquals(machine.ProcessorArchitecture)

			};

			return systemNameMatches && systemVersionMatches && architectureMatches;

		}

	}

}
