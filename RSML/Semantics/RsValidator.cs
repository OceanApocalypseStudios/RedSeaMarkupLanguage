using System;
using System.Collections.Immutable;
using System.Linq;

using RSML.Exceptions;
using RSML.Tokenization;


namespace RSML.Semantics
{

	/// <summary>
	/// The officially maintained semantics validator for RSML.
	/// </summary>
	public sealed class RsValidator : IValidator
	{

		/// <inheritdoc />
		public string? StandardizedVersion => "2.0.0";

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
		public void ValidateLine(ReadOnlySpan<RsToken> tokens)
		{

			if (tokens.IsEmpty)
				throw new InvalidRsmlSyntax("Empty token sequence.");

			var actualTokens = tokens.Length == 1
								   ? tokens
								   : tokens[^1].Type == RsTokenType.Eol
									   ? tokens[..^1]
									   : tokens;

			if (actualTokens[0].Type is RsTokenType.Eol or RsTokenType.Eof)
				return; // we're done here

			if (actualTokens[0].Type == RsTokenType.CommentSymbol)

				#region Comments

			{

				if (actualTokens.Length != 2)
					throw new InvalidRsmlSyntax(
						"A comment must be 2 tokens long."
					); // even if you have a comment with no text not even spaces, you'll have 2 tokens

				if (actualTokens[0].Value != "#")
					throw new InvalidRsmlSyntax($"Expected CommentSymbol of value '#', but received {actualTokens[0].Value} instead.");

				if (actualTokens[1].Type != RsTokenType.CommentText)
					throw new InvalidRsmlSyntax($"Expected CommentText, but received {actualTokens[1].Type} instead.");

				return;

			}

			#endregion

			if (actualTokens[0].Type == RsTokenType.SpecialActionSymbol)

				#region Special Actions

			{

				if (actualTokens.Length != 3)
					throw new InvalidRsmlSyntax("A special action must be 3 tokens long."); // even with no arg, you'll have 3 tokens

				if (actualTokens[0].Value != "@")
					throw new InvalidRsmlSyntax($"Expected SpecialActionSymbol of value '@', but received {actualTokens[0].Value} instead.");

				if (actualTokens[1].Type != RsTokenType.SpecialActionName)
					throw new InvalidRsmlSyntax($"Expected SpecialActionName, but received {actualTokens[1].Type} instead.");

				if (actualTokens[2].Type != RsTokenType.SpecialActionArgument)
					throw new InvalidRsmlSyntax($"Expected SpecialActionArgument, but received {actualTokens[2].Type} instead.");

				return;

			}

			#endregion

			if (actualTokens[0].Type is RsTokenType.ReturnOperator or RsTokenType.ThrowErrorOperator)

				#region Logic Path

			{

				if (!(actualTokens[0].Value is "!>" or "->"))
					throw new InvalidRsmlSyntax("Operator must be one of !> or ->.");

				switch (actualTokens.Length)
				{

					case 2:
						if (actualTokens[1].Type != RsTokenType.LogicPathValue)
							throw new InvalidRsmlSyntax("A 2 token long logic path must be a *Operator + LogicPathValue overload.");

						return;

					case 3:
						if ((actualTokens[1].Type != RsTokenType.SystemName &&
							 actualTokens[1].Type != RsTokenType.DefinedKeyword &&
							 actualTokens[1].Type != RsTokenType.WildcardKeyword) ||
							actualTokens[2].Type != RsTokenType.LogicPathValue)
							throw new InvalidRsmlSyntax("A 3 token long logic path must be a *Operator + SystemName + LogicPathValue overload.");

						string sysName1 = actualTokens[1].Value;

						if (actualTokens[1].Value != "any" && actualTokens[1].Type == RsTokenType.WildcardKeyword)
							throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

						if (actualTokens[1].Value != "defined" && actualTokens[1].Type == RsTokenType.DefinedKeyword)
							throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

						if (!ValidSystems.Any(s => sysName1.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
							actualTokens[1].Type == RsTokenType.SystemName)
							throw new InvalidRsmlSyntax("Invalid system name as of v2.0.0.");

						return;

					case 4:
						if ((actualTokens[1].Type != RsTokenType.SystemName &&
							 actualTokens[1].Type != RsTokenType.DefinedKeyword &&
							 actualTokens[1].Type != RsTokenType.WildcardKeyword) ||
							(actualTokens[2].Type != RsTokenType.ArchitectureIdentifier &&
							 actualTokens[2].Type != RsTokenType.DefinedKeyword &&
							 actualTokens[2].Type != RsTokenType.WildcardKeyword) ||
							actualTokens[3].Type != RsTokenType.LogicPathValue)
							throw new InvalidRsmlSyntax(
								"A 4 token long logic path must be a *Operator + SystemName + ArchitectureIdentifier + LogicPathValue overload."
							);

						string sysName2 = actualTokens[1].Value;
						string archName1 = actualTokens[2].Value;

						if (actualTokens[1].Value != "any" && actualTokens[1].Type == RsTokenType.WildcardKeyword)
							throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

						if (actualTokens[1].Value != "defined" && actualTokens[1].Type == RsTokenType.DefinedKeyword)
							throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

						if (actualTokens[2].Value != "any" && actualTokens[2].Type == RsTokenType.WildcardKeyword)
							throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

						if (actualTokens[2].Value != "defined" && actualTokens[2].Type == RsTokenType.DefinedKeyword)
							throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

						if (!ValidSystems.Any(s => sysName2.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
							actualTokens[1].Type == RsTokenType.SystemName)
							throw new InvalidRsmlSyntax("Invalid system name as of v2.0.0.");

						if (!ValidArchitectures.Any(s => archName1.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
							actualTokens[2].Type == RsTokenType.ArchitectureIdentifier)
							throw new InvalidRsmlSyntax("Invalid architecture identifier as of v2.0.0.");

						return;

					case 5:
						if ((actualTokens[1].Type != RsTokenType.SystemName &&
							 actualTokens[1].Type != RsTokenType.DefinedKeyword &&
							 actualTokens[1].Type != RsTokenType.WildcardKeyword) ||
							(actualTokens[2].Type != RsTokenType.MajorVersionId &&
							 actualTokens[2].Type != RsTokenType.DefinedKeyword &&
							 actualTokens[2].Type != RsTokenType.WildcardKeyword) ||
							(actualTokens[3].Type != RsTokenType.ArchitectureIdentifier &&
							 actualTokens[3].Type != RsTokenType.DefinedKeyword &&
							 actualTokens[3].Type != RsTokenType.WildcardKeyword) ||
							actualTokens[4].Type != RsTokenType.LogicPathValue)
							throw new InvalidRsmlSyntax(
								"A 5 token long logic path must be a *Operator + SystemName + MajorVersionId + ArchitectureIdentifier + LogicPathValue overload."
							);

						string sysName3 = actualTokens[1].Value;
						string major1 = actualTokens[2].Value;
						string archName2 = actualTokens[3].Value;

						if (actualTokens[1].Value != "any" && actualTokens[1].Type == RsTokenType.WildcardKeyword)
							throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

						if (actualTokens[1].Value != "defined" && actualTokens[1].Type == RsTokenType.DefinedKeyword)
							throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

						if (actualTokens[2].Value != "any" && actualTokens[2].Type == RsTokenType.WildcardKeyword)
							throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

						if (actualTokens[2].Value != "defined" && actualTokens[2].Type == RsTokenType.DefinedKeyword)
							throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

						if (actualTokens[3].Value != "any" && actualTokens[3].Type == RsTokenType.WildcardKeyword)
							throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

						if (actualTokens[3].Value != "defined" && actualTokens[3].Type == RsTokenType.DefinedKeyword)
							throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

						if (!ValidSystems.Any(s => sysName3.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
							actualTokens[1].Type == RsTokenType.SystemName)
							throw new InvalidRsmlSyntax($"Invalid system name ({sysName3}) as of v2.0.0.");

						if (!ValidArchitectures.Any(s => archName2.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
							actualTokens[3].Type == RsTokenType.ArchitectureIdentifier)
							throw new InvalidRsmlSyntax("Invalid architecture identifier as of v2.0.0.");

						if (!Int32.TryParse(major1, out _) && actualTokens[2].Type == RsTokenType.MajorVersionId)
							throw new InvalidRsmlSyntax("The major version must be a valid integer");

						return;

					case 6:
						if ((actualTokens[1].Type != RsTokenType.SystemName &&
							 actualTokens[1].Type != RsTokenType.DefinedKeyword &&
							 actualTokens[1].Type != RsTokenType.WildcardKeyword) ||
							(actualTokens[2].Type != RsTokenType.Equals &&
							 actualTokens[2].Type != RsTokenType.Different &&
							 actualTokens[2].Type != RsTokenType.GreaterThan &&
							 actualTokens[2].Type != RsTokenType.LessThan &&
							 actualTokens[2].Type != RsTokenType.GreaterOrEqualsThan &&
							 actualTokens[2].Type != RsTokenType.LessOrEqualsThan) ||
							(actualTokens[3].Type != RsTokenType.MajorVersionId &&
							 actualTokens[3].Type != RsTokenType.DefinedKeyword &&
							 actualTokens[3].Type != RsTokenType.WildcardKeyword) ||
							(actualTokens[4].Type != RsTokenType.ArchitectureIdentifier &&
							 actualTokens[4].Type != RsTokenType.DefinedKeyword &&
							 actualTokens[4].Type != RsTokenType.WildcardKeyword) ||
							actualTokens[5].Type != RsTokenType.LogicPathValue)
							throw new InvalidRsmlSyntax(
								"A 5 token long logic path must be a *Operator + SystemName + |Comparator| + MajorVersionId + ArchitectureIdentifier + LogicPathValue overload."
							);

						string sysName4 = actualTokens[1].Value;
						string comp = actualTokens[2].Value;
						string major2 = actualTokens[3].Value;
						string archName3 = actualTokens[4].Value;

						if (actualTokens[1].Value != "any" && actualTokens[1].Type == RsTokenType.WildcardKeyword)
							throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

						if (actualTokens[1].Value != "defined" && actualTokens[1].Type == RsTokenType.DefinedKeyword)
							throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

						if (actualTokens[4].Value != "any" && actualTokens[4].Type == RsTokenType.WildcardKeyword)
							throw new InvalidRsmlSyntax("A token of type WildcardKeyword must have a value of 'any'.");

						if (actualTokens[4].Value != "defined" && actualTokens[4].Type == RsTokenType.DefinedKeyword)
							throw new InvalidRsmlSyntax("A token of type DefinedKeyword must have a value of 'defined'.");

						if (!ValidSystems.Any(s => sysName4.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
							actualTokens[1].Type == RsTokenType.SystemName)
							throw new InvalidRsmlSyntax("Invalid system name as of v2.0.0.");

						if (!ValidArchitectures.Any(s => archName3.Equals(s, StringComparison.OrdinalIgnoreCase)) &&
							actualTokens[1].Type == RsTokenType.ArchitectureIdentifier)
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

			#endregion

			throw new InvalidRsmlSyntax("Unrecognized start-of-line token.");

		}

		/// <inheritdoc />
		public void ValidateBuffer(ReadOnlySpan<RsToken> bufferTokens)
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

		private static ReadOnlySpan<RsToken> ReadUntilEolOrEof(ReadOnlySpan<RsToken> tokens, ref int pos)
		{

			int start = pos;

			while (pos < tokens.Length && tokens[pos].Type != RsTokenType.Eol && tokens[pos].Type != RsTokenType.Eof)
				pos++;

			return tokens[start..pos];

		}

		private static bool SkipNewlines(ReadOnlySpan<RsToken> tokens, ref int pos)
		{

			while (pos < tokens.Length && tokens[pos].Type == RsTokenType.Eol)
			{

				pos++;

				if (tokens[pos].Type == RsTokenType.Eof)
					return false; // eof hit

			}

			return true;

		}

	}

}
