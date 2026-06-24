using System.Text;

using OceanApocalypseStudios.RSML.Analyzer.Semantics;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Native;
using OceanApocalypseStudios.RSML.Native.Structures;


namespace OceanApocalypseStudios.RSML.Tests
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
		public void NormalizeRsml_SameAsManagedNormalizer(string data)
		{

			var alloc = (delegate* unmanaged[Cdecl]<byte*, int, int>)&ToolchainExports.AllocRsmlBuffer;
			var tokenize = (delegate* unmanaged[Cdecl]<nint, int>)&ToolchainExports.TokenizeRsmlLine;
			var normalize = (delegate* unmanaged[Cdecl]<nint, nint, int>)&ToolchainExports.NormalizeRsmlLine;
			var cleanup = (delegate* unmanaged[Cdecl]<int>)&ToolchainExports.Cleanup;

			fixed (byte* buffer = Encoding.Default.GetBytes(data))
			{

				#region Setup

				alloc(buffer, Encoding.Default.GetByteCount(data));

				NativeLine nativeLine = new()
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

				NativeLine normalizedLine = new()
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

				var nativeLinePtr = &nativeLine;
				var normalizedLinePtr = &normalizedLine;

				tokenize((nint)nativeLinePtr);

				#endregion

				// normalization happens here
				int errorCode = normalize((nint)nativeLinePtr, (nint)normalizedLinePtr);
				Assert.Equal(0, errorCode);
				var convertedNormalizedLine = SyntaxExtensions.PtrToLine(normalizedLinePtr);

				// managed normalization happens here - to be compared
				var managedNormalizedLine = Lexer.TokenizeLine(new(data));
				Normalizer.NormalizeLine(ref managedNormalizedLine, out int tokenCount);

				Assert.False(tokenCount > 8); // no more than 8 tokens
				Assert.True(convertedNormalizedLine == managedNormalizedLine);

			}

			cleanup();

		}

	}

}
