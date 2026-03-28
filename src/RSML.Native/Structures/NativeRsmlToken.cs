using System.Runtime.InteropServices;


namespace OceanApocalypseStudios.RSML.Native.Structures
{

	/// <summary>
	/// A native-friendly RSML token.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct NativeRsmlToken
    {

		/// <summary>
		/// The kind of token.
		/// </summary>
		public byte kind;

		/// <summary>
		/// The index of the buffer at which the occurence starts.
		/// </summary>
		public int startIndex;

		/// <summary>
		/// The index of the buffer at which the occurence ends.
		/// </summary>
		public int endIndex;

    }

}
