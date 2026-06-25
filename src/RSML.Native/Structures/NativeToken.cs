using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;

using JetBrains.Annotations;


namespace OceanApocalypseStudios.RSML.Native.Structures
{

	/// <summary>
	/// A native-friendly RSML token.
	/// </summary>
	[NoReorder]
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct NativeToken(SyntaxToken token)
	{

		/// <summary>
		/// The kind of token.
		/// </summary>
		public readonly byte kind = (byte)token.Kind;

		/// <summary>
		/// The index of the buffer at which the occurence starts.
		/// </summary>
		public readonly int startIndex = token.BufferRange.Start.IsFromEnd
							? -token.BufferRange.Start.Value
							: token.BufferRange.Start.Value;

		/// <summary>
		/// The index of the buffer at which the occurence ends.
		/// </summary>
		public readonly int endIndex = token.BufferRange.End.IsFromEnd
							? -token.BufferRange.End.Value
							: token.BufferRange.End.Value;

		/// <summary>
		/// Empty native token.
		/// </summary>
		public static NativeToken Empty => SyntaxToken.Empty;

		/// <summary>
		/// Converts the native RSML syntax token into a
		/// <strong>managed</strong> RSML syntax token.
		/// </summary>
		/// <returns>A managed (regular) token</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly SyntaxToken ToToken() =>
			new((TokenKind)kind, new(Math.Abs(startIndex), startIndex < 0), new(Math.Abs(endIndex), endIndex < 0));

		/// <summary>
		/// Checks if 2 native tokens are the same.
		/// </summary>
		/// <param name="left">One of the tokens</param>
		/// <param name="right">One of the tokens</param>
		/// <returns><c>true</c> if they're equals</returns>
		public static bool operator ==(
			NativeToken left,
			NativeToken right
		) =>
			left.Equals(right);

		/// <summary>
		/// Checks if 2 native tokens are different.
		/// </summary>
		/// <param name="left">One of the tokens</param>
		/// <param name="right">One of the tokens</param>
		/// <returns><c>true</c> if they're different</returns>
		public static bool operator !=(
			NativeToken left,
			NativeToken right
		) =>
			!(left == right);

		/// <summary>
		/// Directly converts from <see cref="SyntaxToken"/> to <see cref="NativeToken"/>.
		/// </summary>
		/// <param name="token"></param>
		public static implicit operator NativeToken(SyntaxToken token) => new(token);

		/// <inheritdoc />
		public readonly override bool Equals([NotNullWhen(true)] object? obj)
		{

			if (obj is NativeToken nativeToken)
				return Equals(nativeToken);

			if (obj is SyntaxToken managedToken)
				return Equals(managedToken);

			return false;

		}

		/// <summary>
		/// Checks whether two native tokens are equals.
		/// </summary>
		/// <param name="token">The other token</param>
		/// <returns>True if equals</returns>
		public readonly bool Equals(NativeToken token) => kind == token.kind && startIndex == token.startIndex && endIndex == token.endIndex;

		/// <summary>
		/// Checks whether this native token is equals to a given managed token.
		/// </summary>
		/// <param name="managedToken">The managed token</param>
		/// <returns>True if equals</returns>
		public readonly bool Equals(SyntaxToken managedToken) =>
			kind == (byte)managedToken.Kind &&
			startIndex ==
			(managedToken.BufferRange.Start.IsFromEnd
				 ? -managedToken.BufferRange.Start.Value
				 : managedToken.BufferRange.Start.Value) &&
			endIndex ==
			(managedToken.BufferRange.End.IsFromEnd
				 ? -managedToken.BufferRange.End.Value
				 : managedToken.BufferRange.End.Value);

		/// <inheritdoc />
		public readonly override int GetHashCode() => HashCode.Combine(kind, startIndex, endIndex);

	}

}
