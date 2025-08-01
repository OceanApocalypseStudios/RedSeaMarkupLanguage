using System;
using System.Runtime.InteropServices;

using RSML.Parser;


namespace RSML.Native
{

	/// <summary>
	/// Exports for <see cref="RSParser "/>.
	/// </summary>
	public unsafe static class ParserExports
	{

		[UnmanagedCallersOnly(EntryPoint = "rsml_create_parser")]
		public static void CreateRsmlParser() => throw new NotImplementedException("todo");

	}

}
