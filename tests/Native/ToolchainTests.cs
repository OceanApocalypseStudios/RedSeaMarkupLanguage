using System;
using System.Runtime.InteropServices;
using System.Text;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Native;
using OceanApocalypseStudios.RSML.Native.Structures;


namespace OceanApocalypseStudios.RSML.Tests.Native
{

	public unsafe class ToolchainTests
	{

		#region rsml_alloc_buffer

		[Theory]
		[InlineData("-> windows != 10 defined \"Result A\"\r\nthis is random content", "-> windows != 10 defined \"Result A\"")]
		[InlineData("this is random buffer content because why not\r\n-> archlinux defined x86 \"Result C\"", "this is random buffer content because why not")]
		[InlineData("-> ubuntu >= 2 any \"Result B\"\r\ngoodbye\r\nhello", "-> ubuntu >= 2 any \"Result B\"")]
		[InlineData("There's only one way this game can end", null)]
		[InlineData("\r\nGoodbye!!", "")]
		[InlineData("hey\nbye\n", "hey")]
		public void AllocRsml_AllocatesBufferCorrectly_ReadLine(string content, string? firstLine)
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
		[InlineData("", 0, -1)]
		[InlineData(null, 0, -1)]
		[InlineData("-> ubuntu >= 2 any \"Result B\"")]
		[InlineData("-> archlinux defined x86 \"Result C\"", -5, -2)]
		public void AllocRsml_ThrowsErrorCodesCorrectly(string? content, int byteCount = 0, int errorCode = 0)
		{

			var callback = (delegate* unmanaged[Cdecl]<byte*, int, int>)&ToolchainExports.AllocRsmlBuffer;

			fixed (byte* data = Encoding.Default.GetBytes(content ?? ""))
			{

				int actualByteCount = byteCount == 0 ? Encoding.Default.GetByteCount(content ?? "") : byteCount;
				var outputErrorCode = callback(content is null ? null : data, actualByteCount);

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
			{
				Assert.Equal(-2, alloc(data, -2));
			}

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

		#endregion

		#region rsml_get_last_error_message

		[Fact]
		public void GetLastErrorMessage_WorksCorrectly()
		{

			var allocCallback = (delegate* unmanaged[Cdecl]<byte*, int, int>)&ToolchainExports.AllocRsmlBuffer;
			var errorCallback = (delegate* unmanaged[Cdecl]<nint>)&ToolchainExports.GetLastErrorMessage;

			Assert.NotEqual(0, allocCallback(null, -4));

			Assert.Equal(ToolchainExports.lastErrorMessage, errorCallback());
			Assert.Equal(Marshal.PtrToStringAuto(ToolchainExports.lastErrorMessage), Marshal.PtrToStringAuto(errorCallback()));

			if (ToolchainExports.lastErrorMessage != IntPtr.Zero)
			{

				Marshal.FreeHGlobal(ToolchainExports.lastErrorMessage); // cleanup
				ToolchainExports.lastErrorMessage = IntPtr.Zero;

			}

		}

		#endregion

		#region rsml_cleanup

		[Fact]
		public void Cleanup_WorksCorrectly()
		{

			var alloc = (delegate* unmanaged[Cdecl]<byte*, int, int>)&ToolchainExports.AllocRsmlBuffer;
			var cleanup = (delegate* unmanaged[Cdecl]<int>)&ToolchainExports.Cleanup;

			Assert.NotEqual(0, alloc(null, -4)); // allocate errors out here btw
			Assert.NotEqual(IntPtr.Zero, ToolchainExports.lastErrorMessage);
			Assert.Equal(0, cleanup());
			Assert.Equal(IntPtr.Zero, ToolchainExports.lastErrorMessage);

		}

		#endregion

		#region rsml_tokenize_rsml

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

		#endregion

	}

}
