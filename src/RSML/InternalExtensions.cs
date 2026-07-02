using System;


namespace OceanApocalypseStudios.RSML
{

	internal static class InternalExtensions
	{

		public static bool IsNewline(this char character) => character is '\r' or '\n' or '\u2028' or '\u2029';

		public static bool IsWhiteSpace(this char character) => Char.IsWhiteSpace(character);;

	}

}
