using System.Runtime.InteropServices;


namespace OceanApocalypseStudios.RSML.Native.Structures
{

	/// <summary>
	/// A native-friendly line of RSML containing at most 8 tokens.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
    public struct NativeRsmlLine
    {

		/// <summary>
		/// Token 1.
		/// </summary>
		public NativeRsmlToken item1;

		/// <summary>
		/// Token 2.
		/// </summary>
		public NativeRsmlToken item2;

		/// <summary>
		/// Token 3.
		/// </summary>
		public NativeRsmlToken item3;

		/// <summary>
		/// Token 4.
		/// </summary>
		public NativeRsmlToken item4;

		/// <summary>
		/// Token 5.
		/// </summary>
		public NativeRsmlToken item5;

		/// <summary>
		/// Token 6.
		/// </summary>
		public NativeRsmlToken item6;

		/// <summary>
		/// Token 7.
		/// </summary>
		public NativeRsmlToken item7;

		/// <summary>
		/// Token 8.
		/// </summary>
		public NativeRsmlToken item8;

	}

}
