using System.Runtime.CompilerServices;

namespace OceanApocalypseStudios.RSML.Toolchain.Abstractions;

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
}
