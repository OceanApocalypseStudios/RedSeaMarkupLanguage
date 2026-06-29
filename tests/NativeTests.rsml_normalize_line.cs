using System.Text;

using OceanApocalypseStudios.RSML.Analyzer.Semantics;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Native.Structures;


namespace OceanApocalypseStudios.RSML.Tests
{

	[TestClass]
	public unsafe partial class NativeTests
	{

		[TestMethod]
		[DataRow("-> defined == 11 any \"Test A\"")]
		[DataRow("-> any != 11 arm64 \"Test B\"")]
		[DataRow("# Test C\r\n@Void")]
		[DataRow("@Void \"Test D\"")]
		[DataRow("!> windows >= 10 arm64 \"Test E\"")]
		[DataRow("@ThrowError \"Test F1\"\n-> windows <= 7 x86 \"Test F2\"\r\n")]
		[DataRow("!> ubuntu any x86 \"Test G\"")]
		[DataRow("-> archlinux defined x64 \"Test H\"")]
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
				Assert.AreEqual(0, errorCode);
				var convertedNormalizedLine = NativeLine.PointerToLine(normalizedLinePtr);

				// managed normalization happens here - to be compared
				var managedNormalizedLine = Lexer.TokenizeLine(new(data));
				Normalizer.NormalizeLine(ref managedNormalizedLine, out int tokenCount);

				Assert.IsLessThanOrEqualTo(8, tokenCount); // no more than 8 tokens
				Assert.IsTrue(convertedNormalizedLine == managedNormalizedLine);

			}

			cleanup();

		}

	}

}
