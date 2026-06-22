using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;


namespace OceanApocalypseStudios.RSML.Native.Structures
{

	/// <summary>
	/// A native-friendly RSML token.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct NativeToken
    {

		/// <summary>
		/// The kind of token.
		/// </summary>
		public byte kind;

		/// <summary>
		/// The index of the buffer at which the occurence starts.
		/// </summary>
		public int startIndex;

		/// <summary>
		/// The index of the buffer at which the occurence ends.
		/// </summary>
		public int endIndex;

		/// <inheritdoc/>
		public override readonly bool Equals([NotNullWhen(true)] object? obj)
		{

			if (obj is NativeToken nativeToken)
				return Equals(nativeToken);

			if (obj is SyntaxToken managedToken)
				return Equals(managedToken);

			return false;

		}

		/// <inheritdoc/>
		public override readonly int GetHashCode() => HashCode.Combine(kind, startIndex, endIndex);

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
			startIndex == (managedToken.BufferRange.Start.IsFromEnd ? -managedToken.BufferRange.Start.Value : managedToken.BufferRange.Start.Value) &&
			endIndex == (managedToken.BufferRange.End.IsFromEnd ? -managedToken.BufferRange.End.Value : managedToken.BufferRange.End.Value);

		/// <summary>
		/// Checks if 2 native tokens are the same.
		/// </summary>
		/// <param name="left">One of the tokens</param>
		/// <param name="right">One of the tokens</param>
		/// <returns><c>true</c> if they're equals</returns>
		public static bool operator ==(NativeToken left, NativeToken right) => left.Equals(right);

		/// <summary>
		/// Checks if 2 native tokens are different.
		/// </summary>
		/// <param name="left">One of the tokens</param>
		/// <param name="right">One of the tokens</param>
		/// <returns><c>true</c> if they're different</returns>
		public static bool operator !=(NativeToken left, NativeToken right) => !(left == right);

	}

}
