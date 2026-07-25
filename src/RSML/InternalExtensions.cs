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

	extension(ReadOnlySpan<char> span)
	{
		public unsafe bool IsAsciiEqualsIgnoreCase(ReadOnlySpan<char> str)
		{
			if (span.Length != str.Length)
				return false;

			fixed (char* spanPtr = &MemoryMarshal.GetReference(span))
			fixed (char* strPtr = &MemoryMarshal.GetReference(str))
			{
				char* ptrToSpan = spanPtr;
				char* ptrToStr = strPtr;
				int len = span.Length;

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

		public int GetHashCodeForSpan()
		{
			var hashCode = new HashCode();

			foreach (var @char in span)
				hashCode.Add(@char);

			return hashCode.ToHashCode();
		}
	}

	extension(ReadOnlySpan<int> integers)
	{
#if !NETCOREAPP3_0_OR_GREATER
		public bool Contains(int item) => integers.IndexOf(item) != -1;
#endif

		public int GetHashCodeForSpan()
		{
			var hashCode = new HashCode();

			foreach (var integer in integers)
				hashCode.Add(integer);

			return hashCode.ToHashCode();
		}
	}

	extension(ArgumentNullException)
	{
#if !NETCOREAPP3_0_OR_GREATER
		public static void ThrowIfNull(object? obj, string? paramName = null)
		{
			if (obj is null)
				throw new ArgumentNullException(paramName);
		}
#endif
	}
}
