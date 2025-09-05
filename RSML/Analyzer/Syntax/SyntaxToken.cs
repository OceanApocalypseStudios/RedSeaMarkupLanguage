using System;
using System.Diagnostics.CodeAnalysis;


namespace OceanApocalypseStudios.RSML.Analyzer.Syntax
{

	/// <summary>
	/// A RSML token.
	/// </summary>
	public readonly struct SyntaxToken : IEquatable<SyntaxToken>
	{

		/// <summary>
		/// The token's type.
		/// </summary>
		public TokenKind Kind { get; }

		/// <summary>
		/// The token's value's range in the buffer.
		/// </summary>
		public Range BufferRange { get; } = new(^1, 0);

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="kind">Token type</param>
		/// <param name="start">Token start</param>
		/// <param name="end">Token end (exclusive)</param>
		public SyntaxToken(TokenKind kind, Index start, Index end)
		{

			Kind = kind;
			BufferRange = new(start, end);

		}

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="kind">Token type</param>
		/// <param name="range">Token value, as range</param>
		public SyntaxToken(TokenKind kind, Range range)
		{

			Kind = kind;
			BufferRange = range;

		}

		/// <summary>
		/// Checks if the token is off limits.
		/// </summary>
		public bool IsOffLimits => BufferRange.Start.Equals(^1) && BufferRange.End.Equals(0);

		/// <summary>
		/// Checks if the token is empty.
		/// </summary>
		public bool IsEmpty => Kind == TokenKind.UndefinedToken && IsOffLimits;

		/// <summary>
		/// Creates an empty syntax token.
		/// </summary>
		public static SyntaxToken Empty => new(TokenKind.UndefinedToken, ^1, 0);

		/// <inheritdoc />
		public override string ToString() => $"SyntaxToken({Kind.ToString()}, {BufferRange.Start}, {BufferRange.End})";

		/// <summary>
		/// Checks if this instance of a token is equals to an object. Requires boxing.
		/// Use <c>==</c> and <c>!=</c> instead if possible.
		/// </summary>
		/// <param name="obj">The object to check for equality</param>
		/// <returns><c>true</c> if the 2 objects are the same</returns>
		public override bool Equals([NotNullWhen(true)] object? obj) =>
			obj is SyntaxToken token &&
			token.Kind == Kind &&
			token.BufferRange.Equals(BufferRange);

		/// <summary>
		/// Returns the hash code of this instance of a token.
		/// </summary>
		/// <returns>The hash code, as an <see cref="Int32" /></returns>
		public override int GetHashCode() => HashCode.Combine(Kind, BufferRange);

		/// <summary>
		/// Checks if 2 tokens are the same.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <returns><c>true</c> if they're equals</returns>
		public static bool operator ==(SyntaxToken token1, SyntaxToken token2) =>
			token1.Kind == token2.Kind && token1.BufferRange.Equals(token2.BufferRange);

		/// <summary>
		/// Checks if 2 tokens are different.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <returns><c>true</c> if they're different</returns>
		public static bool operator !=(SyntaxToken token1, SyntaxToken token2) => !(token1 == token2);

		/// <inheritdoc />
		public bool Equals(SyntaxToken other) => Kind == other.Kind && BufferRange.Equals(other.BufferRange);

	}

}
