using System;
using System.Collections.Generic;


namespace OceanApocalypseStudios.RSML.Analyzer.Syntax
{

	/// <summary>
	/// A performant syntax line.
	/// </summary>
	public struct SyntaxLine
	{

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
		/// Creates a new syntax line.
		/// </summary>
		/// <param name="token">A token</param>
		public SyntaxLine(SyntaxToken token) { Item1 = token; }

		/// <summary>
		/// Creates a new syntax line.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <param name="token3"></param>
		public SyntaxLine(SyntaxToken token1, SyntaxToken token2, SyntaxToken token3)
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
		/// Checks if the line is empty.
		/// </summary>
		public bool IsEmpty => Length <= 0;

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
		/// Removes a token at index.
		/// </summary>
		/// <param name="index">The index of the token to remove</param>
		public void Remove(int index) => this[index] = SyntaxToken.Empty;
		// todo: ^ Remove(int) leaves gaps
		// maybe make it automatically bump the ones below up??
		// problem with this is that we'd have to check if this wouldn't blow the whole codebase up
		// todo: maybe reusable Compact() method for this thing

		/// <summary>
		/// Converts the line into a list of tokens.
		/// </summary>
		/// <returns>The tokens</returns>
		public List<SyntaxToken> ToList()
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

		/// <summary>
		/// Returns the first non-empty token's index.
		/// </summary>
		/// <returns>The index</returns>
		public byte First()
		{

			// todo: maybe actually return the token itself??

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

			return !Item7.IsEmpty ? (byte)6 : (byte)7;

		}

		/// <summary>
		/// Returns the last non-empty token's index.
		/// </summary>
		/// <returns>The token</returns>
		public byte Last()
		{

			// todo: maybe actually return the token itself??

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

			return !Item2.IsEmpty ? (byte)1 : (byte)0;

		}

		/// <summary>
		/// The amount of non-empty tokens.
		/// </summary>
		public int Length
		{

			// todo: length is O(8)
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

			Item1 = token;

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

		/// <summary>
		/// Accesses a token.
		/// </summary>
		/// <param name="index">The index of the token</param>
		/// <exception cref="IndexOutOfRangeException">The index exceeds the amount of tokens</exception>
		public SyntaxToken this[int index]
		{

			get =>
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

	}

}
