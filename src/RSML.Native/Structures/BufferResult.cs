using System.Runtime.InteropServices;

using JetBrains.Annotations;


namespace OceanApocalypseStudios.RSML.Native.Structures
{

	/// <summary>
	/// A native function return value that contains
	/// a buffer and the amount of bytes in it.
	/// </summary>
	[NoReorder]
	[StructLayout(LayoutKind.Sequential)]
	public readonly unsafe struct BufferResult(byte* data, int byteCount)
	{

		/// <summary>
		/// The buffer to return.
		/// </summary>
		public readonly byte* buffer = data;

		/// <summary>
		/// The amount of bytes returned.
		/// </summary>
		public readonly int byteCount = byteCount;

	}

}
