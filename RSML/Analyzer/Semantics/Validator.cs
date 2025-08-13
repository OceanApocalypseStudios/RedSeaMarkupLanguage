using System;
using System.Collections.Immutable;
using System.Linq;

using RSML.Analyzer.Syntax;
using RSML.Exceptions;
using RSML.Toolchain.Compliance;


namespace RSML.Analyzer.Semantics
{

	/// <summary>
	/// The officially maintained semantics validator for RSML.
	/// </summary>
	public sealed class Validator : IValidator
	{

		private const string ApiVersion = "2.0.0";

		/// <inheritdoc />
		public SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull(ApiVersion);

		/// <inheritdoc />
		public ImmutableHashSet<string> ValidComparators => [ "==", "!=", "<", ">", "<=", ">=" ];

		/// <inheritdoc />
		public ImmutableHashSet<string> ValidArchitectures => [ "x64", "x86", "arm64", "arm32", "loongarch64" ];

		/// <inheritdoc />
		public ImmutableHashSet<string> ValidSystems =>
		[
			"windows", "osx", "linux", "freebsd", "debian", "ubuntu",
			"archlinux", "fedora"
		];

		/// <inheritdoc />
		public void ValidateLine(ReadOnlySpan<SyntaxToken> tokens)
		{

			if (tokens.IsEmpty)
				throw new InvalidRsmlSyntax("Empty token sequence.");

			var actualTokens = tokens.Length == 1
								   ? tokens
								   : tokens[^1].Kind == TokenKind.Eol
									   ? tokens[..^1]
									   : tokens;

			switch (actualTokens[0].Kind)
			{

				case TokenKind.Eol or TokenKind.Eof:
					return; // we're done here

				case TokenKind.CommentSymbol when actualTokens.Length != 2:
					throw new InvalidRsmlSyntax(
						"A comment must be 2 tokens long."
					); // even if you have a comment with no text not even spaces, you'll have 2 tokens

				case TokenKind.CommentSymbol when actualTokens[0].Value != "#":
					throw new InvalidRsmlSyntax($"Expected CommentSymbol of value '#', but received {actualTokens[0].Value} instead.");

				case TokenKind.CommentSymbol when actualTokens[1].Kind != TokenKind.CommentText:
					throw new InvalidRsmlSyntax($"Expected CommentText, but received {actualTokens[1].Kind} instead.");

				case TokenKind.CommentSymbol:
					return;

				case TokenKind.SpecialActionSymbol when actualTokens.Length != 3:
					throw new InvalidRsmlSyntax("A special action must be 3 tokens long."); // even with no arg, you'll have 3 tokens

				case TokenKind.SpecialActionSymbol when actualTokens[0].Value != "@":
					throw new InvalidRsmlSyntax($"Expected SpecialActionSymbol of value '@', but received {actualTokens[0].Value} instead.");

				case TokenKind.SpecialActionSymbol when actualTokens[1].Kind != TokenKind.SpecialActionName:
					throw new InvalidRsmlSyntax($"Expected SpecialActionName, but received {actualTokens[1].Kind} instead.");

				case TokenKind.SpecialActionSymbol when actualTokens[2].Kind != TokenKind.SpecialActionArgument:
					throw new InvalidRsmlSyntax($"Expected SpecialActionArgument, but received {actualTokens[2].Kind} instead.");

				case TokenKind.SpecialActionSymbol:
					return;

			}

			if (actualTokens[0].Kind is not (TokenKind.ReturnOperator or TokenKind.ThrowErrorOperator))
				throw new InvalidRsmlSyntax("Unrecognized start-of-line token.");

			if (actualTokens[0].Value is not ("!>" or "->"))
				throw new InvalidRsmlSyntax("Operator must be one of !> or ->.");

			switch (actualTokens.Length)
			{

				case 2:
					if (actualTokens[1].Kind != TokenKind.LogicPathValue)
						throw new InvalidRsmlSyntax("A 2 token long logic path must be a *Operator + LogicPathValue overload.");

					return;

				case 3:
					if ((actualTokens[1].Kind != TokenKind.SystemName &&
						 actualTokens[1].Kind != TokenKind.DefinedKeyword &&
						 actualTokens[1].Kind != TokenKind.WildcardKeyword) ||
						actualTokens[2].Kind != TokenKind.LogicPathValue)
						throw new InvalidRsmlSyntax("A 3 token long logic path must be a *Operator + SystemName + LogicPathValue overload.");

					string sysName1 = actualTokens[1].Value;

					if (actualTokens[1].Value != "any" && actualTokens[1].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (actualTokens[1].Value != "defined" && actualTokens[1].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (!ValidSystems.Any(s => sysName1.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						actualTokens[1].Kind == TokenKind.SystemName)
						throw new InvalidRsmlSyntax("Invalid system name as of v2.0.0.");

					return;

				case 4:
					if ((actualTokens[1].Kind != TokenKind.SystemName &&
						 actualTokens[1].Kind != TokenKind.DefinedKeyword &&
						 actualTokens[1].Kind != TokenKind.WildcardKeyword) ||
						(actualTokens[2].Kind != TokenKind.ArchitectureIdentifier &&
						 actualTokens[2].Kind != TokenKind.DefinedKeyword &&
						 actualTokens[2].Kind != TokenKind.WildcardKeyword) ||
						actualTokens[3].Kind != TokenKind.LogicPathValue)
					{
						throw new InvalidRsmlSyntax(
							"A 4 token long logic path must be a *Operator + SystemName + ArchitectureIdentifier + LogicPathValue overload."
						);
					}

					string sysName2 = actualTokens[1].Value;
					string archName1 = actualTokens[2].Value;

					if (actualTokens[1].Value != "any" && actualTokens[1].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (actualTokens[1].Value != "defined" && actualTokens[1].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (actualTokens[2].Value != "any" && actualTokens[2].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (actualTokens[2].Value != "defined" && actualTokens[2].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (!ValidSystems.Any(s => sysName2.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						actualTokens[1].Kind == TokenKind.SystemName)
						throw new InvalidRsmlSyntax("Invalid system name as of v2.0.0.");

					if (!ValidArchitectures.Any(s => archName1.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						actualTokens[2].Kind == TokenKind.ArchitectureIdentifier)
						throw new InvalidRsmlSyntax("Invalid architecture identifier as of v2.0.0.");

					return;

				case 5:
					if ((actualTokens[1].Kind != TokenKind.SystemName &&
						 actualTokens[1].Kind != TokenKind.DefinedKeyword &&
						 actualTokens[1].Kind != TokenKind.WildcardKeyword) ||
						(actualTokens[2].Kind != TokenKind.MajorVersionId &&
						 actualTokens[2].Kind != TokenKind.DefinedKeyword &&
						 actualTokens[2].Kind != TokenKind.WildcardKeyword) ||
						(actualTokens[3].Kind != TokenKind.ArchitectureIdentifier &&
						 actualTokens[3].Kind != TokenKind.DefinedKeyword &&
						 actualTokens[3].Kind != TokenKind.WildcardKeyword) ||
						actualTokens[4].Kind != TokenKind.LogicPathValue)
					{
						throw new InvalidRsmlSyntax(
							"A 5 token long logic path must be a *Operator + SystemName + MajorVersionId + ArchitectureIdentifier + LogicPathValue overload."
						);
					}

					string sysName3 = actualTokens[1].Value;
					string major1 = actualTokens[2].Value;
					string archName2 = actualTokens[3].Value;

					if (actualTokens[1].Value != "any" && actualTokens[1].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (actualTokens[1].Value != "defined" && actualTokens[1].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (actualTokens[2].Value != "any" && actualTokens[2].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (actualTokens[2].Value != "defined" && actualTokens[2].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (actualTokens[3].Value != "any" && actualTokens[3].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (actualTokens[3].Value != "defined" && actualTokens[3].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (!ValidSystems.Any(s => sysName3.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						actualTokens[1].Kind == TokenKind.SystemName)
						throw new InvalidRsmlSyntax($"Invalid system name ({sysName3}) as of v2.0.0.");

					if (!ValidArchitectures.Any(s => archName2.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						actualTokens[3].Kind == TokenKind.ArchitectureIdentifier)
						throw new InvalidRsmlSyntax("Invalid architecture identifier as of v2.0.0.");

					if (!Int32.TryParse(major1, out _) && actualTokens[2].Kind == TokenKind.MajorVersionId)
						throw new InvalidRsmlSyntax("The major version must be a valid integer");

					return;

				case 6:
					if ((actualTokens[1].Kind != TokenKind.SystemName &&
						 actualTokens[1].Kind != TokenKind.DefinedKeyword &&
						 actualTokens[1].Kind != TokenKind.WildcardKeyword) ||
						(actualTokens[2].Kind != TokenKind.Equals &&
						 actualTokens[2].Kind != TokenKind.Different &&
						 actualTokens[2].Kind != TokenKind.GreaterThan &&
						 actualTokens[2].Kind != TokenKind.LessThan &&
						 actualTokens[2].Kind != TokenKind.GreaterOrEqualsThan &&
						 actualTokens[2].Kind != TokenKind.LessOrEqualsThan) ||
						(actualTokens[3].Kind != TokenKind.MajorVersionId &&
						 actualTokens[3].Kind != TokenKind.DefinedKeyword &&
						 actualTokens[3].Kind != TokenKind.WildcardKeyword) ||
						(actualTokens[4].Kind != TokenKind.ArchitectureIdentifier &&
						 actualTokens[4].Kind != TokenKind.DefinedKeyword &&
						 actualTokens[4].Kind != TokenKind.WildcardKeyword) ||
						actualTokens[5].Kind != TokenKind.LogicPathValue)
					{
						throw new InvalidRsmlSyntax(
							"A 5 token long logic path must be a *Operator + SystemName + |Comparator| + MajorVersionId + ArchitectureIdentifier + LogicPathValue overload."
						);
					}

					string sysName4 = actualTokens[1].Value;
					string comp = actualTokens[2].Value;
					string major2 = actualTokens[3].Value;
					string archName3 = actualTokens[4].Value;

					if (actualTokens[1].Value != "any" && actualTokens[1].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (actualTokens[1].Value != "defined" && actualTokens[1].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (actualTokens[4].Value != "any" && actualTokens[4].Kind == TokenKind.WildcardKeyword)
						throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

					if (actualTokens[4].Value != "defined" && actualTokens[4].Kind == TokenKind.DefinedKeyword)
						throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

					if (!ValidSystems.Any(s => sysName4.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						actualTokens[1].Kind == TokenKind.SystemName)
						throw new InvalidRsmlSyntax("Invalid system name as of v2.0.0.");

					if (!ValidArchitectures.Any(s => archName3.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
						actualTokens[1].Kind == TokenKind.ArchitectureIdentifier)
						throw new InvalidRsmlSyntax("Invalid architecture identifier as of v2.0.0.");

					if (!ValidComparators.Any(s => comp.Equals(s, StringComparison.OrdinalIgnoreCase)))
						throw new InvalidRsmlSyntax("Invalid comparator.");

					if (!Int32.TryParse(major2, out _))
						throw new InvalidRsmlSyntax("The major version must be a valid integer. Wildcards are not compatible with comparators.");

					return;

				default:
					throw new InvalidRsmlSyntax("Unrecognized logic path overload.");

			}

		}

		/// <inheritdoc />
		public void ValidateBuffer(ReadOnlySpan<SyntaxToken> bufferTokens)
		{

			if (bufferTokens.IsEmpty)
				throw new InvalidRsmlSyntax("Empty token sequence.");

			int pos = 0;

			while (pos < bufferTokens.Length)
			{

				bool eofHit = SkipNewlines(bufferTokens, ref pos);

				if (eofHit)
					return;

				var line = ReadUntilEolOrEof(bufferTokens, ref pos);

				if (line.IsEmpty)
					continue;

				ValidateLine(line);

			}

		}

		#region Helpers

		private static ReadOnlySpan<SyntaxToken> ReadUntilEolOrEof(ReadOnlySpan<SyntaxToken> tokens, ref int pos)
		{

			int start = pos;

			while (pos < tokens.Length && tokens[pos].Kind != TokenKind.Eol && tokens[pos].Kind != TokenKind.Eof)
				pos++;

			return tokens[start..pos];

		}

		private static bool SkipNewlines(ReadOnlySpan<SyntaxToken> tokens, ref int pos)
		{

			while (pos < tokens.Length && tokens[pos].Kind == TokenKind.Eol)
			{

				pos++;

				if (tokens[pos].Kind == TokenKind.Eof)
					return false; // eof hit

			}

			return true;

		}

		#endregion

	}

}
