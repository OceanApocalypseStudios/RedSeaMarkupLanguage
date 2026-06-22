using System;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
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
		public void Ext_ToNativeToken_ConvertsCorrectly(TokenKind kind, int start, int end)
		{

			var nativeToken = new SyntaxToken(kind, new(Math.Abs(start), start < 0), new(Math.Abs(end), end < 0)).ToNativeToken();
			NativeToken token = new() { kind = (byte)kind, startIndex = start, endIndex = end };
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
		public void Ext_ToManagedToken_ConvertsCorrectly(byte kind, int start, int end)
		{

			var managedToken = new NativeToken() { kind = kind, startIndex = start, endIndex = end }.ToToken();
			SyntaxToken token = new((TokenKind)kind, new(Math.Abs(start), start < 0), new(Math.Abs(end), end < 0));
			Assert.True(token.Equals(managedToken));

		}

		[Fact]
		public void Ext_ToNativeLine_ConvertsCorrectly()
		{

			var nativeLine = new SyntaxLine(new(TokenKind.CommentSymbol, 4, 8), new(TokenKind.SpecialActionSymbol, 7, 10), new(TokenKind.SystemName, ^4, ^1), new(TokenKind.MajorVersionId, 2, 7), new(TokenKind.Eol, 15, 140)).ToNativeLine();
			NativeLine line = new()
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
		public void Ext_ToManagedLine_ConvertsCorrectly()
		{

			var managedLine = new NativeLine()
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

		[Fact]
		public void NativeLine_EqualsOperator_NativeLine()
		{

			NativeLine nativeLine1 = new()
			{
				item1 = new() { kind = 018, startIndex = 04, endIndex = 008 },
				item2 = new() { kind = 015, startIndex = 07, endIndex = 010 },
				item3 = new() { kind = 003, startIndex = -4, endIndex = -01 },
				item4 = new() { kind = 005, startIndex = 02, endIndex = 007 },
				item5 = SyntaxToken.Empty.ToNativeToken(),
				item6 = SyntaxToken.Empty.ToNativeToken(),
				item7 = SyntaxToken.Empty.ToNativeToken(),
				item8 = SyntaxToken.Empty.ToNativeToken()
			};

			NativeLine nativeLine2 = new()
			{
				item1 = new() { kind = 018, startIndex = 04, endIndex = 008 },
				item2 = new() { kind = 015, startIndex = 07, endIndex = 010 },
				item3 = new() { kind = 003, startIndex = -4, endIndex = -01 },
				item4 = new() { kind = 005, startIndex = 02, endIndex = 007 },
				item5 = SyntaxToken.Empty.ToNativeToken(),
				item6 = SyntaxToken.Empty.ToNativeToken(),
				item7 = SyntaxToken.Empty.ToNativeToken(),
				item8 = SyntaxToken.Empty.ToNativeToken()
			};

			Assert.True(nativeLine1 == nativeLine2);

		}

		[Fact]
		public void NativeToken_EqualsOperator1_NativeToken()
		{

			NativeToken nativeToken1 = new()
			{
				kind = 254,
				startIndex = -26,
				endIndex = -1
			};

			NativeToken nativeToken2 = new()
			{
				kind = 254,
				startIndex = -26,
				endIndex = -1
			};

			Assert.True(nativeToken1 == nativeToken2);

		}

		[Fact]
		public void NativeLine_NotEqualsOperator_NativeLine()
		{

			NativeLine nativeLine1 = new()
			{
				item1 = new() { kind = 018, startIndex = 04, endIndex = 008 },
				item2 = new() { kind = 015, startIndex = 07, endIndex = 010 },
				item3 = new() { kind = 003, startIndex = -4, endIndex = -01 },
				item4 = new() { kind = 005, startIndex = 02, endIndex = 007 },
				item5 = SyntaxToken.Empty.ToNativeToken(),
				item6 = SyntaxToken.Empty.ToNativeToken(),
				item7 = SyntaxToken.Empty.ToNativeToken(),
				item8 = SyntaxToken.Empty.ToNativeToken()
			};

			NativeLine nativeLine2 = new()
			{
				item1 = new() { kind = 015, startIndex = 07, endIndex = 010 },
				item2 = SyntaxToken.Empty.ToNativeToken(),
				item3 = new() { kind = 018, startIndex = 04, endIndex = 008 },
				item4 = new() { kind = 005, startIndex = 02, endIndex = 007 },
				item5 = SyntaxToken.Empty.ToNativeToken(),
				item6 = new() { kind = 003, startIndex = -4, endIndex = -01 },
				item7 = SyntaxToken.Empty.ToNativeToken(),
				item8 = SyntaxToken.Empty.ToNativeToken()
			};

			Assert.True(nativeLine1 != nativeLine2);

		}

		[Fact]
		public void NativeToken_NotEqualsOperator2_NativeToken()
		{

			NativeToken nativeToken1 = new()
			{
				kind = 254,
				startIndex = -26,
				endIndex = -1
			};

			NativeToken nativeToken2 = new()
			{
				kind = 254,
				startIndex = -26,
				endIndex = -1
			};

			Assert.False(nativeToken1 != nativeToken2);

		}

		[Fact]
		public void NativeToken_NotEqualsOperator_NativeToken()
		{

			NativeToken nativeToken1 = new()
			{
				kind = 24,
				startIndex = 62,
				endIndex = -1
			};

			NativeToken nativeToken2 = new()
			{
				kind = 254,
				startIndex = -26,
				endIndex = 1
			};

			Assert.True(nativeToken1 != nativeToken2);

		}

		[Fact]
		public unsafe void Ext_PtrToLine_ConvertsCorrectly()
		{

			NativeLine nativeLine = new()
			{
				item1 = new() { kind = 012, startIndex = 02, endIndex = 038 },
				item2 = new() { kind = 006, startIndex = 01, endIndex = 007 },
				item3 = new() { kind = 004, startIndex = -8, endIndex = -01 },
				item4 = new() { kind = 254, startIndex = 18, endIndex = 025 },
				item5 = new() { kind = 017, startIndex = 21, endIndex = 150 },
				item6 = SyntaxToken.Empty.ToNativeToken(),
				item7 = SyntaxToken.Empty.ToNativeToken(),
				item8 = SyntaxToken.Empty.ToNativeToken()
			};

			NativeLine* nativeLinePtr = &nativeLine;
			
			var managedLine = SyntaxExtensions.PtrToLine(nativeLinePtr);
			Assert.True(nativeLine.Equals(managedLine));

		}

		[Fact]
		public unsafe void Ext_PtrToLine_SameAsDereferencedConversion()
		{

			NativeLine nativeLine = new()
			{
				item1 = new() { kind = 012, startIndex = 02, endIndex = 038 },
				item2 = new() { kind = 006, startIndex = 01, endIndex = 007 },
				item3 = new() { kind = 004, startIndex = -8, endIndex = -01 },
				item4 = new() { kind = 254, startIndex = 18, endIndex = 025 },
				item5 = new() { kind = 017, startIndex = 21, endIndex = 150 },
				item6 = SyntaxToken.Empty.ToNativeToken(),
				item7 = SyntaxToken.Empty.ToNativeToken(),
				item8 = SyntaxToken.Empty.ToNativeToken()
			};

			NativeLine* nativeLinePtr = &nativeLine;

			var managedLine1 = SyntaxExtensions.PtrToLine(nativeLinePtr);
			var managedLine2 = (*nativeLinePtr).ToLine();

			Assert.True(managedLine1.Equals(managedLine2));

		}

		[Fact]
		public unsafe void Ext_CopyToNative_CopiesCorrectly()
		{

			SyntaxLine managedLine = new(
				new(TokenKind.LogicPathValue, 2, 38),
				new(TokenKind.EqualTo, 1, 7),
				new(TokenKind.ArchitectureIdentifier, ^8, ^1),
				new(TokenKind.Eol, 18, 25),
				new(TokenKind.SpecialActionArgument, 21, 150)
			);

			NativeLine output = new()
			{
				item1 = SyntaxToken.Empty.ToNativeToken(),
				item2 = SyntaxToken.Empty.ToNativeToken(),
				item3 = SyntaxToken.Empty.ToNativeToken(),
				item4 = SyntaxToken.Empty.ToNativeToken(),
				item5 = SyntaxToken.Empty.ToNativeToken(),
				item6 = SyntaxToken.Empty.ToNativeToken(),
				item7 = SyntaxToken.Empty.ToNativeToken(),
				item8 = SyntaxToken.Empty.ToNativeToken()
			};

			NativeLine* nativeLinePtr = &output;

			managedLine.CopyToNative(nativeLinePtr);
			Assert.True(output.Equals(managedLine));

		}

	}

}
