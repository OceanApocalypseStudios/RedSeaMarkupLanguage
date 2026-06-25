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

			var callback = (delegate* unmanaged[Cdecl]<byte*, int, int>)&ToolchainExports.AllocRsmlBuffer;

			fixed (byte* data = Encoding.Default.GetBytes(content))
			{

				int byteCount = Encoding.Default.GetByteCount(content);

				Assert.Equal(0, callback(data, byteCount));

				Assert.NotNull(ToolchainExports.buffer);
				Assert.Equal(0, ToolchainExports.buffer.CaretPosition);

				Assert.Equal(content, ToolchainExports.buffer.ReadUntil((_, _) => false).ToString());

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

			var callback = (delegate* unmanaged[Cdecl]<byte*, int, int>)&ToolchainExports.AllocRsmlBuffer;

			fixed (byte* data = Encoding.Default.GetBytes(content))
			{

				int byteCount = Encoding.Default.GetByteCount(content);

				Assert.Equal(0, callback(data, byteCount));

				Assert.NotNull(ToolchainExports.buffer);
				Assert.Equal(0, ToolchainExports.buffer.CaretPosition);

				Assert.Equal(firstLine ?? content, ToolchainExports.buffer.ReadLine().ToString());

			}

		}

		[Fact]
		public void AllocRsml_CleansLastErrorBeforeAssigning()
		{

			var alloc = (delegate* unmanaged[Cdecl]<byte*, int, int>)&ToolchainExports.AllocRsmlBuffer;

			Assert.Equal(-1, alloc(null, 2));
			nint firstErrorMessagePtr = ToolchainExports.lastErrorMessage;
			Assert.NotEqual(IntPtr.Zero, firstErrorMessagePtr);
			string? firstErrorMessage = Marshal.PtrToStringAuto(firstErrorMessagePtr);
			Assert.NotNull(firstErrorMessage);

			fixed (byte* data = Encoding.Default.GetBytes("Random content for testing purposes"))
				Assert.Equal(-2, alloc(data, -2));

			nint secondErrorMessagePtr = ToolchainExports.lastErrorMessage;
			Assert.NotEqual(IntPtr.Zero, secondErrorMessagePtr);
			string? secondErrorMessage = Marshal.PtrToStringAuto(secondErrorMessagePtr);
			Assert.NotNull(secondErrorMessage);

			Assert.NotEqual(firstErrorMessage, secondErrorMessage);

			if (ToolchainExports.lastErrorMessage != IntPtr.Zero)
			{

				Marshal.FreeHGlobal(ToolchainExports.lastErrorMessage); // cleanup
				ToolchainExports.lastErrorMessage = IntPtr.Zero;

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

			var callback = (delegate* unmanaged[Cdecl]<byte*, int, int>)&ToolchainExports.AllocRsmlBuffer;

			fixed (byte* data = Encoding.Default.GetBytes(content ?? ""))
			{

				int actualByteCount = byteCount == 0
										  ? Encoding.Default.GetByteCount(content ?? "")
										  : byteCount;

				int outputErrorCode = callback(
					content is null
						? null
						: data,
					actualByteCount
				);

				Assert.Equal(errorCode, outputErrorCode);

				if (errorCode != 0)
					Assert.NotEqual(0, ToolchainExports.lastErrorMessage);
				else
					Assert.Equal(0, ToolchainExports.lastErrorMessage);

				if (ToolchainExports.lastErrorMessage != IntPtr.Zero)
				{

					Marshal.FreeHGlobal(ToolchainExports.lastErrorMessage); // cleanup
					ToolchainExports.lastErrorMessage = IntPtr.Zero;

				}

			}

		}

	}

}
