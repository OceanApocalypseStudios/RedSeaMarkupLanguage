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
		/// Tries to read and tokenize a whole line.
		/// </summary>
		/// <param name="lexer">The tokenizer to use</param>
		/// <param name="tokens">The return tokens</param>
		/// <returns><c>false</c> if the end has been reached</returns>
		public bool TryReadAndTokenizeLine(ILexer lexer, out RsToken[] tokens)
		{

			if (curIndex < 0)
			{

				tokens = eofToken;

				return false;

			}

			var span = source[curIndex..].AsSpan();

			if (span.Length < 1)
			{

				curIndex = -1;
				TryReadAndTokenizeLine(lexer, out _); // will return false with EOF token

			}

			int nextNewline = span.IndexOf('\n');

			if (nextNewline < 0)
			{

				tokens = lexer.TokenizeLine(span);
				curIndex = -1; // We consumed it. Done.

				return true;

			}

			var temp = span[..(nextNewline)];

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
