using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace OceanApocalypseStudios.RSML.Analyzer
{

	/// <summary>
	/// All use-cases text buffer.
	/// </summary>
	public sealed class DualTextBuffer
	{

		private ReadOnlyMemory<char> primary;

		private int primaryCaretPos;
		private ReadOnlyMemory<char> secondary = ReadOnlyMemory<char>.Empty;
		private int secondaryCaretPos;
		private bool usesPrimary = true;

		/// <summary>
		/// Creates an empty text buffer.
		/// </summary>
		public DualTextBuffer() { primary = ReadOnlyMemory<char>.Empty; }

		/// <summary>
		/// Creates a text buffer from memory.
		/// </summary>
		/// <param name="text"></param>
		public DualTextBuffer(ReadOnlyMemory<char> text) { primary = text; }

		/// <summary>
		/// Creates a text buffer from a string.
		/// </summary>
		/// <param name="text"></param>
		public DualTextBuffer(string text) { primary = text.AsMemory(); }

		/// <summary>
		/// Creates a text buffer from a string reader.
		/// </summary>
		/// <param name="reader"></param>
		public DualTextBuffer(StringReader reader) { primary = reader.ReadToEnd().AsMemory(); }

		/// <summary>
		/// Creates a text buffer from an enumerable of strings.
		/// </summary>
		/// <param name="lines">The lines</param>
		public DualTextBuffer(IEnumerable<string> lines) { primary = String.Join('\n', lines).AsMemory(); }

		/// <summary>
		/// Creates a text buffer from an array of characters.
		/// </summary>
		/// <param name="characters">UTF-16 characters</param>
		public DualTextBuffer(char[] characters) { primary = new(characters); }

		/// <summary>
		/// Creates a text buffer from an array of characters.
		/// </summary>
		/// <param name="characters">UTF-8 characters</param>
		public DualTextBuffer(byte[] characters) { primary = Encoding.UTF8.GetString(characters).AsMemory(); }

		/// <summary>
		/// Creates a text buffer from a span of characters.
		/// </summary>
		/// <param name="characters">UTF-8 characters</param>
		public DualTextBuffer(ReadOnlySpan<byte> characters) { primary = Encoding.UTF8.GetString(characters).AsMemory(); }

		/// <summary>
		/// Creates a text buffer from a span of characters.
		/// </summary>
		/// <param name="text">The text</param>
		public DualTextBuffer(ReadOnlySpan<char> text) { primary = text.ToString().AsMemory(); }

		/// <summary>
		/// The number of the buffer that is currently in use.
		/// </summary>
		public byte BufferNumber => (byte)(usesPrimary ? 1 : 2);

		/// <summary>
		/// The current caret position in the current buffer. Make sure to check against length.
		/// </summary>
		public int CaretPosition
		{

			get => usesPrimary ? primaryCaretPos : secondaryCaretPos;

			private set
			{

				if (usesPrimary)
					primaryCaretPos = value;
				else
					secondaryCaretPos = value;

			}

		}

		/// <summary>
		/// <c>true</c> if the buffer is empty.
		/// </summary>
		public bool IsEmpty => Text.IsEmpty;

		/// <summary>
		/// The length of the buffer.
		/// </summary>
		public int Length => Text.Length;

		/// <summary>
		/// The loaded text.
		/// </summary>
		public ReadOnlyMemory<char> Text
		{

			get => usesPrimary ? primary : secondary;

			set
			{

				CaretPosition = 0;

				if (usesPrimary)
					primary = value;
				else
					secondary = value;

			}

		}

		/// <summary>
		/// Goes back to index 0 on the current buffer.
		/// </summary>
		public void BackToStart() => CaretPosition = 0;

		/// <summary>
		/// Retrieves the next character in the buffer without advancing to its position.
		/// </summary>
		/// <returns>The character, as an integer, or <c>-1</c> if the end of the buffer has been reached.</returns>
		public int Peek()
		{

			if (CaretPosition >= Text.Length)
				return -1;

			return Convert.ToInt32(Text.Span[CaretPosition]);

		}

		/// <summary>
		/// Retrieves the next character in the buffer and advances to its position.
		/// </summary>
		/// <returns>The character, as an integer, or <c>-1</c> if the end of the buffer has been reached.</returns>
		public int Read()
		{

			if (CaretPosition >= Text.Length)
				return -1;

			return Convert.ToInt32(Text.Span[CaretPosition++]);

		}

		/// <summary>
		/// Retrieves a given amount of characters, starting at the current position.
		/// </summary>
		/// <param name="characters">The amount of characters to read</param>
		/// <returns>The characters</returns>
		public ReadOnlyMemory<char> Read(int characters)
		{

			int endIndex = CaretPosition + characters + 1; // end index, inclusive in this case, hence +1 motherfucker

			if (CaretPosition >= Text.Length || endIndex >= Text.Length)
				return Text[CaretPosition..];

			return Text[CaretPosition..endIndex];

		}

		/// <summary>
		/// Reads a whole line.
		/// </summary>
		/// <returns>The line</returns>
		public ReadOnlyMemory<char> ReadLine()
		{

			if (CaretPosition >= Text.Length)
				return ReadOnlyMemory<char>.Empty;

			int startIndex = CaretPosition;
			var span = Text.Span[CaretPosition..];
			int nextNewline = span.IndexOfNewline(out byte newlineLen);
			int advanceBy;

			if (nextNewline < 0)
				advanceBy = span.Length;
			else
				advanceBy = nextNewline + newlineLen;

			CaretPosition += advanceBy;

			return Text[startIndex..CaretPosition];

		}

		/// <summary>
		/// Reads continuously until a predicate returns <c>true</c>.
		/// </summary>
		/// <param name="predicate">The predicate</param>
		/// <returns>The text, from the current index to the moment the predicate is hit</returns>
		public ReadOnlyMemory<char> ReadUntil(Func<char, int, bool> predicate)
		{

			int startIndex = CaretPosition;

			while (CaretPosition < Text.Length)
			{

				char c = Text.Span[CaretPosition++];

				if (predicate(c, CaretPosition - 1))
					break;

			}

			return startIndex == CaretPosition ? ReadOnlyMemory<char>.Empty : Text[startIndex..CaretPosition];

		}

		/// <summary>
		/// Reads continuously until a whitespace character is found.
		/// </summary>
		/// <param name="enforceNewlinesAsWhitespace">
		/// If set to<c>true</c>, newlines are counted as "whitespace". Otherwise, only actual space characters are.
		/// </param>
		/// <returns>The text from the current index until whitespace is found</returns>
		public ReadOnlyMemory<char> ReadUntilWhitespace(bool enforceNewlinesAsWhitespace = true)
		{

			int startIndex = CaretPosition;

			if (enforceNewlinesAsWhitespace)
			{

				while (CaretPosition < Text.Length && !Char.IsWhiteSpace(Text.Span[CaretPosition]))
					CaretPosition++;

			}
			else
			{

				while (CaretPosition < Text.Length && Text.Span[CaretPosition] != ' ')
					CaretPosition++;

			}

			return startIndex == CaretPosition ? ReadOnlyMemory<char>.Empty : Text[startIndex..CaretPosition];

		}

		/// <summary>
		/// Skips whitespace, advancing positions until a non-whitespace character is hit.
		/// </summary>
		/// <param name="enforceNewlinesAsWhitespace">
		/// If set to<c>true</c>, newlines are counted as "whitespace". Otherwise, only actual space characters are.
		/// </param>
		public void SkipWhitespace(bool enforceNewlinesAsWhitespace = true)
		{

			if (enforceNewlinesAsWhitespace)
			{

				while (CaretPosition < Text.Length && Char.IsWhiteSpace(Text.Span[CaretPosition]))
					CaretPosition++;

				return;

			}

			while (CaretPosition < Text.Length && Text.Span[CaretPosition] == ' ')
				CaretPosition++;

		}

		/// <summary>
		/// Swaps buffer from primary to secondary (hold) and back.
		/// </summary>
		public void SwapBuffer() => usesPrimary ^= true;

		/// <summary>
		/// Returns character at given index.
		/// </summary>
		/// <param name="index">The index</param>
		public char this[int index] => Text.Span[index];

		/// <summary>
		/// Returns character at given index.
		/// </summary>
		/// <param name="index">The index</param>
		public char this[Index index] => Text.Span[index];

		/// <summary>
		/// Returns character at given range.
		/// </summary>
		/// <param name="range">The range</param>
		public ReadOnlyMemory<char> this[Range range] => Text[range];

	}

}
