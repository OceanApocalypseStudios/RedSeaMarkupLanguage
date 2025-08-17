using System;
using System.Linq;

using RSML.Analyzer.Semantics;
using RSML.Analyzer.Syntax;
using RSML.Exceptions;
using RSML.Toolchain.Compliance;


namespace RSML.Performance.Value
{

	/// <summary>
	/// An optimized stateless RSML semantic validator.
	/// </summary>
	public static class OptimizedValidator
	{

		/// <summary>
		/// The level of compliance, per feature, with the official language standard for the current API version.
		/// </summary>
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull("2.0.0");

		/// <summary>
		/// Validates a line of RSML.
		/// </summary>
		/// <param name="line">The line</param>
		/// <exception cref="InvalidRsmlSyntax">The line contains invalid syntax</exception>
		public static void ValidateLine(SyntaxLine line)
		{

			if (line.Length == 0)
				throw new InvalidRsmlSyntax("Empty token sequence.");

			if (line.Length != 1 && line[line.Last()].Kind == TokenKind.Eol)
				line.Remove(line.Last()); // removes last

			switch (line[0].Kind)
			{

				case TokenKind.Eol or TokenKind.Eof:
					return; // we're done here

				case TokenKind.CommentSymbol when line.Length != 2:
					throw new InvalidRsmlSyntax(
						"A comment must be 2 tokens long."
					); // even if you have a comment with no text not even spaces, you'll have 2 tokens

				case TokenKind.CommentSymbol when !line[0].Value.IsEquals("#"):
					throw new InvalidRsmlSyntax($"Expected CommentSymbol of value '#', but received {line[0].Value} instead.");

				case TokenKind.CommentSymbol when line[1].Kind != TokenKind.CommentText:
					throw new InvalidRsmlSyntax($"Expected CommentText, but received {line[1].Kind} instead.");

				case TokenKind.CommentSymbol:
					return;

				case TokenKind.SpecialActionSymbol when line.Length != 3:
					throw new InvalidRsmlSyntax("A special action must be 3 tokens long."); // even with no arg, you'll have 3 tokens

				case TokenKind.SpecialActionSymbol when !line[0].Value.IsEquals("@"):
					throw new InvalidRsmlSyntax($"Expected SpecialActionSymbol of value '@', but received {line[0].Value} instead.");

				case TokenKind.SpecialActionSymbol when line[1].Kind != TokenKind.SpecialActionName:
					throw new InvalidRsmlSyntax($"Expected SpecialActionName, but received {line[1].Kind} instead.");

				case TokenKind.SpecialActionSymbol when line[2].Kind != TokenKind.SpecialActionArgument:
					throw new InvalidRsmlSyntax($"Expected SpecialActionArgument, but received {line[2].Kind} instead.");

				case TokenKind.SpecialActionSymbol:
					return;

			}

			if (line[0].Kind is not (TokenKind.ReturnOperator or TokenKind.ThrowErrorOperator))
				throw new InvalidRsmlSyntax("Unrecognized start-of-line token.");

			if (!line[0].Value.IsEquals("!>") && !line[0].Value.IsEquals("->"))
				throw new InvalidRsmlSyntax("Operator must be one of !> or ->.");

			switch (line.Length)
			{

				case 2:
					if (line[1].Kind != TokenKind.LogicPathValue)
						throw new InvalidRsmlSyntax("A 2 token long logic path must be a *Operator + LogicPathValue overload.");

					return;

				case 3:
					if ((line[1].Kind != TokenKind.SystemName &&
						 line[1].Kind != TokenKind.DefinedKeyword &&
						 line[1].Kind != TokenKind.WildcardKeyword) ||
						line[2].Kind != TokenKind.LogicPathValue)
						throw new InvalidRsmlSyntax("A 3 token long logic path must be a *Operator + SystemName + LogicPathValue overload.");

					string sysName1 = line[1].Value.ToString();

					if (!line[1].Value.IsEquals("any") && line[1].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (!line[1].Value.IsEquals("defined") && line[1].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (!Validator.ValidSystems.Any(s => sysName1.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						line[1].Kind == TokenKind.SystemName)
						throw new InvalidRsmlSyntax("Invalid system name as of v2.0.0.");

					return;

				case 4:
					if ((line[1].Kind != TokenKind.SystemName &&
						 line[1].Kind != TokenKind.DefinedKeyword &&
						 line[1].Kind != TokenKind.WildcardKeyword) ||
						(line[2].Kind != TokenKind.ArchitectureIdentifier &&
						 line[2].Kind != TokenKind.DefinedKeyword &&
						 line[2].Kind != TokenKind.WildcardKeyword) ||
						line[3].Kind != TokenKind.LogicPathValue)
					{
						throw new InvalidRsmlSyntax(
							"A 4 token long logic path must be a *Operator + SystemName + ArchitectureIdentifier + LogicPathValue overload."
						);
					}

					string sysName2 = line[1].Value.ToString();
					string archName1 = line[2].Value.ToString();

					if (!line[1].Value.IsEquals("any") && line[1].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (!line[1].Value.IsEquals("defined") && line[1].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (!line[2].Value.IsEquals("any") && line[2].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (!line[2].Value.IsEquals("defined") && line[2].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (!Validator.ValidSystems.Any(s => sysName2.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						line[1].Kind == TokenKind.SystemName)
						throw new InvalidRsmlSyntax("Invalid system name as of v2.0.0.");

					if (!Validator.ValidArchitectures.Any(s => archName1.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						line[2].Kind == TokenKind.ArchitectureIdentifier)
						throw new InvalidRsmlSyntax("Invalid architecture identifier as of v2.0.0.");

					return;

				case 5:
					if ((line[1].Kind != TokenKind.SystemName &&
						 line[1].Kind != TokenKind.DefinedKeyword &&
						 line[1].Kind != TokenKind.WildcardKeyword) ||
						(line[2].Kind != TokenKind.MajorVersionId &&
						 line[2].Kind != TokenKind.DefinedKeyword &&
						 line[2].Kind != TokenKind.WildcardKeyword) ||
						(line[3].Kind != TokenKind.ArchitectureIdentifier &&
						 line[3].Kind != TokenKind.DefinedKeyword &&
						 line[3].Kind != TokenKind.WildcardKeyword) ||
						line[4].Kind != TokenKind.LogicPathValue)
					{
						throw new InvalidRsmlSyntax(
							"A 5 token long logic path must be a *Operator + SystemName + MajorVersionId + ArchitectureIdentifier + LogicPathValue overload."
						);
					}

					string sysName3 = line[1].Value.ToString();
					string major1 = line[2].Value.ToString();
					string archName2 = line[3].Value.ToString();

					if (!line[1].Value.IsEquals("any") && line[1].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (!line[1].Value.IsEquals("defined") && line[1].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (!line[2].Value.IsEquals("any") && line[2].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (!line[2].Value.IsEquals("defined") && line[2].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (!line[3].Value.IsEquals("any") && line[3].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (!line[3].Value.IsEquals("defined") && line[3].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (!Validator.ValidSystems.Any(s => sysName3.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						line[1].Kind == TokenKind.SystemName)
						throw new InvalidRsmlSyntax($"Invalid system name ({sysName3}) as of v2.0.0.");

					if (!Validator.ValidArchitectures.Any(s => archName2.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						line[3].Kind == TokenKind.ArchitectureIdentifier)
						throw new InvalidRsmlSyntax("Invalid architecture identifier as of v2.0.0.");

					if (!Int32.TryParse(major1, out _) && line[2].Kind == TokenKind.MajorVersionId)
						throw new InvalidRsmlSyntax("The major version must be a valid integer");

					return;

				case 6:
					if ((line[1].Kind != TokenKind.SystemName &&
						 line[1].Kind != TokenKind.DefinedKeyword &&
						 line[1].Kind != TokenKind.WildcardKeyword) ||
						(line[2].Kind != TokenKind.Equals &&
						 line[2].Kind != TokenKind.Different &&
						 line[2].Kind != TokenKind.GreaterThan &&
						 line[2].Kind != TokenKind.LessThan &&
						 line[2].Kind != TokenKind.GreaterOrEqualsThan &&
						 line[2].Kind != TokenKind.LessOrEqualsThan) ||
						(line[3].Kind != TokenKind.MajorVersionId &&
						 line[3].Kind != TokenKind.DefinedKeyword &&
						 line[3].Kind != TokenKind.WildcardKeyword) ||
						(line[4].Kind != TokenKind.ArchitectureIdentifier &&
						 line[4].Kind != TokenKind.DefinedKeyword &&
						 line[4].Kind != TokenKind.WildcardKeyword) ||
						line[5].Kind != TokenKind.LogicPathValue)
					{
						throw new InvalidRsmlSyntax(
							"A 5 token long logic path must be a *Operator + SystemName + |Comparator| + MajorVersionId + ArchitectureIdentifier + LogicPathValue overload."
						);
					}

					string sysName4 = line[1].Value.ToString();
					string comp = line[2].Value.ToString();
					string major2 = line[3].Value.ToString();
					string archName3 = line[4].Value.ToString();

					if (!line[1].Value.IsEquals("any") && line[1].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (!line[1].Value.IsEquals("defined") && line[1].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (!line[4].Value.IsEquals("any") && line[4].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (!line[4].Value.IsEquals("defined") && line[4].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (!Validator.ValidSystems.Any(s => sysName4.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						line[1].Kind == TokenKind.SystemName)
						throw new InvalidRsmlSyntax("Invalid system name as of v2.0.0.");

					if (!Validator.ValidArchitectures.Any(s => archName3.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						line[1].Kind == TokenKind.ArchitectureIdentifier)
						throw new InvalidRsmlSyntax("Invalid architecture identifier as of v2.0.0.");

					if (!Validator.ValidComparators.Any(s => comp.Equals(s, StringComparison.OrdinalIgnoreCase)))
						throw new InvalidRsmlSyntax("Invalid comparator.");

					if (!Int32.TryParse(major2, out _))
						throw new InvalidRsmlSyntax("The major version must be a valid integer. Wildcards are not compatible with comparators.");

					return;

				default:
					throw new InvalidRsmlSyntax("Unrecognized logic path overload.");

			}

		}

	}

}
