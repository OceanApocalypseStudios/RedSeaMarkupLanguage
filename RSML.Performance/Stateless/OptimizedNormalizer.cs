using System;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Performance.Value;
using OceanApocalypseStudios.RSML.Toolchain.Compliance;


namespace OceanApocalypseStudios.RSML.Performance.Stateless
{

	/// <summary>
	/// An optimized RSML normalizer.
	/// </summary>
	public static class OptimizedNormalizer
	{

		/// <summary>
		/// The level of compliance, per feature, with the official language standard for the current API version.
		/// </summary>
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull("2.0.0");

		/// <summary>
		/// Normalizes a line of RSML.
		/// </summary>
		/// <param name="line">The line</param>
		/// <param name="length">The amount of tokens output</param>
		/// <returns>The normalized line</returns>
		public static SyntaxLine NormalizeLine(SyntaxLine line, out int length)
		{

			if (line.Length == 0)
			{

				length = 0;

				return new();

			}

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (line[0].Kind)
			{

				case TokenKind.Eol when line.Length == 1:
				case TokenKind.Eof when line.Length == 1:
					length = 1;

					return line;

				case TokenKind.CommentSymbol:
					switch (line.Length)
					{

						case 2:
							length = 3;

							return new(line[0], new(TokenKind.CommentText, ""), new(TokenKind.Eol, Environment.NewLine));

						case 3:
							length = 3;

							return line;

						default:
							length = 0;

							return new();

					}

				case TokenKind.SpecialActionSymbol:
					switch (line.Length)
					{

						case 3:
							length = 3;

							return new(
								line[0], line[1], new(TokenKind.SpecialActionArgument, ""), new(TokenKind.Eol, Environment.NewLine), ValueToken.Empty
							);

						case 4:
							length = 4;

							return line;

						default:
							length = 0;

							return line;

					}

				case TokenKind.ReturnOperator:
				case TokenKind.ThrowErrorOperator:
					switch (line.Length)
					{

						// new(TokenKind.Eol, Environment.NewLine) matters
						case 3:
							length = 6;

							return new(
								line[0], new(TokenKind.WildcardKeyword, "any"), new(TokenKind.WildcardKeyword, "any"),
								new(TokenKind.WildcardKeyword, "any"), line[1], new(TokenKind.Eol, Environment.NewLine), ValueToken.Empty,
								ValueToken.Empty
							);

						case 4:
							length = 6;

							return new(
								line[0], line[1], new(TokenKind.WildcardKeyword, "any"), new(TokenKind.WildcardKeyword, "any"), line[2],
								new(TokenKind.Eol, Environment.NewLine), ValueToken.Empty, ValueToken.Empty
							);

						case 5:
							length = 6;

							return new(
								line[0], line[1], new(TokenKind.WildcardKeyword, "any"), line[2], line[3],
								new(TokenKind.Eol, Environment.NewLine), ValueToken.Empty, ValueToken.Empty
							);

						case 6:
							length = 6;

							return new(
								line[0], line[1], line[2], line[3], line[4],
								new(TokenKind.Eol, Environment.NewLine),
								ValueToken.Empty, ValueToken.Empty
							);

						case 7:
							length = 7;

							return new(
								line[0], line[1], line[2], line[3], line[4],
								line[5],
								new(TokenKind.Eol, Environment.NewLine), ValueToken.Empty
							);

						default:
							length = 0;

							return new();
					}

				default:
					length = 0;

					return new();

			}

		}

	}

}
