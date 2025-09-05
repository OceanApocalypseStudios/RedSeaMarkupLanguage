using System;
using System.Collections.Immutable;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Toolchain.Compliance;


namespace OceanApocalypseStudios.RSML.Analyzer.Semantics
{

	/// <summary>
	/// The officially maintained semantics validator for RSML.
	/// </summary>
	public sealed class Validator : IValidator
	{

		private const string ApiVersion = "2.0.0";

		/// <inheritdoc />
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull(ApiVersion);

		/// <inheritdoc />
		public static ImmutableArray<ReadOnlyMemory<char>> ValidArchitectures =>
		[
			"x64".AsMemory(), "x86".AsMemory(), "arm64".AsMemory(), "arm32".AsMemory(), "loongarch64".AsMemory()
		];

		/// <inheritdoc />
		public static ImmutableArray<ReadOnlyMemory<char>> ValidComparators =>
		[
			"==".AsMemory(), "!=".AsMemory(), "<".AsMemory(), ">".AsMemory(), "<=".AsMemory(), ">=".AsMemory()
		];

		/// <inheritdoc />
		public static ImmutableArray<ReadOnlyMemory<char>> ValidSpecialActionNames =>
		[
			"Void".AsMemory(), "ThrowError".AsMemory(), "EndAll".AsMemory()
		];

		/// <inheritdoc />
		public static ImmutableArray<ReadOnlyMemory<char>> ValidSystems =>
		[
			"windows".AsMemory(), "osx".AsMemory(), "linux".AsMemory(), "freebsd".AsMemory(), "debian".AsMemory(), "ubuntu".AsMemory(),
			"archlinux".AsMemory(), "fedora".AsMemory()
		];

		/// <inheritdoc />
		public static void ValidateLine(SyntaxLine line, DualTextBuffer context)
		{

			if (line.IsEmpty)
				throw new InvalidRsmlSyntax("Empty token sequence.");

			if (line.Length > 1 && line[^1].Kind == TokenKind.Eol)
				line.Remove(line.Last());

			switch (line[0].Kind)
			{

				case TokenKind.Eol or TokenKind.Eof:
					return; // we're done here

				case TokenKind.CommentSymbol when line.Length != 2:
					throw new InvalidRsmlSyntax(
						"A comment must be 2 tokens long."
					); // even if you have a comment with no text not even spaces, you'll have 2 tokens

				case TokenKind.CommentSymbol when line[1].Kind != TokenKind.CommentText:
					throw new InvalidRsmlSyntax($"Expected CommentText, but received {line[1].Kind} instead.");

				case TokenKind.CommentSymbol:
					return;

				case TokenKind.SpecialActionSymbol when line.Length != 3:
					throw new InvalidRsmlSyntax("A special action must be 3 tokens long."); // even with no arg, you'll have 3 tokens

				case TokenKind.SpecialActionSymbol when line[1].IsOffLimits ||
														line[1].Kind != TokenKind.SpecialActionName ||
														!ValidSpecialActionNames.ContainsMemory(context[line[1].BufferRange]):
					throw new InvalidRsmlSyntax($"Expected a valid SpecialActionName, but received {line[1].Kind} with wrong value instead.");

				case TokenKind.SpecialActionSymbol when line[2].Kind != TokenKind.SpecialActionArgument:
					throw new InvalidRsmlSyntax($"Expected SpecialActionArgument, but received {line[2].Kind} instead.");

				case TokenKind.SpecialActionSymbol:
					return;

			}

			if (line[0].Kind is not (TokenKind.ReturnOperator or TokenKind.ThrowErrorOperator) || line[0].IsOffLimits)
				throw new InvalidRsmlSyntax("Unrecognized start-of-line token.");

			if (!context[line[0].BufferRange].IsEquals("!>") && !context[line[0].BufferRange].IsEquals("->"))
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

					if (!line[1].IsOffLimits &&
						!context[line[1].BufferRange].IsAsciiEqualsIgnoreCase(ValidSystems) &&
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

					if (!line[1].IsOffLimits &&
						!context[line[1].BufferRange].IsAsciiEqualsIgnoreCase(ValidSystems) &&
						line[1].Kind == TokenKind.SystemName)
						throw new InvalidRsmlSyntax("Invalid system name as of v2.0.0.");

					// ReSharper disable once InvertIf
					if (!line[2].IsOffLimits &&
						!context[line[2].BufferRange].IsAsciiEqualsIgnoreCase(ValidArchitectures) &&
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


					if (!line[1].IsOffLimits &&
						!context[line[1].BufferRange].IsAsciiEqualsIgnoreCase(ValidSystems) &&
						line[1].Kind == TokenKind.SystemName)
						throw new InvalidRsmlSyntax("Invalid system name as of v2.0.0.");

					if (!line[3].IsOffLimits &&
						!context[line[3].BufferRange].IsAsciiEqualsIgnoreCase(ValidArchitectures) &&
						line[3].Kind == TokenKind.ArchitectureIdentifier)
						throw new InvalidRsmlSyntax("Invalid architecture identifier as of v2.0.0.");

					if (!line[2].IsOffLimits && !Int32.TryParse(context[line[2].BufferRange].Span, out _) && line[2].Kind == TokenKind.MajorVersionId)
						throw new InvalidRsmlSyntax("The major version must be a valid integer");

					return;

				case 6:
					if ((line[1].Kind != TokenKind.SystemName &&
						 line[1].Kind != TokenKind.DefinedKeyword &&
						 line[1].Kind != TokenKind.WildcardKeyword) ||
						(line[2].Kind != TokenKind.EqualTo &&
						 line[2].Kind != TokenKind.NotEqualTo &&
						 line[2].Kind != TokenKind.GreaterThan &&
						 line[2].Kind != TokenKind.LessThan &&
						 line[2].Kind != TokenKind.GreaterThanOrEqualTo &&
						 line[2].Kind != TokenKind.LessThanOrEqualTo) ||
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

					if (!line[1].IsOffLimits &&
						!context[line[1].BufferRange].IsAsciiEqualsIgnoreCase(ValidSystems) &&
						line[1].Kind == TokenKind.SystemName)
						throw new InvalidRsmlSyntax("Invalid system name as of v2.0.0.");

					if (!line[4].IsOffLimits &&
						!context[line[4].BufferRange].IsAsciiEqualsIgnoreCase(ValidArchitectures) &&
						line[4].Kind == TokenKind.ArchitectureIdentifier)
						throw new InvalidRsmlSyntax("Invalid architecture identifier as of v2.0.0.");

					if (!line[2].IsOffLimits &&
						!context[line[2].BufferRange].IsAsciiEqualsIgnoreCase(ValidComparators))
						throw new InvalidRsmlSyntax("Invalid comparator.");

					if (!line[3].IsOffLimits && !Int32.TryParse(context[line[3].BufferRange].Span, out _))
						throw new InvalidRsmlSyntax("The major version must be a valid integer. Wildcards are not compatible with comparators.");

					return;

				default:
					throw new InvalidRsmlSyntax("Unrecognized logic path overload.");

			}

		}

	}

}
