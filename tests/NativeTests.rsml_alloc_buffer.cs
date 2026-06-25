using System;
using System.Runtime.InteropServices;
using System.Text;

using OceanApocalypseStudios.RSML.Native;


namespace OceanApocalypseStudios.RSML.Tests
{

	public unsafe partial class NativeTests
	{

		[Theory]
		[InlineData("-> windows != 10 defined \"Result A\"")]
		[InlineData("this is random content")]
		[InlineData("-> ubuntu >= 2 any \"Result B\"")]
		[InlineData("-> archlinux defined x86 \"Result C\"")]
		[InlineData("this is random buffer content because why not")]
		[InlineData("hello\n\ngoodbye")]
		[InlineData("this is yet\n\r\nanother weird teststring")]
		[InlineData("-> ubuntu >= 2 any \"Result B\"\r\ngoodbye\r\nhello")]
		public void AllocRsml_AllocatesBufferCorrectly_ReadAll(string content)
		{

			fixed (byte* data = Encoding.Default.GetBytes(content))
			{

				int byteCount = Encoding.Default.GetByteCount(content);

				Assert.Equal(0, allocate(data, byteCount));

				Assert.NotNull(Exports.buffer);
				Assert.Equal(0, Exports.buffer.CaretPosition);

				Assert.Equal(content, Exports.buffer.ReadUntil((_, _) => false).ToString());

			}

		}

		[Theory]
		[InlineData("-> windows != 10 defined \"Result A\"\r\nthis is random content", "-> windows != 10 defined \"Result A\"")]
		[InlineData("this is random buffer content because why not\r\n-> archlinux defined x86 \"Result C\"", "this is random buffer content because why not")]
		[InlineData("-> ubuntu >= 2 any \"Result B\"\r\ngoodbye\r\nhello", "-> ubuntu >= 2 any \"Result B\"")]
		[InlineData("There's only one way this game can end", null)]
		[InlineData("\r\nGoodbye!!", "")]
		[InlineData("hey\nbye\n", "hey")]
		public void AllocRsml_AllocatesBufferCorrectly_ReadLine(
			string content,
			string? firstLine
		)
		{

			int byteCount = Encoding.Default.GetByteCount(content);

			fixed (byte* data = Encoding.Default.GetBytes(content))
			{


				Assert.Equal(0, allocate(data, byteCount));

				Assert.NotNull(Exports.buffer);
				Assert.Equal(0, Exports.buffer.CaretPosition);

				Assert.Equal(firstLine ?? content, Exports.buffer.ReadLine().ToString());

			}

		}

		[Fact]
		public void AllocRsml_CleansLastErrorBeforeAssigning()
		{

			Assert.Equal(-1, allocate(null, 2));
			nint firstErrorMessagePtr = Exports.lastErrorMessage;
			Assert.NotEqual(IntPtr.Zero, firstErrorMessagePtr);
			string? firstErrorMessage = Marshal.PtrToStringAuto(firstErrorMessagePtr);
			Assert.NotNull(firstErrorMessage);

			fixed (byte* data = Encoding.Default.GetBytes("Random content for testing purposes"))
				Assert.Equal(-2, allocate(data, -2));

			nint secondErrorMessagePtr = Exports.lastErrorMessage;
			Assert.NotEqual(IntPtr.Zero, secondErrorMessagePtr);
			string? secondErrorMessage = Marshal.PtrToStringAuto(secondErrorMessagePtr);
			Assert.NotNull(secondErrorMessage);

			Assert.NotEqual(firstErrorMessage, secondErrorMessage);

			if (Exports.lastErrorMessage != IntPtr.Zero)
			{

				Marshal.FreeHGlobal(Exports.lastErrorMessage); // cleanup
				Exports.lastErrorMessage = IntPtr.Zero;

			}

		}

		[Theory]
		[InlineData("", 0, -1)]
		[InlineData(null, 0, -1)]
		[InlineData("-> ubuntu >= 2 any \"Result B\"")]
		[InlineData("-> archlinux defined x86 \"Result C\"", -5, -2)]
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

				Assert.Equal(errorCode, outputErrorCode);

				if (errorCode != 0)
					Assert.NotEqual(0, Exports.lastErrorMessage);
				else
					Assert.Equal(0, Exports.lastErrorMessage);

				if (Exports.lastErrorMessage != IntPtr.Zero)
				{

					Marshal.FreeHGlobal(Exports.lastErrorMessage); // cleanup
					Exports.lastErrorMessage = IntPtr.Zero;

				}

			}

		}

	}

}
