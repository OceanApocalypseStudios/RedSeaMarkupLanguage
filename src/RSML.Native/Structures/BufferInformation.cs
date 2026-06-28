using System.Runtime.InteropServices;

using JetBrains.Annotations;


namespace OceanApocalypseStudios.RSML.Native.Structures
{

	/// <summary>
	/// A native function return type that contains
	/// information about RSML's internally
	/// used buffer.
	/// </summary>
	[NoReorder]
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct BufferInformation(int index, int length)
	{

		/// <summary>
		/// A 0-based index pointing directly to the caret.
		/// </summary>
		public readonly int index = index;

		/// <summary>
		/// A 0-based index pointing directly to the caret.
		/// </summary>
		public readonly int length = length;

	}

}
