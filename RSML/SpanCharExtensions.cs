using System;


namespace RSML
{

	internal static class SpanCharExtensions
	{

		/// <summary>
		/// Checks is a span of characters contains nothing but new lines / carriage returns.
		/// </summary>
		/// <param name="chars">The span of characters</param>
		/// <returns><c>true</c> if all newlines / carriage returns</returns>
		internal static bool IsNewLinesOnly(this ReadOnlySpan<char> chars)
		{

			foreach (char c in chars)
			{

				if (c != '\r' && c != '\n')
					return false;

			}

			return true;

		}

		internal static bool IsEquals(this Span<char> chars, string str) => chars.SequenceEqual(str);

		internal static bool IsEquals(this Span<char> chars, string? str, StringComparison stringComparison)
		{

			if (str is null)
				return false;

			return stringComparison switch
			{

				StringComparison.Ordinal => chars.IsEquals(str), // 0 allocs
				StringComparison.OrdinalIgnoreCase
					or StringComparison.CurrentCulture
					or StringComparison.CurrentCultureIgnoreCase
					or StringComparison.InvariantCulture
					or StringComparison.InvariantCultureIgnoreCase =>
					String.Equals(chars.ToString(), str, stringComparison), // sadly 1 alloc but hey who gives a fuck
				_ => throw new ArgumentException("Unsupported StringComparison mode.", nameof(stringComparison))

			};

		}

		internal static bool IsEquals(this ReadOnlySpan<char> chars, string str) => chars.SequenceEqual(str);

		internal static bool IsEquals(this ReadOnlySpan<char> chars, string? str, StringComparison stringComparison)
		{

			if (str is null)
				return false;

			return stringComparison switch
			{

				StringComparison.Ordinal => chars.IsEquals(str), // 0 allocs
				StringComparison.OrdinalIgnoreCase
					or StringComparison.CurrentCulture
					or StringComparison.CurrentCultureIgnoreCase
					or StringComparison.InvariantCulture
					or StringComparison.InvariantCultureIgnoreCase =>
					String.Equals(chars.ToString(), str, stringComparison), // sadly 1 alloc but hey who gives a fuck
				_ => throw new ArgumentException("Unsupported StringComparison mode.", nameof(stringComparison))

			};

		}

		internal static bool IsEquals(this ReadOnlySpan<char> chars, StringComparison stringComparison, params string?[] strings)
		{

			foreach (string? str in strings)
			{

				if (str is null)
					continue;

				if (chars.IsEquals(str, stringComparison))
					return true;

			}

			return false;

		}

	}

}
