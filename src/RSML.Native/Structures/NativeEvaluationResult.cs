using System.Runtime.InteropServices;


namespace OceanApocalypseStudios.RSML.Native.Structures
{

	[StructLayout(LayoutKind.Sequential)]
	public struct NativeEvaluationResult
	{

		public byte wasMatchFound;

		public int matchValueStart;

		public int matchValueEnd;

	}

}
