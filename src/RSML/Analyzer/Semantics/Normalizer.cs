using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Toolchain;
using OceanApocalypseStudios.RSML.Toolchain.Compliance;


namespace OceanApocalypseStudios.RSML.Analyzer.Semantics
{

	/// <summary>
	/// The official RSML normalizer.
	/// </summary>
	public sealed class Normalizer : INormalizer
	{

		private const string ApiVersion = "2.0.0";
		private static readonly SyntaxToken eol = TokenBank.eolToken;

		private static readonly SyntaxToken wildcard = TokenBank.wildcardToken;

		/// <summary>
		/// Creates a new Normalizer instance.
		/// </summary>
		public Normalizer() { }

		/// <inheritdoc />
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull(ApiVersion);

		/// <inheritdoc />
		public static void NormalizeLine(ref SyntaxLine line, out int tokenCount)
		{

			if (line.Length == 0)
			{

				tokenCount = 0;
				line.Clear();

				return;

			}

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (line[0].Kind)
			{

				case TokenKind.Eol when line.Length == 1:
				case TokenKind.Eof when line.Length == 1:
					tokenCount = 1;

					return;

				case TokenKind.CommentSymbol:
					switch (line.Length)
					{

						case 2:
							line[1] = new(TokenKind.CommentText, ^1, 0);
							line[2] = eol;
							line[3] = SyntaxToken.Empty;
							line[4] = SyntaxToken.Empty;
							line[5] = SyntaxToken.Empty;
							line[6] = SyntaxToken.Empty;
							line[7] = SyntaxToken.Empty;

							tokenCount = 3;

							break;

						case 3:
							tokenCount = 3;

							break;

						default:
							line.Clear();
							tokenCount = 0;

							break;

					}

					return;

				case TokenKind.SpecialActionSymbol:
					switch (line.Length)
					{

						case 3:
							line[2] = new(TokenKind.SpecialActionArgument, ^1, 0);
							line[3] = eol;
							line[4] = SyntaxToken.Empty;
							line[5] = SyntaxToken.Empty;
							line[6] = SyntaxToken.Empty;
							line[7] = SyntaxToken.Empty;

							tokenCount = 4;

							break;

						case 4:
							tokenCount = 4;

							break;

						default:
							line.Clear();
							tokenCount = 0;

							break;

					}

					return;

				case TokenKind.ReturnOperator:
				case TokenKind.ThrowErrorOperator:
					switch (line.Length)
					{
						// eol matters
						case 3:
							line[4] = line[1];
							line[1] = line[2] = line[3] = wildcard;
							line[5] = eol;
							line[6] = SyntaxToken.Empty;
							line[7] = SyntaxToken.Empty;

							tokenCount = 6;

							break;

						case 4:
							line[4] = line[2];
							line[2] = line[3] = wildcard;
							line[5] = eol;
							line[6] = SyntaxToken.Empty;
							line[7] = SyntaxToken.Empty;

							tokenCount = 6;

							break;

						case 5:
							line[4] = line[3];
							line[3] = line[2];
							line[2] = wildcard;
							line[5] = eol;
							line[6] = SyntaxToken.Empty;
							line[7] = SyntaxToken.Empty;

							tokenCount = 6;

							break;

						case 6:
							line[5] = eol;
							line[6] = SyntaxToken.Empty;
							line[7] = SyntaxToken.Empty;

							tokenCount = 6;

							break;

						case 7:
							/*
							 line[6] = eol;
							line[7] = SyntaxToken.Empty;
							*/

							tokenCount = 7;

							break;

						default:
							line.Clear();
							tokenCount = 0;

							break;

					}

					return;

				default:
					line.Clear();
					tokenCount = 0;

					return;

			}

		}

	}

}
