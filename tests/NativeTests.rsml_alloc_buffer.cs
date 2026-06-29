using System.Runtime.InteropServices;
using System.Text;

using OceanApocalypseStudios.RSML.Native;


namespace OceanApocalypseStudios.RSML.Tests
{

	public unsafe partial class NativeTests
	{

		[TestMethod]
		[DataRow("-> windows != 10 defined \"Result A\"")]
		[DataRow("this is random content")]
		[DataRow("-> ubuntu >= 2 any \"Result B\"")]
		[DataRow("-> archlinux defined x86 \"Result C\"")]
		[DataRow("this is random buffer content because why not")]
		[DataRow("hello\n\ngoodbye")]
		[DataRow("this is yet\n\r\nanother weird teststring")]
		[DataRow("-> ubuntu >= 2 any \"Result B\"\r\ngoodbye\r\nhello")]
		public void AllocRsml_AllocatesBufferCorrectly_ReadAll(string content)
		{

			int byteCount = Encoding.Default.GetByteCount(content);

			fixed (byte* data = Encoding.Default.GetBytes(content))
			{

				Assert.AreEqual(0, allocate(data, byteCount));

				Assert.IsNotNull(Exports.buffer);
				Assert.AreEqual(0, Exports.buffer.CaretPosition);

				Assert.AreEqual(content, Exports.buffer.ReadUntil((_, _) => false).ToString());

			}

		}

		[TestMethod]
		[DataRow("-> windows != 10 defined \"Result A\"\r\nthis is random content", "-> windows != 10 defined \"Result A\"")]
		[DataRow("this is random buffer content because why not\r\n-> archlinux defined x86 \"Result C\"", "this is random buffer content because why not")]
		[DataRow("-> ubuntu >= 2 any \"Result B\"\r\ngoodbye\r\nhello", "-> ubuntu >= 2 any \"Result B\"")]
		[DataRow("There's only one way this game can end", null)]
		[DataRow("\r\nGoodbye!!", "")]
		[DataRow("hey\nbye\n", "hey")]
		public void AllocRsml_AllocatesBufferCorrectly_ReadLine(
			string content,
			string? firstLine
		)
		{

			int byteCount = Encoding.Default.GetByteCount(content);

			fixed (byte* data = Encoding.Default.GetBytes(content))
			{


				Assert.AreEqual(0, allocate(data, byteCount));

				Assert.IsNotNull(Exports.buffer);
				Assert.AreEqual(0, Exports.buffer.CaretPosition);

				Assert.AreEqual(firstLine ?? content, Exports.buffer.ReadLine().ToString());

			}

		}

		[TestMethod]
		public void AllocRsml_CleansLastErrorBeforeAssigning()
		{

			Assert.AreEqual(-1, allocate(null, 2));
			nint firstErrorMessagePtr = Exports.lastErrorMessage;
			Assert.AreNotEqual(IntPtr.Zero, firstErrorMessagePtr);
			string? firstErrorMessage = Marshal.PtrToStringAuto(firstErrorMessagePtr);
			Assert.IsNotNull(firstErrorMessage);

			fixed (byte* data = Encoding.Default.GetBytes("Random content for testing purposes"))
				Assert.AreEqual(-2, allocate(data, -2));

			nint secondErrorMessagePtr = Exports.lastErrorMessage;
			Assert.AreNotEqual(IntPtr.Zero, secondErrorMessagePtr);
			string? secondErrorMessage = Marshal.PtrToStringAuto(secondErrorMessagePtr);
			Assert.IsNotNull(secondErrorMessage);

			Assert.AreNotEqual(firstErrorMessage, secondErrorMessage);

			if (Exports.lastErrorMessage != IntPtr.Zero)
			{

				Marshal.FreeHGlobal(Exports.lastErrorMessage); // cleanup
				Exports.lastErrorMessage = IntPtr.Zero;

			}

		}

		[TestMethod]
		[DataRow("", 0, -1)]
		[DataRow(null, 0, -1)]
		[DataRow("-> ubuntu >= 2 any \"Result B\"")]
		[DataRow("-> archlinux defined x86 \"Result C\"", -5, -2)]
		public void AllocRsml_ThrowsErrorCodesCorrectly(
			string? content,
			int byteCount = 0,
			int errorCode = 0
		)
		{

			fixed (byte* data = Encoding.Default.GetBytes(content ?? ""))
			{

				int actualByteCount = byteCount == 0
										  ? Encoding.Default.GetByteCount(content ?? "")
										  : byteCount;

				int outputErrorCode = allocate(
					content is null
						? null
						: data,
					actualByteCount
				);

				Assert.AreEqual(errorCode, outputErrorCode);

				if (errorCode != 0)
					Assert.AreNotEqual(0, Exports.lastErrorMessage);
				else
					Assert.AreEqual(0, Exports.lastErrorMessage);

				if (Exports.lastErrorMessage != IntPtr.Zero)
				{

					Marshal.FreeHGlobal(Exports.lastErrorMessage); // cleanup
					Exports.lastErrorMessage = IntPtr.Zero;

				}

			}

		}

	}

}
