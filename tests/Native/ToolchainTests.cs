using System.Runtime.InteropServices;

using OceanApocalypseStudios.RSML.Native;


namespace OceanApocalypseStudios.RSML.Tests.Native
{

	public unsafe class ToolchainTests
	{

		[Theory]
		[InlineData("-> windows != 10 defined \"Result A\"\r\nthis is random content", "-> windows != 10 defined \"Result A\"")]
		[InlineData("this is random buffer content because why not\r\n-> archlinux defined x86 \"Result C\"", "this is random buffer content because why not")]
		[InlineData("-> ubuntu >= 2 any \"Result B\"\r\ngoodbye\r\nhello", "-> ubuntu >= 2 any \"Result B\"")]
		[InlineData("There's only one way this game can end", null)]
		[InlineData("\r\nGoodbye!!", "")]
		[InlineData("hey\nbye\n", "hey")]
		public void AllocRsml_AllocatesBufferCorrectly_ReadLine(string content, string? firstLine)
		{

			var callback = (delegate* unmanaged[Cdecl]<nint, int>)&ToolchainExports.AllocRsmlBuffer;
			var ptr = Marshal.StringToHGlobalAuto(content);

			Assert.Equal(0, callback(ptr));
			Marshal.FreeHGlobal(ptr);

			Assert.NotNull(ToolchainExports.buffer);
			Assert.Equal(0, ToolchainExports.buffer.CaretPosition);

			Assert.Equal(firstLine ?? content, ToolchainExports.buffer.ReadLine().ToString());

		}

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

			var callback = (delegate* unmanaged[Cdecl]<nint, int>)&ToolchainExports.AllocRsmlBuffer;
			var ptr = Marshal.StringToHGlobalAuto(content);

			Assert.Equal(0, callback(ptr));
			Marshal.FreeHGlobal(ptr);

			Assert.NotNull(ToolchainExports.buffer);
			Assert.Equal(0, ToolchainExports.buffer.CaretPosition);

			Assert.Equal(content, ToolchainExports.buffer.ReadUntil((_, _) => false).ToString());

		}

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

			var callback = (delegate* unmanaged[Cdecl]<nint, int>)&ToolchainExports.AllocRsmlBuffer;
			var ptr = Marshal.StringToHGlobalAuto(content);

			Assert.Equal(0, callback(ptr));
			Marshal.FreeHGlobal(ptr);

			Assert.NotNull(ToolchainExports.buffer);
			Assert.Equal(0, ToolchainExports.buffer.CaretPosition);

			Assert.Equal(content, ToolchainExports.buffer.ReadUntil((_, _) => false).ToString());

		}

	}

}
