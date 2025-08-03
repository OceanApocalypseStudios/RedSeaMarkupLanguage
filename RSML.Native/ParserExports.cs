using System;
using System.Runtime.InteropServices;

using RSML.Parser;


namespace RSML.Native
{

	/// <summary>
	/// Exports for <see cref="RsParser " />.
	/// </summary>
	public static class ParserExports
	{

		[UnmanagedCallersOnly(EntryPoint = "rsml_create_parser")]
		public static void CreateRsmlParser() => throw new NotImplementedException("todo");

	}

}
