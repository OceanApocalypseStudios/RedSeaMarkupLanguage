using System;
using RSML.Language;


namespace RSML.Reader
{

	/// <summary>
	/// A performant byref stack allocated RSML reader that reads AND tokenizes RSML.
	/// </summary>
	public ref struct RSReader
	{

		private bool consumed = false;
		private ReadOnlySpan<char> source;
		private readonly RSToken eofToken = new() { Type = RSTokenType.EOF, Value = "\0".AsMemory() };
		private readonly RSToken eolToken = new() { Type = RSTokenType.EOL, Value = "\n".AsMemory() };

		/// <summary>
		/// Initializes a RSML reader.
		/// </summary>
		/// <param name="source">A span of characters as input</param>
		public RSReader(ReadOnlySpan<char> source)
		{

			this.source = source;

		}

		/// <summary>
		/// Initializes a RSML reader.
		/// </summary>
		/// <param name="source">A string as input</param>
		public RSReader(string source)
		{

			this.source = source.AsSpan();

		}

		/// <summary>
		/// Tries to read the next character.
		/// </summary>
		/// <param name="character">The next character</param>
		/// <returns><c>false</c> if the end has been reached, in which case the character will be a null termiantor (<c>\0</c>)</returns>
		public bool TryReadNextCharacter(out char character)
		{

			if (source.Length < 1)
				consumed = true;

			if (consumed)
			{

				character = '\0';
				return false;

			}

			character = source[0];
			source = source.Length == 1 ? [] : source[1..];

			return true;

		}

		/// <summary>
		/// Tries to read and tokenize a whole line.
		/// </summary>
		/// <param name="tokenizer">The tokenizer to use</param>
		/// <param name="languageStandard">The language standard to use for the line</param>
		/// <param name="tokens">The return tokens</param>
		/// <returns><c>false</c> if the end has been reached</returns>
		public bool TryReadAndTokenizeLine(ITokenizer tokenizer, in LanguageStandard languageStandard, out ReadOnlySpan<RSToken> tokens)
		{

			if (source.Length < 1)
				consumed = true;

			if (consumed)
			{

				tokens = new([eofToken]);
				return false;

			}

			var nextNewline = source.IndexOf('\n');

			if (nextNewline == -1)
			{

				tokens = tokenizer.TokenizeLine(source, languageStandard);
				consumed = true; // we consumed it. done.
				return true;

			}

			var temp = source[..nextNewline];

			if (temp.Length > 0 && temp[^1] == '\r')
				temp = temp[..^1]; // normalize \r\n by ignoring \r

			if (temp.Length < 1)
			{

				tokens = new([eolToken]);
				source = source[(nextNewline + 1)..];
				return true;

			}

			tokens = tokenizer.TokenizeLine(temp, languageStandard);
			source = source[(nextNewline + 1)..];
			return true;

		}

	}

}
