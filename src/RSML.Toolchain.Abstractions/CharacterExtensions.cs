using System.Runtime.CompilerServices;

namespace OceanApocalypseStudios.RSML.Toolchain.Abstractions;

public static class CharacterExtensions
{
	extension(char character)
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsNewline() => character is '\r' or '\n' or '\u2028' or '\u2029';
	}
}
