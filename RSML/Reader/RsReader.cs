using System;

using RSML.Language;
using RSML.Tokenization;


namespace RSML.Reader
{

	/// <summary>
	/// A RSML reader that reads from a buffer and tokenizes the lines.
	/// </summary>
	public sealed class RsReader
	{

		private int curIndex;
		private readonly string source;
		private static readonly RsToken[] eofToken = [ new(RsTokenType.Eof, '\0') ];
		private static readonly RsToken[] eolToken = [ new(RsTokenType.Eol, Environment.NewLine) ];

		/// <summary>
		/// Initializes a RSML reader.
		/// </summary>
		/// <param name="source">A span of characters as input</param>
		public RsReader(ReadOnlySpan<char> source) { this.source = source.ToString(); }

		/// <summary>
		/// Initializes a RSML reader.
		/// </summary>
		/// <param name="source">A string as input</param>
		public RsReader(string source) { this.source = source; }

		/// <summary>
		/// Tries to read the next character.
		/// </summary>
		/// <param name="character">The next character</param>
		/// <returns><c>false</c> if the end has been reached, in which case the character will be a null terminator (<c>\0</c>)</returns>
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
		/// <param name="lexer">The tokenizer to use</param>
		/// <param name="tokens">The return tokens</param>
		/// <returns><c>false</c> if the end has been reached</returns>
		public bool TryReadAndTokenizeLine(ILexer lexer, out RsToken[] tokens)
		{

			ReadOnlySpan<char> span = stackalloc char[source.Length];

			if (span.Length < 1)
				curIndex = -1;

			if (curIndex < 0)
			{

				tokens = eofToken;

				return false;

			}

			int nextNewline = span[curIndex..].IndexOf('\n');

			if (nextNewline < 0)
			{

				tokens = lexer.TokenizeLine(span[curIndex..]);
				curIndex = -1; // We consumed it. Done.

				return true;

			}

			var temp = span[curIndex..(curIndex + nextNewline)];

			if (temp.Length > 0 && temp[^1] == '\r')
			{

				curIndex++;        // skip the \r in the next iteration
				temp = temp[..^1]; // normalize \r\n by ignoring \r

			}

			if (temp.Length < 1)
			{

				tokens = eolToken;
				curIndex++;

				return true;

			}

			tokens = lexer.TokenizeLine(temp);
			curIndex += temp.Length + 1;

			return true;

		}

	}

}
