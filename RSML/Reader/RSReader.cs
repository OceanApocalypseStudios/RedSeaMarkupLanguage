using System;
using System.Runtime.InteropServices;

using RSML.Language;
using RSML.Tokenization;


namespace RSML.Reader
{

	/// <summary>
	/// A performant byref stack allocated RSML reader that reads AND tokenizes RSML.
	/// </summary>
	public ref struct RsReader
	{

		private int curIndex = 0;
		private readonly ReadOnlySpan<char> source;
		private static RsToken eofToken = new(RsTokenType.EOF, "\0");
		private static RsToken eolToken = new(RsTokenType.EOL, "\n");

		/// <summary>
		/// Initializes a RSML reader.
		/// </summary>
		/// <param name="source">A span of characters as input</param>
		public RsReader(ReadOnlySpan<char> source) { this.source = source; }

		/// <summary>
		/// Initializes a RSML reader.
		/// </summary>
		/// <param name="source">A string as input</param>
		public RsReader(string source) { this.source = source.AsSpan(); }

		/// <summary>
		/// Tries to read the next character.
		/// </summary>
		/// <param name="character">The next character</param>
		/// <returns><c>false</c> if the end has been reached, in which case the character will be a null termiantor (<c>\0</c>)</returns>
		public bool TryReadNextCharacter(out char character)
		{

			if (source.Length < 1)
				curIndex = -1;

			if (curIndex < 0)
			{

				character = '\0';

				return false;

			}

			character = source[curIndex++];

			return true;

		}

		/// <summary>
		/// Tries to read and tokenize a whole line.
		/// </summary>
		/// <param name="tokenizer">The tokenizer to use</param>
		/// <param name="languageStandard">The language standard to use for the line</param>
		/// <param name="tokens">The return tokens</param>
		/// <returns><c>false</c> if the end has been reached</returns>
		public bool TryReadAndTokenizeLine(ITokenizer tokenizer, in LanguageStandard languageStandard, out ReadOnlySpan<RsToken> tokens)
		{

			if (source.Length < 1)
				curIndex = -1;

			if (curIndex < 0)
			{

				tokens = MemoryMarshal.CreateReadOnlySpan(ref eofToken, 1);

				return false;

			}

			int nextNewline = source[curIndex..].IndexOf('\n');

			if (nextNewline < 0)
			{

				tokens = tokenizer.TokenizeLine(source[curIndex..], languageStandard);
				curIndex = -1; // We consumed it. Done.

				return true;

			}

			var temp = source[curIndex..(curIndex + nextNewline)];

			if (temp.Length > 0 && temp[^1] == '\r')
			{

				curIndex++;        // skip the \r in the next iteration
				temp = temp[..^1]; // normalize \r\n by ignoring \r

			}

			if (temp.Length < 1)
			{

				tokens = MemoryMarshal.CreateReadOnlySpan(ref eolToken, 1);
				curIndex++;

				return true;

			}

			tokens = tokenizer.TokenizeLine(temp, languageStandard);
			curIndex += temp.Length + 1;

			return true;

		}

	}

}
