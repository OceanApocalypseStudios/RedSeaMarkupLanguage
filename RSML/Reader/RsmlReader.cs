using System;
using System.Collections.Generic;

using RSML.Analyzer.Syntax;
using RSML.Toolchain;
using RSML.Toolchain.Compliance;


namespace RSML.Reader
{

	/// <summary>
	/// The officially maintained RSML Reader that reads lines from a buffer and feeds them into a lexer.
	/// </summary>
	public sealed class RsmlReader : IReader
	{

		private const string ApiVersion = "2.0.0";
		private readonly SyntaxToken[] eofToken = [ TokenBank.eofToken ];
		private readonly SyntaxToken[] eolToken = [ TokenBank.eolToken ];
		private readonly string source;

		private int curIndex;

		/// <summary>
		/// Initializes a RSML reader.
		/// </summary>
		/// <param name="source">A span of characters as input</param>
		public RsmlReader(ReadOnlySpan<char> source) { this.source = source.ToString(); }

		/// <summary>
		/// Initializes a RSML reader.
		/// </summary>
		/// <param name="source">A string as input</param>
		public RsmlReader(string source) { this.source = source; }

		/// <inheritdoc />
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull(ApiVersion);

		/// <inheritdoc />
		public bool TryTokenizeNextLine(out IEnumerable<SyntaxToken> tokens)
		{

			if (curIndex < 0 || curIndex >= source.Length)
			{

				tokens = eofToken;

				return false;

			}

			var span = source.AsSpan(curIndex);
			int nextNewline = span.IndexOfNewline(out byte newlineLen);

			ReadOnlySpan<char> lineSpan;
			int advancedBy;

			if (nextNewline < 0)
			{

				lineSpan = span;
				advancedBy = lineSpan.Length;

			}
			else
			{

				lineSpan = span[..nextNewline];
				advancedBy = nextNewline + newlineLen; // handle CRLF and LF

			}

			curIndex += advancedBy;

			if (lineSpan.IsEmpty)
			{

				tokens = eolToken;

				return true;

			}

			tokens = Lexer.TokenizeLine(lineSpan.ToString());

			return true;

		}

	}

}
