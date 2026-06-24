using System;
using System.Text;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Native;
using OceanApocalypseStudios.RSML.Native.Structures;

namespace OceanApocalypseStudios.RSML.Tests.Native
{

	public unsafe partial class NativeTests
	{

		[Theory]
		[InlineData("-> defined == 11 any \"Test A\"")]
		[InlineData("-> any != 11 arm64 \"Test B\"")]
		[InlineData("# Test C\r\n@Void")]
		[InlineData("@Void \"Test D\"")]
		[InlineData("!> windows >= 10 arm64 \"Test E\"")]
		[InlineData("@ThrowError \"Test F1\"\n-> windows <= 7 x86 \"Test F2\"\r\n")]
		[InlineData("!> ubuntu any x86 \"Test G\"")]
		[InlineData("-> archlinux defined x64 \"Test H\"")]
		public void TokenizeRsml_SameAsManagedTokenizer(string data)
		{

			var alloc = (delegate* unmanaged[Cdecl]<byte*, int, int>)&ToolchainExports.AllocRsmlBuffer;
			var tokenize = (delegate* unmanaged[Cdecl]<nint, int>)&ToolchainExports.TokenizeRsmlLine;
			var cleanup = (delegate* unmanaged[Cdecl]<int>)&ToolchainExports.Cleanup;

			fixed (byte* buffer = Encoding.Default.GetBytes(data))
			{

				alloc(buffer, Encoding.Default.GetByteCount(data));

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
				NativeLine* outputPtr = &output;

				var errorCode = tokenize((nint)outputPtr);
				var managedLine = SyntaxExtensions.PtrToLine(outputPtr);
				var managedTokens = Lexer.TokenizeLine(new(data));

				Assert.Equal(0, errorCode);
				Assert.True(managedLine == managedTokens);

			}

			cleanup();

		}

		[Theory]
		[InlineData("# Just an innocent comment", -1)]
		[InlineData(null, -2)]
		[InlineData("", -2)]
		[InlineData("!> windows linux defined any x64 x86 arm64 \"Failure\"\r\n", -5)]
		public void TokenizeRsml_ThrowsErrorCodeCorrectly(string? data, int errorCode)
		{

			var alloc = (delegate* unmanaged[Cdecl]<byte*, int, int>)&ToolchainExports.AllocRsmlBuffer;
			var tokenize = (delegate* unmanaged[Cdecl]<nint, int>)&ToolchainExports.TokenizeRsmlLine;
			var cleanup = (delegate* unmanaged[Cdecl]<int>)&ToolchainExports.Cleanup;

			// simulate no-alloc (cuz test)
			if (data is null && errorCode == -2)
			{

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
				NativeLine* outputPtr = &output;

				var actualErrorCode = tokenize((nint)outputPtr);
				Assert.Equal(errorCode, actualErrorCode);

				return;

			}

			fixed (byte* buffer = Encoding.Default.GetBytes(data!))
			{

				alloc(buffer, Encoding.Default.GetByteCount(data!));

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
				NativeLine* outputPtr = &output;

				// if error code is -1 simulate a nullptr (IntPtr.Zero) - cuz test
				var actualErrorCode = tokenize(errorCode == -1 ? IntPtr.Zero : ((nint)outputPtr));
				Assert.Equal(errorCode, actualErrorCode);

			}

			cleanup();

		}

	}

}
