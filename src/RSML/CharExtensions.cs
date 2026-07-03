using System;
using System.Runtime.CompilerServices;


namespace OceanApocalypseStudios.RSML
{

	internal static class CharExtensions
	{

		public const byte AsciiCaseBit = 0x20;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNewline(this char character) => character is '\r' or '\n' or '\u2028' or '\u2029';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsWhiteSpace(this char character) => character != '\0' && character <= 32 && Char.IsWhiteSpace(character);

		public static bool IsAsciiEqualsIgnoreCase(this char @this, char other)
		{

			@this |= @this is >= 'A' and <= 'Z'
				? (char)AsciiCaseBit
				: (char)0;

			other |= other is >= 'A' and <= 'Z'
				? (char)AsciiCaseBit
				: (char)0;

			return @this == other;

		}

	}

}
