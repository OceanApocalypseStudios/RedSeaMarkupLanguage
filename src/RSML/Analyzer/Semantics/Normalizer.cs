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

						// i was gonna say make this stricter, but it's just a comment sooo no change on this one
						// i think it's a good idea - im full of them :))
						// also jerry what the fuck??
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

						// todo: make this stricter (error throw instead of silent clear)
						default:
							line.Clear();
							tokenCount = 0;

							break;

					}

					return;

				case TokenKind.ReturnOperator:
				case TokenKind.ThrowErrorOperator:
					// eol matters
					switch (line.Length)
					{

						// operator + value + eol
						// becomes
						// operator + any(name) + any(version) + any(arch) + value + eol
						case 3:
							// value
							line[4] = line[1];

							// name, version and arch become "any"
							line[1] = line[2] = line[3] = wildcard;

							// eol and clear
							line[5] = eol;
							line[6] = SyntaxToken.Empty;
							line[7] = SyntaxToken.Empty;

							tokenCount = 6;

							break;

						// operator + name + value + eol
						// becomes
						// operator + name + any(version) + any(arch) + value + eol
						case 4:
							// value
							line[4] = line[2];

							// version and arch become "any"
							line[2] = line[3] = wildcard;

							// eol and clear
							line[5] = eol;
							line[6] = SyntaxToken.Empty;
							line[7] = SyntaxToken.Empty;

							tokenCount = 6;

							break;

						// operator + name + arch + value + eol
						// becomes
						// operator + name + arch + any(version) + value + eol
						case 5:
							// value
							line[4] = line[3];

							// arch
							line[3] = line[2];

							// version becomes "any"
							line[2] = wildcard;

							// eol and clear
							line[5] = eol;
							line[6] = SyntaxToken.Empty;
							line[7] = SyntaxToken.Empty;

							tokenCount = 6;

							break;

						// operator + name + version + arch + value + eol
						// is already normalized
						// eol and clear
						case 6:
							line[5] = eol;
							line[6] = SyntaxToken.Empty;
							line[7] = SyntaxToken.Empty;

							tokenCount = 6;

							break;

						// operator + name + comparison symbol + version + arch + value + eol
						// is already normalized
						// no need for eol and clear (it's already eol)
						case 7:
							/*
							 line[6] = eol;
							line[7] = SyntaxToken.Empty;
							*/

							tokenCount = 7;

							break;

						// not a valid form
						// todo: make this stricter (error throw instead of silent clear)
						default:
							line.Clear();
							tokenCount = 0;

							break;

					}

					return;

				// todo: make this stricter (error throw instead of silent clear)
				default:
					line.Clear();
					tokenCount = 0;

					return;

			}

		}

	}

}
