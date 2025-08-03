using System;
using System.Diagnostics.CodeAnalysis;


namespace RSML.Tokenization
{

	/// <summary>
	/// A RSML token.
	/// </summary>
	public readonly struct RsToken
	{

		/// <summary>
		/// The token's type.
		/// </summary>
		public RsTokenType Type { get; init; }

		/// <summary>
		/// The token's value.
		/// </summary>
		public char[] Value { get; init; }

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="type">Token type</param>
		/// <param name="value">Token value, as span</param>
		public RsToken(RsTokenType type, ReadOnlySpan<char> value)
		{

			Type = type;
			Value = value.ToArray();

		}

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="type">Token type</param>
		/// <param name="value">Token value, as string</param>
		public RsToken(RsTokenType type, string value)
		{

			Type = type;
			Value = value.ToCharArray();

		}

		/// <summary>
		/// Checks if this instance of a token is equals to an object. Requires boxing.
		/// Use <c>==</c> and <c>!=</c> instead if possible.
		/// </summary>
		/// <param name="obj">The object to check for equality</param>
		/// <returns><c>true</c> if the 2 objects are the same</returns>
		public override bool Equals([NotNullWhen(true)] object? obj) => obj is RsToken token && token.Type == Type && token.Value == Value;

		/// <summary>
		/// Returns the hash code of this instance of a token.
		/// </summary>
		/// <returns>The hash code, as an <see cref="Int32" /></returns>
		public override int GetHashCode() => HashCode.Combine(Type, Value);

		/// <summary>
		/// Checks if 2 tokens are the same.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <returns><c>true</c> if they're equals</returns>
		public static bool operator ==(RsToken token1, RsToken token2) => token1.Type == token2.Type && token1.Value == token2.Value;

		/// <summary>
		/// Checks if 2 tokens are different.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <returns><c>true</c> if they're different</returns>
		public static bool operator !=(RsToken token1, RsToken token2) => token1.Type != token2.Type || token1.Value != token2.Value;

	}

}
