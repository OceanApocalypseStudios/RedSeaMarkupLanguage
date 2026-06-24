using System;
using System.Runtime.InteropServices;

using OceanApocalypseStudios.RSML.Native;


namespace OceanApocalypseStudios.RSML.Tests
{

	public unsafe partial class NativeTests
	{

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

	}

}
