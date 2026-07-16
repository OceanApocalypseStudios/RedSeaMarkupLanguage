using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace OceanApocalypseStudios.RSML;

internal static class InternalExtensions
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

	extension(ReadOnlySpan<char> chars)
	{
		public unsafe bool IsAsciiEqualsIgnoreCase(ReadOnlySpan<char> str)
		{
			if (chars.Length != str.Length)
				return false;

			fixed (char* spanPtr = &MemoryMarshal.GetReference(chars))
			fixed (char* strPtr = &MemoryMarshal.GetReference(str))
			{
				char* ptrToSpan = spanPtr;
				char* ptrToStr = strPtr;
				int len = chars.Length;

				for (int i = 0; i < len; i++)
				{
					char spanChar = *ptrToSpan++;
					char strChar = *ptrToStr++;

					if (!spanChar.IsAsciiEqualsIgnoreCase(strChar))
						return false;
				}
			}

			return true;
		}
	}
}
