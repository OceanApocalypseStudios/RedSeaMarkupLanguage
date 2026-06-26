using System.Text;

using OceanApocalypseStudios.RSML.Analyzer.Semantics;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
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

			fixed (byte* buffer = Encoding.Default.GetBytes(data))
			{

				allocate(buffer, Encoding.Default.GetByteCount(data));

				NativeLine nativeLine = NativeLine.Empty;
				NativeLine normalizedLine = NativeLine.Empty;

				var nativeLinePtr = &nativeLine;
				var normalizedLinePtr = &normalizedLine;

				tokenize((nint)nativeLinePtr);

				// normalization happens here
				int errorCode = normalize((nint)nativeLinePtr, (nint)normalizedLinePtr);
				Assert.Equal(0, errorCode);
				var convertedNormalizedLine = NativeLine.PointerToLine(normalizedLinePtr);

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
