using System;

using RSML.Analyzer.Syntax;


namespace RSML.Toolchain
{

	internal static class TokenBank
	{

		public static SyntaxToken eolToken = new(TokenKind.Eol, Environment.NewLine);
		public static SyntaxToken eofToken = new(TokenKind.Eof, '\0');
		public static SyntaxToken wildcard = new(TokenKind.WildcardKeyword, "any");

	}

}
