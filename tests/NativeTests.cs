using System;
using System.Runtime.InteropServices;

using OceanApocalypseStudios.RSML.Native;
using OceanApocalypseStudios.RSML.Native.Structures;


namespace OceanApocalypseStudios.RSML.Tests
{

	public unsafe partial class NativeTests
	{

		/// <inheritdoc cref="Exports.AllocRsmlBuffer(Byte*, Int32)" />
		private static readonly delegate* unmanaged[Cdecl]<byte*, int, int> allocate = (delegate* unmanaged[Cdecl]<byte*, int, int>)&Exports.AllocRsmlBuffer;

		/// <inheritdoc cref="Exports.TokenizeRsmlLine(IntPtr)" />
		private static readonly delegate* unmanaged[Cdecl]<nint, int> tokenize = (delegate* unmanaged[Cdecl]<nint, int>)&Exports.TokenizeRsmlLine;

		/// <inheritdoc cref="Exports.NormalizeRsmlLine(IntPtr, IntPtr)"/>
		private static readonly delegate* unmanaged[Cdecl]<nint, nint, int> normalize = (delegate* unmanaged[Cdecl]<nint, nint, int>)&Exports.NormalizeRsmlLine;

		/// <inheritdoc cref="Exports.ValidateRsmlLine(IntPtr)"/>
		private static readonly delegate* unmanaged[Cdecl]<nint, int> validate = (delegate* unmanaged[Cdecl]<nint, int>)&Exports.ValidateRsmlLine;

		/// <inheritdoc cref="Exports.EvaluateRsmlDocument(IntPtr, Int32, Int32, Int32, Int32, Byte)"/>
		private static readonly delegate* unmanaged[Cdecl]<nint, int, int, int, int, byte, int> evaluate = (delegate* unmanaged[Cdecl]<nint, int, int, int, int, byte, int>)&Exports.EvaluateRsmlDocument;

		/// <inheritdoc cref="Exports.Cleanup"/>
		private static readonly delegate* unmanaged[Cdecl]<int> cleanup = (delegate* unmanaged[Cdecl]<int>)&Exports.Cleanup;

		/// <inheritdoc cref="Exports.GetLastErrorMessage"/>
		private static readonly delegate* unmanaged[Cdecl]<nint> getError = (delegate* unmanaged[Cdecl]<nint>)&Exports.GetLastErrorMessage;

		/// <inheritdoc cref="Exports.GetApiAuthorName"/>
		private static readonly delegate* unmanaged[Cdecl]<StringResult> getApiAuthor = (delegate* unmanaged[Cdecl]<StringResult>)&Exports.GetApiAuthorName;

		/// <inheritdoc cref="Exports.GetApiDocumentationUrl"/>
		private static readonly delegate* unmanaged[Cdecl]<StringResult> getDocsUrl = (delegate* unmanaged[Cdecl]<StringResult>)&Exports.GetApiDocumentationUrl;

		/// <inheritdoc cref="Exports.GetApiVersion"/>
		private static readonly delegate* unmanaged[Cdecl]<StringResult> getApiVersion = (delegate* unmanaged[Cdecl]<StringResult>)&Exports.GetApiVersion;

		[TestMethod]
		public void Cleanup_WorksCorrectly()
		{

			Assert.AreNotEqual(0, allocate(null, -4)); // allocate errors out here btw
			Assert.AreNotEqual(IntPtr.Zero, Exports.lastErrorMessage);
			Assert.AreEqual(0, cleanup());
			Assert.AreEqual(IntPtr.Zero, Exports.lastErrorMessage);

		}

		[TestMethod]
		public void GetLastErrorMessage_WorksCorrectly()
		{

			// please error out (first time im begging for an error)
			Assert.AreNotEqual(0, allocate(null, -4));

			// null pointer assertions
			Assert.AreNotEqual(IntPtr.Zero, Exports.lastErrorMessage);
			Assert.AreNotEqual(IntPtr.Zero, getError());

			// they point to the same mfing thing
			Assert.AreEqual(Exports.lastErrorMessage, getError());

			var lastErrorMessageStr = Marshal.PtrToStringAuto(Exports.lastErrorMessage);
			var getErrorStr = Marshal.PtrToStringAuto(getError());

			// still null pointer assertions
			Assert.IsNotNull(lastErrorMessageStr);
			Assert.IsNotNull(getErrorStr);

			// twins lmao
			Assert.AreEqual(lastErrorMessageStr, getErrorStr);

			cleanup();

		}

	}

}
