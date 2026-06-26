using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace OceanApocalypseStudios.RSML.Analyzer.Syntax
{

	/// <summary>
	/// A performant syntax line.
	/// </summary>
	public struct SyntaxLine : IEnumerable<SyntaxToken>, IEquatable<SyntaxLine>
	{

		/// <summary>
		/// Creates a new syntax line.
		/// </summary>
		/// <param name="token">A token</param>
		public SyntaxLine(SyntaxToken token) => Item1 = token;

		/// <summary>
		/// Creates a new syntax line.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <param name="token3"></param>
		public SyntaxLine(
			SyntaxToken token1,
			SyntaxToken token2,
			SyntaxToken token3
		)
		{

			Item1 = token1;
			Item2 = token2;
			Item3 = token3;

		}

		/// <summary>
		/// Creates a new syntax line.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <param name="token3"></param>
		/// <param name="token4"></param>
		/// <param name="token5"></param>
		public SyntaxLine(
			SyntaxToken token1,
			SyntaxToken token2,
			SyntaxToken token3,
			SyntaxToken token4,
			SyntaxToken token5
		)
		{

			Item1 = token1;
			Item2 = token2;
			Item3 = token3;
			Item4 = token4;
			Item5 = token5;

		}

		/// <summary>
		/// Creates a new syntax line.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <param name="token3"></param>
		/// <param name="token4"></param>
		/// <param name="token5"></param>
		/// <param name="token6"></param>
		/// <param name="token7"></param>
		/// <param name="token8"></param>
		public SyntaxLine(
			SyntaxToken token1,
			SyntaxToken token2,
			SyntaxToken token3,
			SyntaxToken token4,
			SyntaxToken token5,
			SyntaxToken token6,
			SyntaxToken token7,
			SyntaxToken token8
		)

		{

			Item1 = token1;
			Item2 = token2;
			Item3 = token3;
			Item4 = token4;
			Item5 = token5;
			Item6 = token6;
			Item7 = token7;
			Item8 = token8;

		}

		/// <summary>
		/// Creates a new syntax line.
		/// </summary>
		/// <param name="tokens">An array of tokens with at least 8 tokens</param>
		/// <exception cref="ArgumentOutOfRangeException">The array has less than 8 tokens</exception>
		public SyntaxLine(SyntaxToken[] tokens)
		{

			if (tokens.Length < 8)
				throw new ArgumentOutOfRangeException(nameof(tokens), "A syntax line as array must have at least 8 tokens");

			Item1 = tokens[0];
			Item2 = tokens[1];
			Item3 = tokens[2];
			Item4 = tokens[3];
			Item5 = tokens[4];
			Item6 = tokens[5];
			Item7 = tokens[6];
			Item8 = tokens[7];

		}

		/// <summary>
		/// Creates a new syntax line.
		/// </summary>
		/// <param name="tokens">A list of tokens with at least 8 tokens</param>
		/// <exception cref="ArgumentOutOfRangeException">The list has less than 8 tokens</exception>
		public SyntaxLine(IList<SyntaxToken> tokens)
		{

			if (tokens.Count < 8)
				throw new ArgumentOutOfRangeException(nameof(tokens), "A syntax line as array must have at least 8 tokens");

			Item1 = tokens[0];
			Item2 = tokens[1];
			Item3 = tokens[2];
			Item4 = tokens[3];
			Item5 = tokens[4];
			Item6 = tokens[5];
			Item7 = tokens[6];
			Item8 = tokens[7];

		}

		/// <summary>
		/// Returns the first non-empty token's index.
		/// </summary>
		/// <returns>The token's index</returns>
		public readonly byte IndexOfFirst
		{

			get
			{

				if (!Item1.IsEmpty)
					return 0;

				if (!Item2.IsEmpty)
					return 1;

				if (!Item3.IsEmpty)
					return 2;

				if (!Item4.IsEmpty)
					return 3;

				if (!Item5.IsEmpty)
					return 4;

				if (!Item6.IsEmpty)
					return 5;

				return (byte)(!Item7.IsEmpty
								  ? 6
								  : 7);

			}

		}

		/// <summary>
		/// Returns the last non-empty token's index.
		/// </summary>
		/// <returns>The token's index</returns>
		public readonly byte IndexOfLast
		{

			get
			{

				if (!Item8.IsEmpty)
					return 7;

				if (!Item7.IsEmpty)
					return 6;

				if (!Item6.IsEmpty)
					return 5;

				if (!Item5.IsEmpty)
					return 4;

				if (!Item4.IsEmpty)
					return 3;

				if (!Item3.IsEmpty)
					return 2;

				return (byte)(!Item2.IsEmpty
								  ? 1
								  : 0);

			}

		}

		/// <summary>
		/// Checks if the line is empty.
		/// </summary>
		public readonly bool IsEmpty => Length <= 0;

		/// <summary>
		/// Accesses a token.
		/// </summary>
		/// <param name="index">The index of the token</param>
		/// <exception cref="IndexOutOfRangeException">The index exceeds the amount of tokens</exception>
		public SyntaxToken this[int index]
		{

			readonly get =>
				index switch
				{
					0 => Item1,
					1 => Item2,
					2 => Item3,
					3 => Item4,
					4 => Item5,
					5 => Item6,
					6 => Item7,
					7 => Item8,
					_ => throw new IndexOutOfRangeException("No such item.")
				};

			set
			{

				switch (index)
				{
					case 0:
						Item1 = value;

						break;

					case 1:
						Item2 = value;

						break;

					case 2:
						Item3 = value;

						break;

					case 3:
						Item4 = value;

						break;

					case 4:
						Item5 = value;

						break;

					case 5:
						Item6 = value;

						break;

					case 6:
						Item7 = value;

						break;

					case 7:
						Item8 = value;

						break;

					default:
						throw new IndexOutOfRangeException("No such item.");

				}

			}

		}

		/// <summary>
		/// First token.
		/// </summary>
		public SyntaxToken Item1 { get; set; } = SyntaxToken.Empty;

		/// <summary>
		/// Second token.
		/// </summary>
		public SyntaxToken Item2 { get; set; } = SyntaxToken.Empty;

		/// <summary>
		/// Third token.
		/// </summary>
		public SyntaxToken Item3 { get; set; } = SyntaxToken.Empty;

		/// <summary>
		/// Fourth token.
		/// </summary>
		public SyntaxToken Item4 { get; set; } = SyntaxToken.Empty;

		/// <summary>
		/// Fifth token.
		/// </summary>
		public SyntaxToken Item5 { get; set; } = SyntaxToken.Empty;

		/// <summary>
		/// Sixth token.
		/// </summary>
		public SyntaxToken Item6 { get; set; } = SyntaxToken.Empty;

		/// <summary>
		/// Seventh token.
		/// </summary>
		public SyntaxToken Item7 { get; set; } = SyntaxToken.Empty;

		/// <summary>
		/// Eighth token.
		/// </summary>
		public SyntaxToken Item8 { get; set; } = SyntaxToken.Empty;

		/// <summary>
		/// The amount of non-empty tokens.
		/// </summary>
		public readonly int Length
		{

			// todo: see #26
			// length is O(8)
			// maybe cache length on operations??
			// to make this shite even faster??

			get
			{

				int len = 8;

				if (Item8.IsEmpty)
					len--;

				if (Item7.IsEmpty)
					len--;

				if (Item6.IsEmpty)
					len--;

				if (Item5.IsEmpty)
					len--;

				if (Item4.IsEmpty)
					len--;

				if (Item3.IsEmpty)
					len--;

				if (Item2.IsEmpty)
					len--;

				if (Item1.IsEmpty)
					len--;

				return len;

			}

		}

		/// <summary>
		/// Checks whether two objects of type <see cref="SyntaxLine" />
		/// are equal.
		/// </summary>
		/// <param name="line1">One of the lines</param>
		/// <param name="line2">One of the lines</param>
		/// <returns>True if equal, false if different</returns>
		public static bool operator ==(
			SyntaxLine line1,
			SyntaxLine line2
		) =>
			line1.Equals(line2);

		/// <summary>
		/// Checks whether two objects of type <see cref="SyntaxLine" />
		/// are different.
		/// </summary>
		/// <param name="line1">One of the lines</param>
		/// <param name="line2">One of the lines</param>
		/// <returns>True if different, false if equal</returns>
		public static bool operator !=(
			SyntaxLine line1,
			SyntaxLine line2
		) =>
			!line1.Equals(line2);

		/// <summary>
		/// Adds a token to the start of the line.
		/// </summary>
		/// <param name="token">The token</param>
		public void Add(SyntaxToken token)
		{

			if (Item1.IsEmpty)
				Item1 = token;

			else if (Item2.IsEmpty)
				Item2 = token;

			else if (Item3.IsEmpty)
				Item3 = token;

			else if (Item4.IsEmpty)
				Item4 = token;

			else if (Item5.IsEmpty)
				Item5 = token;

			else if (Item6.IsEmpty)
				Item6 = token;

			else if (Item7.IsEmpty)
				Item7 = token;

			else if (Item8.IsEmpty)
				Item8 = token;

			else
				throw new ArgumentOutOfRangeException(nameof(token), "Maximum length was reached (8)");

		}

		/// <summary>
		/// Adds a token to the end of the line.
		/// </summary>
		/// <param name="token">The token</param>
		public void AddToEnd(SyntaxToken token)
		{

			if (Item8.IsEmpty)
				Item8 = token;

			else if (Item7.IsEmpty)
				Item7 = token;

			else if (Item6.IsEmpty)
				Item6 = token;

			else if (Item5.IsEmpty)
				Item5 = token;

			else if (Item4.IsEmpty)
				Item4 = token;

			else if (Item3.IsEmpty)
				Item3 = token;

			else if (Item2.IsEmpty)
				Item2 = token;

			else if (Item1.IsEmpty)
				Item1 = token;

			else
				throw new ArgumentOutOfRangeException(nameof(token), "Maximum length was reached (8)");

		}

		/// <summary>
		/// Clears the collection.
		/// </summary>
		public void Clear()
		{

			Item1 = SyntaxToken.Empty;
			Item2 = SyntaxToken.Empty;
			Item3 = SyntaxToken.Empty;
			Item4 = SyntaxToken.Empty;
			Item5 = SyntaxToken.Empty;
			Item6 = SyntaxToken.Empty;
			Item7 = SyntaxToken.Empty;
			Item8 = SyntaxToken.Empty;

		}

		/// <inheritdoc />
		public readonly override bool Equals([NotNullWhen(true)] object? obj)
		{

			if (obj is SyntaxLine line)
				return Equals(line);

			if (obj is SyntaxToken[] array)
				return Equals(new(array));

			if (obj is IList<SyntaxToken> list)
				return Equals(new(list));

			return false;

		}

		/// <summary>
		/// Checks whether two objects of type <see cref="SyntaxLine" />
		/// are equal.
		/// </summary>
		/// <param name="line">The line to check against</param>
		/// <returns>True if equal, false if not</returns>
		public readonly bool Equals(SyntaxLine line) =>
			Item1.Equals(line.Item1) &&
			Item2.Equals(line.Item2) &&
			Item3.Equals(line.Item3) &&
			Item4.Equals(line.Item4) &&
			Item5.Equals(line.Item5) &&
			Item6.Equals(line.Item6) &&
			Item7.Equals(line.Item7) &&
			Item8.Equals(line.Item8);

		/// <summary>
		/// Returns the first non-empty token.
		/// </summary>
		/// <returns>The token</returns>
		public readonly SyntaxToken GetFirst()
		{

			if (!Item1.IsEmpty)
				return Item1;

			if (!Item2.IsEmpty)
				return Item2;

			if (!Item3.IsEmpty)
				return Item3;

			if (!Item4.IsEmpty)
				return Item4;

			if (!Item5.IsEmpty)
				return Item5;

			if (!Item6.IsEmpty)
				return Item6;

			return !Item7.IsEmpty
					   ? Item7
					   : Item8;

		}

		/// <inheritdoc />
		public readonly override int GetHashCode() =>
			HashCode.Combine(
				Item1, Item2, Item3, Item4, Item5,
				Item6, Item7, Item8
			);

		/// <summary>
		/// Returns the last non-empty token.
		/// </summary>
		/// <returns>The token</returns>
		public readonly SyntaxToken GetLast()
		{

			if (!Item8.IsEmpty)
				return Item8;

			if (!Item7.IsEmpty)
				return Item7;

			if (!Item6.IsEmpty)
				return Item6;

			if (!Item5.IsEmpty)
				return Item5;

			if (!Item4.IsEmpty)
				return Item4;

			if (!Item3.IsEmpty)
				return Item3;

			return !Item2.IsEmpty
					   ? Item2
					   : Item1;

		}

		/// <summary>
		/// Removes a token at index.
		/// </summary>
		/// <param name="index">The index of the token to remove</param>
		public void Remove(int index) => this[index] = SyntaxToken.Empty;

		// todo: ^ Remove(int) leaves gaps
		// maybe make it automatically bump the ones below up??
		// problem with this is that we'd have to check if this wouldn't blow the whole codebase up
		// xxx: maybe reusable Compact() method for this thing

		/// <summary>
		/// Converts the line into a list of tokens.
		/// </summary>
		/// <returns>The tokens</returns>
		public readonly List<SyntaxToken> ToList()
		{

			List<SyntaxToken> tokens = [ ];

			if (!Item1.IsEmpty)
				tokens.Add(Item1);

			if (!Item2.IsEmpty)
				tokens.Add(Item2);

			if (!Item3.IsEmpty)
				tokens.Add(Item3);

			if (!Item4.IsEmpty)
				tokens.Add(Item4);

			if (!Item5.IsEmpty)
				tokens.Add(Item5);

			if (!Item6.IsEmpty)
				tokens.Add(Item6);

			if (!Item7.IsEmpty)
				tokens.Add(Item7);

			if (!Item8.IsEmpty)
				tokens.Add(Item8);

			return tokens;

		}

		internal struct TokenEnumerator(SyntaxLine line) : IEnumerator<SyntaxToken>
		{

			private int index = -1;

			public readonly SyntaxToken Current => line[index];

			readonly object IEnumerator.Current => Current;

			public void Dispose() => Reset();

			public bool MoveNext()
			{

				if (index + 1 >= 8)
					return false;

				index++;
				return true;

			}

			public void Reset() => index = -1;

		}

		/// <inheritdoc/>
		public readonly IEnumerator<SyntaxToken> GetEnumerator() => new TokenEnumerator(this);

		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	}

}
