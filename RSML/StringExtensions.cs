using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace RSML
{

	internal static class StringExtensions
	{

		internal const byte AsciiCaseBit = 0x20;

		internal static unsafe bool IsNewLinesOnly(this ReadOnlySpan<char> chars)
		{

			int len = chars.Length;

			if (len == 0)
				return false; // technically the line does not have any newlines

			fixed (char* start = &MemoryMarshal.GetReference(chars))
			{

				char* pos = start;
				char* end = start + len;
				bool crDetected = false;

				while (pos < end)
				{

					char c = *pos++;

					if (c == '\r' && !crDetected)
					{

						crDetected = true;

						continue;

					}

					if (c != '\n')
						return false;

					crDetected = false;

				}

			}

			return true;

		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int CrLfCheck(ReadOnlySpan<char> chars, int index, out byte length)
		{

			if (index == -1)
			{

				length = 0;

				return -1;

			}

			if (index != 0 && chars[index - 1] == '\r')
			{

				length = 2;

				return index - 1;

			}

			length = 1;

			return index;

		}

		internal static int IndexOfNewline(this ReadOnlySpan<char> chars, out byte length)
		{

			int nextNewline = chars.IndexOf('\n');

			return CrLfCheck(chars, nextNewline, out length);

		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsEquals(this ReadOnlySpan<char> chars, string str) => chars.SequenceEqual(str);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsEquals(this ReadOnlyMemory<char> chars, string? str) => str is not null && chars.Span.IsEquals(str);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsEquals(this ReadOnlyMemory<char> chars, ReadOnlyMemory<char> memory) => chars.Span.SequenceEqual(memory.Span);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsEquals_8(
			this ReadOnlySpan<char> chars,
			string strA,
			string strB,
			string? strC = null,
			string? strD = null,
			string? strE = null,
			string? strF = null,
			string? strG = null,
			string? strH = null
		)
		{

			if (chars.IsEquals(strA))
				return true;

			if (chars.IsEquals(strB))
				return true;

			if (strC is not null && chars.IsEquals(strC))
				return true;

			if (strD is not null && chars.IsEquals(strD))
				return true;

			if (strE is not null && chars.IsEquals(strE))
				return true;

			if (strF is not null && chars.IsEquals(strF))
				return true;

			if (strG is not null && chars.IsEquals(strG))
				return true;

			return strH is not null && chars.IsEquals(strH);

		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static unsafe bool IsAsciiEqualsIgnoreCase(this ReadOnlySpan<char> chars, string str)
		{

			if (chars.Length != str.Length)
				return false;

			fixed (char* spanPtr = &MemoryMarshal.GetReference(chars))
			fixed (char* strPtr = str)
			{

				char* ptrToSpan = spanPtr;
				char* ptrToStr = strPtr;
				int len = chars.Length;

				for (int i = 0; i < len; i++)
				{

					char spanChar = *ptrToSpan++;
					char strChar = *ptrToStr++;

					spanChar |= spanChar is >= 'A' and <= 'Z' ? (char)AsciiCaseBit : (char)0;
					strChar |= strChar is >= 'A' and <= 'Z' ? (char)AsciiCaseBit : (char)0;

					if (spanChar != strChar)
						return false;

				}

			}

			return true;

		}

		/// <summary>
		/// Checks if a span of ASCII characters is equals to any of 10 ASCII-only strings, while ignoring case.
		/// Yes, the code might look weird, but it saves up a lot in performance.
		/// I call this method every single iteration of a loop.
		/// If I used params string[], that would allocate a whole new array every single time I call this method.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsAsciiEqualsIgnoreCase_10(
			this ReadOnlySpan<char> chars,
			string strA,
			string strB,
			string strC,
			string strD,
			string strE,
			string strF,
			string? strG = null,
			string? strH = null,
			string? strI = null,
			string? strJ = null
		)
		{

			if (chars.IsAsciiEqualsIgnoreCase(strA))
				return true;

			if (chars.IsAsciiEqualsIgnoreCase(strB))
				return true;

			if (chars.IsAsciiEqualsIgnoreCase(strC))
				return true;

			if (chars.IsAsciiEqualsIgnoreCase(strD))
				return true;

			if (chars.IsAsciiEqualsIgnoreCase(strE))
				return true;

			if (chars.IsAsciiEqualsIgnoreCase(strF))
				return true;

			if (strG is not null && chars.IsAsciiEqualsIgnoreCase(strG))
				return true;

			if (strH is not null && chars.IsAsciiEqualsIgnoreCase(strH))
				return true;

			if (strI is not null && chars.IsAsciiEqualsIgnoreCase(strI))
				return true;

			return strJ is not null && chars.IsAsciiEqualsIgnoreCase(strJ);

		}

		/// <summary>
		/// Checks if a span of ASCII characters is equals to any of 5 ASCII-only strings, while ignoring case.
		/// Yes, the code might look weird, but it saves up a lot in performance.
		/// I call this method every single iteration of a loop.
		/// If I used params string[], that would allocate a whole new array every single time I call this method.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsAsciiEqualsIgnoreCase_5(
			this ReadOnlySpan<char> chars,
			string strA,
			string strB,
			string strC,
			string strD,
			string strE
		)
		{

			if (chars.IsAsciiEqualsIgnoreCase(strA))
				return true;

			if (chars.IsAsciiEqualsIgnoreCase(strB))
				return true;

			if (chars.IsAsciiEqualsIgnoreCase(strC))
				return true;

			return chars.IsAsciiEqualsIgnoreCase(strD) || chars.IsAsciiEqualsIgnoreCase(strE);

		}

	}

}
