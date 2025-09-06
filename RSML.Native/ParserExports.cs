using System;
using System.Runtime.InteropServices;


namespace OceanApocalypseStudios.RSML.Native
{

	public static class ParserExports
	{

		[UnmanagedCallersOnly(EntryPoint = "rsml_create_parser")]
		public static void CreateRsmlParser() => throw new NotImplementedException("todo");

	}

}
