using System;
using System.Runtime.CompilerServices;


namespace OceanApocalypseStudios.RSML;

internal static class CharExtensions
{
	public const byte AsciiCaseBit = 0x20;

	extension(char character)
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsNewline() => character is '\r' or '\n' or '\u2028' or '\u2029';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsWhiteSpace() => character != '\0' && character <= 32 && Char.IsWhiteSpace(character);

		public bool IsAsciiEqualsIgnoreCase(char other)
		{
			character |= character is >= 'A' and <= 'Z'
							? (char)AsciiCaseBit
							: (char)0;

			other |= other is >= 'A' and <= 'Z'
						 ? (char)AsciiCaseBit
						 : (char)0;

			return character == other;
		}
	}
}
