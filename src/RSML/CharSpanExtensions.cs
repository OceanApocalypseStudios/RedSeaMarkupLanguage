using System;
using System.Runtime.InteropServices;


namespace OceanApocalypseStudios.RSML;

internal static class CharSpanExtensions
{
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

		public char GetCharAt(int index)
		{
			if (index < 0)
				index += chars.Length;

			if (index >= chars.Length)
				return '\0';

			return chars[index];
		}
	}

	public static char GetCharAt(this Span<char> span, int index)
	{
		if (index < 0)
			index += span.Length;

		if (index >= span.Length)
			return '\0';

		return span[index];
	}

	public static char GetCharAt(this string span, int index)
	{
		if (index < 0)
			index += span.Length;

		if (index >= span.Length)
			return '\0';

		return span[index];
	}

	public static char GetCharAt(this char[] array, int index)
	{
		if (index < 0)
			index += array.Length;

		if (index >= array.Length)
			return '\0';

		return array[index];
	}
}
