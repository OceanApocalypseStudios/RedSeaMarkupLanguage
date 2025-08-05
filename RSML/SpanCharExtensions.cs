using System;

using RSML.Language;


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

		internal static bool IsDigitOnly(this ReadOnlySpan<char> chars)
		{

			foreach (char c in chars)
			{

				if (c != 0 && c != 1 && c != 2 && c != 3 && c != 4 && c != 5 && c != 6 && c != 7 && c != 8 && c != 9)
					return false;

			}

			return true;

		}

		internal static bool IsEquals(this ReadOnlySpan<char> chars, string str) => chars.SequenceEqual(str);

		internal static bool IsEquals(this Span<char> chars, string str) => chars.SequenceEqual(str);

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

			foreach (var str in strings)
			{

				if (str is null)
					continue;

				if (chars.IsEquals(str, stringComparison))
					return true;

			}

			return false;

		}

		internal static RsTokenType? GetArchTokenType(this ReadOnlySpan<char> chars)
		{

			if (chars.IsEquals("x64"))
				return RsTokenType.X64;

			if (chars.IsEquals("x86"))
				return RsTokenType.X86;

			if (chars.IsEquals("arm64"))
				return RsTokenType.Arm64;

			if (chars.IsEquals("arm32"))
				return RsTokenType.Arm32;

			return null;

		}

		internal static RsTokenType? GetSystemTokenType(this Span<char> chars)
		{

			if (chars.IsEquals("windows"))
				return RsTokenType.Windows;

			if (chars.IsEquals("osx"))
				return RsTokenType.OSX;

			if (chars.IsEquals("linux"))
				return RsTokenType.Linux;

			if (chars.IsEquals("arch"))
				return RsTokenType.Arch;

			if (chars.IsEquals("gentoo"))
				return RsTokenType.Gentoo;

			if (chars.IsEquals("freebsd"))
				return RsTokenType.FreeBsd;

			if (chars.IsEquals("debian"))
				return RsTokenType.Debian;

			if (chars.IsEquals("alpine"))
				return RsTokenType.Alpine;

			return null;

		}

	}

}
