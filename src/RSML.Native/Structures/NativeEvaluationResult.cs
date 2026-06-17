using System.Runtime.InteropServices;


namespace OceanApocalypseStudios.RSML.Native.Structures
{

	/// <summary>
	/// The result of a native evaluation process.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct NativeEvaluationResult
	{

		/// <summary>
		/// Whether a match was found. Boolean.
		/// </summary>
		public byte wasMatchFound;

		/// <summary>
		/// The start index for the match value.
		/// </summary>
		public int matchValueStart;

		/// <summary>
		/// The end index for the match value.
		/// </summary>
		public int matchValueEnd;

	}

}
