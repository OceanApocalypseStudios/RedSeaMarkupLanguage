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
	}
}
