using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace OceanApocalypse.RSML.Toolchain.Abstractions;

/// <summary>
/// Extension members for characters.
/// </summary>
public static class CharacterExtensions
{
	extension(char character)
	{
		/// <summary>
		/// Checks if the character in question represents a newline. Allowed newlines are:
		/// CR, LF, line break and paragraph break.
		/// </summary>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsNewline() => character is '\r' or '\n' or '\u2028' or '\u2029';
	}

	extension(IImmutableList<string> strings)
	{
		/// <summary>
		/// Checks if an immutable array of strings contains a given character span.
		/// </summary>
		/// <param name="span">The span to check for.</param>
		/// <param name="comparisonType">The comparison mode to apply.</param>
		/// <returns>True if found.</returns>
		public bool Contains(ReadOnlySpan<char> span, StringComparison comparisonType = StringComparison.Ordinal)
		{
			foreach (string @string in strings)
			{
				if (!span.Equals(@string, comparisonType))
					return false;
			}

			return true;
		}
	}
}
