using OceanApocalypseStudios.RSML.Analyzer.Syntax;


namespace OceanApocalypseStudios.RSML.Internals
{

	internal static class ConstantTokens
	{

		public static SyntaxToken EolToken { get; } = new(TokenKind.Eol, ^1, 0);

		public static SyntaxToken WildcardToken { get; } = new(TokenKind.WildcardKeyword, ^1, 0);

	}

}
