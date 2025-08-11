using System;
using System.Diagnostics.CodeAnalysis;


namespace RSML.Analyzer.Syntax
{

	/// <summary>
	/// A RSML token.
	/// </summary>
	public readonly struct SyntaxToken
	{

		/// <summary>
		/// The token's type.
		/// </summary>
		public TokenKind Kind { get; }

		/// <summary>
		/// The token's value.
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="kind">Token type</param>
		/// <param name="value">Token value, as span</param>
		public SyntaxToken(TokenKind kind, ReadOnlySpan<char> value)
		{

			Kind = kind;
			Value = value.ToString();

		}

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="kind">Token type</param>
		/// <param name="value">Token value, as span</param>
		public SyntaxToken(TokenKind kind, Span<char> value)
		{

			Kind = kind;
			Value = value.ToString();

		}

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="kind">Token type</param>
		/// <param name="value">Token value, as string</param>
		public SyntaxToken(TokenKind kind, string value)
		{

			Kind = kind;
			Value = value;

		}

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="kind">Token type</param>
		/// <param name="value">Token value, as a character</param>
		public SyntaxToken(TokenKind kind, char value)
		{

			Kind = kind;
			Value = String.Create(1, value, (span, ch) => span[0] = ch);

		}

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
		public override int GetHashCode() => HashCode.Combine(Kind, Value);

		/// <summary>
		/// Checks if 2 tokens are the same.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <returns><c>true</c> if they're equals</returns>
		public static bool operator ==(SyntaxToken token1, SyntaxToken token2) => token1.Kind == token2.Kind && token1.Value == token2.Value;

		/// <summary>
		/// Checks if 2 tokens are different.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <returns><c>true</c> if they're different</returns>
		public static bool operator !=(SyntaxToken token1, SyntaxToken token2) => token1.Kind != token2.Kind || token1.Value != token2.Value;

	}

}
