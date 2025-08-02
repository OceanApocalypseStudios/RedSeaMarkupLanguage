using System;


namespace RSML
{

	internal static class ReadOnlySpanCharExtensions
	{

		/// <summary>
		/// Checks is a span of characters contains nothing but new lines / carriage returns.
		/// </summary>
		/// <param name="chars">The span of characters</param>
		/// <returns><c>true</c> if all newlines / carriage returns</returns>
		internal static bool IsNewLinesOnly(this ReadOnlySpan<char> chars)
		{

			foreach (var c in chars)
				if (c != '\r' && c != '\n')
					return false;

			return true;

		}

		internal static bool IsEquals(this ReadOnlySpan<char> chars, string str)
		{

			if (chars.Length != str.Length)
				return false;

			for (int i = 0; i < chars.Length; i++)
			{

				if (chars[i] != str[i])
					return false;

			}

			return true;

		}

	}

}
