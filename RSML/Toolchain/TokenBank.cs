using System;

using RSML.Analyzer.Syntax;


namespace RSML.Toolchain
{

	internal static class TokenBank
	{

		public static SyntaxToken eolToken = new(TokenKind.Eol, Environment.NewLine);
		public static SyntaxToken eofToken = new(TokenKind.Eof, '\0');
		public static SyntaxToken wildcardToken = new(TokenKind.WildcardKeyword, "any");
		public static SyntaxToken commentToken = new(TokenKind.CommentSymbol, '#');
		public static SyntaxToken specialActionSymbolToken = new(TokenKind.SpecialActionSymbol, '@');

	}

}
