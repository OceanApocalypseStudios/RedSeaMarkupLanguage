using System;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Native;
using OceanApocalypseStudios.RSML.Native.Structures;


namespace OceanApocalypseStudios.RSML.Tests.Native
{

	public class NativeStructTests
	{

		[Theory]
		[InlineData(TokenKind.SpecialActionName, 17, 85)]
		[InlineData(TokenKind.CommentSymbol, -1, 825)]
		[InlineData(TokenKind.DefinedKeyword, 35, 69)]
		[InlineData(TokenKind.Eof, 0, 96)]
		[InlineData(TokenKind.SpecialActionSymbol, 4, 6)]
		[InlineData(TokenKind.NotEqualTo, 11, 886)]
		[InlineData(TokenKind.GreaterThanOrEqualTo, 14, 90)]
		public void ToNativeToken_ConvertsCorrectly(TokenKind kind, int start, int end)
		{

			var nativeToken = new SyntaxToken(kind, new(Math.Abs(start), start < 0), new(Math.Abs(end), end < 0)).ToNativeToken();
			NativeRsmlToken token = new() { kind = (byte)kind, startIndex = start, endIndex = end };
			Assert.True(token.Equals(nativeToken));

		}

		[Theory]
		[InlineData(17, 35, 69)]
		[InlineData(15, 17, 85)]
		[InlineData(254, 0, 96)]
		[InlineData(7, 11, 886)]
		[InlineData(19, 4, 6)]
		[InlineData(10, 14, 90)]
		[InlineData(3, -1, 825)]
		public void ToManagedToken_ConvertsCorrectly(byte kind, int start, int end)
		{

			var managedToken = new NativeRsmlToken() { kind = kind, startIndex = start, endIndex = end }.ToToken();
			SyntaxToken token = new((TokenKind)kind, new(Math.Abs(start), start < 0), new(Math.Abs(end), end < 0));
			Assert.True(token.Equals(managedToken));

		}

		[Fact]
		public void ToNativeLine_ConvertsCorrectly()
		{

			var nativeLine = new SyntaxLine(new(TokenKind.CommentSymbol, 4, 8), new(TokenKind.SpecialActionSymbol, 7, 10), new(TokenKind.SystemName, ^4, ^1), new(TokenKind.MajorVersionId, 2, 7), new(TokenKind.Eol, 15, 140)).ToNativeLine();
			NativeRsmlLine line = new()
			{
				item1 = new() { kind = 018, startIndex = 04, endIndex = 008 },
				item2 = new() { kind = 015, startIndex = 07, endIndex = 010 },
				item3 = new() { kind = 003, startIndex = -4, endIndex = -01 },
				item4 = new() { kind = 005, startIndex = 02, endIndex = 007 },
				item5 = new() { kind = 254, startIndex = 15, endIndex = 140 },
				item6 = SyntaxToken.Empty.ToNativeToken(),
				item7 = SyntaxToken.Empty.ToNativeToken(),
				item8 = SyntaxToken.Empty.ToNativeToken()
			};

			Assert.True(line.Equals(nativeLine));

		}

		[Fact]
		public void ToManagedLine_ConvertsCorrectly()
		{

			var managedLine = new NativeRsmlLine()
			{
				item1 = new() { kind = 018, startIndex = 04, endIndex = 008 },
				item2 = new() { kind = 015, startIndex = 07, endIndex = 010 },
				item3 = new() { kind = 003, startIndex = -4, endIndex = -01 },
				item4 = new() { kind = 005, startIndex = 02, endIndex = 007 },
				item5 = new() { kind = 254, startIndex = 15, endIndex = 140 },
				item6 = SyntaxToken.Empty.ToNativeToken(),
				item7 = SyntaxToken.Empty.ToNativeToken(),
				item8 = SyntaxToken.Empty.ToNativeToken()
			}.ToLine();

			SyntaxLine line = new(
				new(TokenKind.CommentSymbol, 4, 8),
				new(TokenKind.SpecialActionSymbol, 7, 10),
				new(TokenKind.SystemName, ^4, ^1),
				new(TokenKind.MajorVersionId, 2, 7),
				new(TokenKind.Eol, 15, 140)
			);

			Assert.True(line.Equals(managedLine));

		}

	}

}
