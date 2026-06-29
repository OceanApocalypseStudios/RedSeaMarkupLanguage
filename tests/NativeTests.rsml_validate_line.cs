using System.Text;

using OceanApocalypseStudios.RSML.Analyzer.Semantics;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Native.Structures;


namespace OceanApocalypseStudios.RSML.Tests
{

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
		[DataRow("-> templeos any x69 \"Test I\"")] // invalid line
		public void ValidateRsml_SameAsManagedValidator(string data)
		{

			fixed (byte* buffer = Encoding.Default.GetBytes(data))
			{

				allocate(buffer, Encoding.Default.GetByteCount(data));

				NativeLine nativeLine = NativeLine.Empty;
				NativeLine normalizedLine = NativeLine.Empty;

				var nativeLinePtr = &nativeLine;
				var normalizedLinePtr = &normalizedLine;

				tokenize((nint)nativeLinePtr);
				normalize((nint)nativeLinePtr, (nint)normalizedLinePtr);

				// normalization happens here
				int errorCode = validate((nint)normalizedLinePtr);
				Assert.IsTrue(errorCode is 0 or 1); // success (valid or invalid lines)

				// managed normalization happens here - to be compared
				DualTextBuffer managedBuffer = new(data);
				var managedNormalizedLine = Lexer.TokenizeLine(managedBuffer);
				Normalizer.NormalizeLine(ref managedNormalizedLine, out _);
				bool isManagedLineValid = true;

				try
				{

					Validator.ValidateLine(managedNormalizedLine, managedBuffer);

				}
				catch (InvalidRsmlSyntax)
				{

					isManagedLineValid = false;

				}

				if (isManagedLineValid != (errorCode == 0))
					Assert.Fail("isManagedLineValid doesn't match boolean state of errorCode");

			}

			cleanup();

		}

	}

}
