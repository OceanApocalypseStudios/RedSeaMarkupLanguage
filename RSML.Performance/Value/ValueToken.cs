using System;
using System.Diagnostics.CodeAnalysis;

using RSML.Analyzer.Syntax;


namespace RSML.Performance.Value
{

	/// <summary>
	/// A syntax token, but with compiler optimizations.
	/// </summary>
	public ref struct ValueToken
	{

		/// <summary>
		/// The token's kind.
		/// </summary>
		public TokenKind Kind { get; set; }

		/// <summary>
		/// The token's value.
		/// </summary>
		public ReadOnlySpan<char> Value { get; set; }

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="kind">Token type</param>
		/// <param name="value">Token value, as span</param>
		public ValueToken(TokenKind kind, ReadOnlySpan<char> value)
		{

			Kind = kind;
			Value = value;

		}

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="kind">Token type</param>
		/// <param name="value">Token value, as span</param>
		public ValueToken(TokenKind kind, Span<char> value)
		{

			Kind = kind;
			Value = value;

		}

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="kind">Token type</param>
		/// <param name="value">Token value, as string</param>
		public ValueToken(TokenKind kind, string value)
		{

			Kind = kind;
			Value = value.AsSpan();

		}

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="kind">Token type</param>
		/// <param name="value">Token value, as a single character</param>
		public ValueToken(TokenKind kind, char value)
		{

			Kind = kind;
			Value = String.Create(1, value, (span, ch) => span[0] = ch).AsSpan();

		}

		/// <summary>
		/// Checks if the current token is empty.
		/// </summary>
		/// <returns></returns>
		public bool IsEmpty() => IsEmpty(this);

		/// <summary>
		/// Empty token.
		/// </summary>
		public static ValueToken Empty => new(TokenKind.UndefinedToken, [ ]);

		/// <summary>
		/// Checks if a token is empty.
		/// </summary>
		/// <param name="token">The token</param>
		/// <returns></returns>
		public static bool IsEmpty(ValueToken token) => token.Kind == TokenKind.UndefinedToken && token.Value.IsEmpty;

		/// <inheritdoc />
		public override string ToString() => $"SyntaxToken({Kind.ToString()}, {Value.ToString().Replace("\r", "\\r").Replace("\n", "\\n")})";

		/// <summary>
		/// Checks if this instance of a token is equals to an object. Requires boxing.
		/// Use <c>==</c> and <c>!=</c> instead if possible.
		/// </summary>
		/// <param name="obj">The object to check for equality</param>
		/// <returns><c>true</c> if the 2 objects are the same</returns>
		public override bool Equals([NotNullWhen(true)] object? obj) => obj is SyntaxToken token && token.Kind == Kind && token.Value == Value;

		/// <summary>
		/// Returns the hash code of this instance of a token.
		/// </summary>
		/// <returns>The hash code, as an <see cref="Int32" /></returns>
		public override int GetHashCode() => HashCode.Combine(Kind, Value.ToString());

		/// <summary>
		/// Checks if 2 tokens are the same.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <returns><c>true</c> if they're equals</returns>
		public static bool operator ==(ValueToken token1, ValueToken token2) => token1.Kind == token2.Kind && token1.Value == token2.Value;

		/// <summary>
		/// Checks if 2 tokens are different.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <returns><c>true</c> if they're different</returns>
		public static bool operator !=(ValueToken token1, ValueToken token2) => token1.Kind != token2.Kind || token1.Value != token2.Value;

		/// <summary>
		/// Casts the <see cref="ValueToken" /> into a <see cref="SyntaxToken" />.
		/// </summary>
		/// <returns>A syntax token</returns>
		public static explicit operator SyntaxToken(ValueToken token) => new(token.Kind, token.Value);

	}

}
