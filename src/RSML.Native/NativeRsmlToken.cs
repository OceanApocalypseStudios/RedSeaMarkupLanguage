using System.Runtime.InteropServices;


namespace OceanApocalypseStudios.RSML.Native
{

	[StructLayout(LayoutKind.Sequential)]
	public struct NativeRsmlToken
    {

		public byte kind;
		public int startIndex;
		public int endIndex;

    }

}
