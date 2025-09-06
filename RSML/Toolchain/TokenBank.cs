using OceanApocalypseStudios.RSML.Analyzer.Syntax;


namespace OceanApocalypseStudios.RSML.Toolchain
{

	internal static class TokenBank
	{

		public static SyntaxToken eolToken = new(TokenKind.Eol, ^1, 0);
		public static SyntaxToken wildcardToken = new(TokenKind.WildcardKeyword, ^1, 0);

	}

}
