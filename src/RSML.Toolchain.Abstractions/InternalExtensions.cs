using System;
using System.Runtime.CompilerServices;

namespace OceanApocalypseStudios.RSML.Toolchain.Abstractions;

internal static class InternalExtensions
{
	extension(char character)
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsNewline() => character is '\r' or '\n' or '\u2028' or '\u2029';
	}

	extension(ReadOnlySpan<int> integers)
	{
#if !NETCOREAPP3_0_OR_GREATER
		public bool Contains(int item) => integers.IndexOf(item) != -1;
#endif
	}
}
