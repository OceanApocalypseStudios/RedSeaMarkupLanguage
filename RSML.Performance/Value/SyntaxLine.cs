using System;
using System.Collections.Generic;

using RSML.Analyzer.Syntax;


namespace RSML.Performance.Value
{

	/// <summary>
	/// A performant syntax line.
	/// </summary>
	public ref struct SyntaxLine
	{

		/// <summary>
		/// First token.
		/// </summary>
		public ValueToken Item1 { get; set; } = ValueToken.Empty;

		/// <summary>
		/// Second token.
		/// </summary>
		public ValueToken Item2 { get; set; } = ValueToken.Empty;

		/// <summary>
		/// Third token.
		/// </summary>
		public ValueToken Item3 { get; set; } = ValueToken.Empty;

		/// <summary>
		/// Fourth token.
		/// </summary>
		public ValueToken Item4 { get; set; } = ValueToken.Empty;

		/// <summary>
		/// Fifth token.
		/// </summary>
		public ValueToken Item5 { get; set; } = ValueToken.Empty;

		/// <summary>
		/// Sixth token.
		/// </summary>
		public ValueToken Item6 { get; set; } = ValueToken.Empty;

		/// <summary>
		/// Seventh token.
		/// </summary>
		public ValueToken Item7 { get; set; } = ValueToken.Empty;

		/// <summary>
		/// Eighth token.
		/// </summary>
		public ValueToken Item8 { get; set; } = ValueToken.Empty;

		/// <summary>
		/// Creates a new syntax line.
		/// </summary>
		/// <param name="token">A token</param>
		public SyntaxLine(ValueToken token) { Item1 = token; }

		/// <summary>
		/// Creates a new syntax line.
		/// </summary>
		/// <param name="token1"></param>
		/// <param name="token2"></param>
		/// <param name="token3"></param>
		public SyntaxLine(ValueToken token1, ValueToken token2, ValueToken token3)
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
			ValueToken token1,
			ValueToken token2,
			ValueToken token3,
			ValueToken token4,
			ValueToken token5
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
			ValueToken token1,
			ValueToken token2,
			ValueToken token3,
			ValueToken token4,
			ValueToken token5,
			ValueToken token6,
			ValueToken token7,
			ValueToken token8
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
		public void Remove(int index) => this[index] = ValueToken.Empty;

		/// <summary>
		/// Converts the line into an array of tokens.
		/// </summary>
		/// <returns>The tokens</returns>
		public SyntaxToken[] ToArray() =>
		[
			(SyntaxToken)Item1, (SyntaxToken)Item2, (SyntaxToken)Item3, (SyntaxToken)Item4, (SyntaxToken)Item5, (SyntaxToken)Item6,
			(SyntaxToken)Item7, (SyntaxToken)Item8
		];

		/// <summary>
		/// Converts the line into a list of tokens.
		/// </summary>
		/// <returns>The tokens</returns>
		public List<SyntaxToken> ToList() =>
		[
			(SyntaxToken)Item1, (SyntaxToken)Item2, (SyntaxToken)Item3, (SyntaxToken)Item4, (SyntaxToken)Item5, (SyntaxToken)Item6,
			(SyntaxToken)Item7, (SyntaxToken)Item8
		];

		/// <summary>
		/// Returns the first non-empty token's index.
		/// </summary>
		/// <returns>The index</returns>
		public byte First()
		{

			if (!Item1.IsEmpty())
				return 0;

			if (!Item2.IsEmpty())
				return 1;

			if (!Item3.IsEmpty())
				return 2;

			if (!Item4.IsEmpty())
				return 3;

			if (!Item5.IsEmpty())
				return 4;

			if (!Item6.IsEmpty())
				return 5;

			return !Item7.IsEmpty() ? (byte)6 : (byte)7;

		}

		/// <summary>
		/// Returns the last non-empty token's index.
		/// </summary>
		/// <returns>The token</returns>
		public byte Last()
		{

			if (!Item8.IsEmpty())
				return 7;

			if (!Item7.IsEmpty())
				return 6;

			if (!Item6.IsEmpty())
				return 5;

			if (!Item5.IsEmpty())
				return 4;

			if (!Item4.IsEmpty())
				return 3;

			if (!Item3.IsEmpty())
				return 2;

			return !Item2.IsEmpty() ? (byte)1 : (byte)0;

		}

		/// <summary>
		/// The amount of non-empty tokens.
		/// </summary>
		public int Length
		{

			get
			{

				int len = 8;

				if (Item8.IsEmpty())
					len--;

				if (Item7.IsEmpty())
					len--;

				if (Item6.IsEmpty())
					len--;

				if (Item5.IsEmpty())
					len--;

				if (Item4.IsEmpty())
					len--;

				if (Item3.IsEmpty())
					len--;

				if (Item2.IsEmpty())
					len--;

				if (Item1.IsEmpty())
					len--;

				return len;

			}

		}

		/// <summary>
		/// Adds a token to the start of the line.
		/// </summary>
		/// <param name="token">The token</param>
		public void Add(ValueToken token)
		{

			if (Item1.IsEmpty())
				Item1 = token;

			else if (Item2.IsEmpty())
				Item2 = token;

			else if (Item3.IsEmpty())
				Item3 = token;

			else if (Item4.IsEmpty())
				Item4 = token;

			else if (Item5.IsEmpty())
				Item5 = token;

			else if (Item6.IsEmpty())
				Item6 = token;

			else if (Item7.IsEmpty())
				Item7 = token;

			else if (Item8.IsEmpty())
				Item8 = token;

		}

		/// <summary>
		/// Adds a token to the end of the line.
		/// </summary>
		/// <param name="token">The token</param>
		public void AddToEnd(ValueToken token)
		{

			if (Item8.IsEmpty())
				Item8 = token;

			else if (Item7.IsEmpty())
				Item7 = token;

			else if (Item6.IsEmpty())
				Item6 = token;

			else if (Item5.IsEmpty())
				Item5 = token;

			else if (Item4.IsEmpty())
				Item4 = token;

			else if (Item3.IsEmpty())
				Item3 = token;

			else if (Item2.IsEmpty())
				Item2 = token;

			Item1 = token;

		}

		/// <summary>
		/// Accesses a token.
		/// </summary>
		/// <param name="index">The index of the token</param>
		/// <exception cref="IndexOutOfRangeException">The index exceeds the amount of tokens</exception>
		public ValueToken this[int index]
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
