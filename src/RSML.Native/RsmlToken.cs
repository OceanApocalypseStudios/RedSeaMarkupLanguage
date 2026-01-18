using System.Runtime.InteropServices;


namespace OceanApocalypseStudios.RSML.Native
{

	[StructLayout(LayoutKind.Sequential)]
	public struct RsmlToken
    {

		public byte kind;
		public int startIndex;
		public int endIndex;

    }

}
