using System;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Performance.Value;
using OceanApocalypseStudios.RSML.Toolchain.Compliance;


namespace OceanApocalypseStudios.RSML.Performance.Stateless
{

	/// <summary>
	/// A stateless RSML reader.
	/// </summary>
	public static class StatelessReader
	{

		/// <summary>
		/// The level of compliance, per feature, with the official language standard for the current API version.
		/// </summary>
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull("2.0.0");

		/// <summary>
		/// Tries to tokenize the first line of the buffer.
		/// </summary>
		/// <param name="buffer">The buffer to tokenize</param>
		/// <param name="line">The output tokens</param>
		/// <param name="nextLineStart">The index of the start of the next newline</param>
		/// <returns><c>true</c> if successful</returns>
		public static bool TryTokenizeNextLine(ReadOnlySpan<char> buffer, out SyntaxLine line, out int nextLineStart) =>
			TryTokenizeNextLine(buffer, 0, out line, out nextLineStart);

		/// <summary>
		/// Tries to tokenize the next line of a buffer.
		/// </summary>
		/// <param name="buffer">The buffer to tokenize</param>
		/// <param name="startPosition">The position at which to start tokenizing</param>
		/// <param name="line">The output tokens</param>
		/// <param name="nextLineStart">The index of the start of the next newline</param>
		/// <returns><c>true</c> if successful</returns>
		public static bool TryTokenizeNextLine(
			ReadOnlySpan<char> buffer,
			int startPosition,
			out SyntaxLine line,
			out int nextLineStart
		)
		{

			int curIndex = startPosition;

			if (curIndex < 0 || curIndex >= buffer.Length)
			{

				line = new(new(TokenKind.Eof, '\0'));
				nextLineStart = -1;

				return false;

			}

			int nextNewline = buffer[curIndex..].IndexOfNewline(out byte newlineLen);

			ReadOnlySpan<char> lineSpan;
			int advancedBy;

			if (nextNewline < 0)
			{

				lineSpan = buffer[curIndex..];
				advancedBy = lineSpan.Length;

			}
			else
			{

				lineSpan = buffer[curIndex..(nextNewline + curIndex)];
				advancedBy = nextNewline + newlineLen; // handle CRLF and LF

			}

			curIndex += advancedBy;
			nextLineStart = curIndex;

			if (lineSpan.IsEmpty)
			{

				line = new(new(TokenKind.Eol, Environment.NewLine));

				return true;

			}

			line = OptimizedLexer.TokenizeLine(lineSpan);

			return true;

		}

	}

}
