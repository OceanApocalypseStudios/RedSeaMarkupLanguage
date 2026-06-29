using System;
using System.Text;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
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
		public void TokenizeRsml_SameAsManagedTokenizer(string data)
		{

			fixed (byte* buffer = Encoding.Default.GetBytes(data))
			{

				allocate(buffer, Encoding.Default.GetByteCount(data));

				NativeLine output = NativeLine.Empty;

				var outputPtr = &output;

				int errorCode = tokenize((nint)outputPtr);
				var managedLine = NativeLine.PointerToLine(outputPtr);
				var managedTokens = Lexer.TokenizeLine(new(data));

				Assert.AreEqual(0, errorCode);
				Assert.IsTrue(managedLine == managedTokens);

			}

			cleanup();

		}

		[TestMethod]
		[DataRow("# Just an innocent comment", -1)]
		[DataRow(null, -2)]
		[DataRow("", -2)]
		[DataRow("!> windows linux defined any x64 x86 arm64 \"Failure\"\r\n", -5)]
		public void TokenizeRsml_ThrowsErrorCodeCorrectly(
			string? data,
			int errorCode
		)
		{

			// simulate no-alloc (cuz test)
			if (data is null && errorCode == -2)
			{

				NativeLine output = NativeLine.Empty;

				var outputPtr = &output;

				int actualErrorCode = tokenize((nint)outputPtr);
				Assert.AreEqual(errorCode, actualErrorCode);

				return;

			}

			fixed (byte* buffer = Encoding.Default.GetBytes(data!))
			{

				allocate(buffer, Encoding.Default.GetByteCount(data!));

				NativeLine output = NativeLine.Empty;

				var outputPtr = &output;

				// if error code is -1 simulate a nullptr (IntPtr.Zero) - cuz test
				int actualErrorCode = tokenize(
					errorCode == -1
						? IntPtr.Zero
						: (nint)outputPtr
				);

				Assert.AreEqual(errorCode, actualErrorCode);

			}

			cleanup();

		}

	}

}
