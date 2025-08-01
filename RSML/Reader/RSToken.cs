using System;


namespace RSML.Reader
{

	/// <summary>
	/// A RSML token.
	/// </summary>
	public readonly struct RSToken
	{

		/// <summary>
		/// The token's type.
		/// </summary>
		public RSTokenType Type { get; init; }

		/// <summary>
		/// The token's value.
		/// </summary>
		public ReadOnlyMemory<char> Value { get; init; }

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="type">Token type</param>
		/// <param name="value">Token value, as memory</param>
		public RSToken(RSTokenType type, ReadOnlyMemory<char> value)
		{

			Type = type;
			Value = value;

		}

		/// <summary>
		/// Initializes a new RSML token.
		/// </summary>
		/// <param name="type">Token type</param>
		/// <param name="value">Token value, as string</param>
		public RSToken(RSTokenType type, string value)
		{

			Type = type;
			Value = value.AsMemory();

		}

	}

}
