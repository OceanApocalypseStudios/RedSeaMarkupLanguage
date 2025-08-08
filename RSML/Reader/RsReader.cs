using System;
using System.Collections.Generic;

using RSML.Tokenization;


namespace RSML.Reader
{

	/// <summary>
	/// The officially maintained RSML Reader that reads lines from a buffer and feeds them into a lexer.
	/// </summary>
	public sealed class RsReader : IReader
	{

		private int curIndex = 0;
		private readonly string source;
		private readonly RsToken[] eofToken = [ new(RsTokenType.Eof, '\0') ];
		private readonly RsToken[] eolToken = [ new(RsTokenType.Eol, Environment.NewLine) ];

		/// <inheritdoc/>
		public string? StandardizedVersion => "2.0.0";

		/// <summary>
		/// Initializes a RSML reader.
		/// </summary>
		/// <param name="source">A span of characters as input</param>
		public RsReader(ReadOnlySpan<char> source) { this.source = source.ToString().ReplaceLineEndings(); }

		/// <summary>
		/// Initializes a RSML reader.
		/// </summary>
		/// <param name="source">A string as input</param>
		public RsReader(string source) { this.source = source.ReplaceLineEndings(); }

		/// <inheritdoc/>
		public bool TryTokenizeNextLine(ILexer lexer, out IEnumerable<RsToken> tokens)
		{

			if (curIndex < 0 || curIndex >= source.Length)
			{

				tokens = eofToken;
				return false;

			}

			ReadOnlySpan<char> span = source.AsSpan(curIndex);
			int nextNewline = span.IndexOf(Environment.NewLine);

			ReadOnlySpan<char> lineSpan;
			int advancedBy;

			if (nextNewline < 0)
			{

				lineSpan = span;
				advancedBy = lineSpan.Length;

			}
			else
			{

				lineSpan = span[ ..nextNewline ];
				advancedBy = nextNewline + 1;

			}

			curIndex += advancedBy;

			if (lineSpan.IsEmpty)
			{

				tokens = eolToken;
				return true;

			}

			tokens = lexer.TokenizeLine(lineSpan.ToString());
			return true;

		}

	}

}
