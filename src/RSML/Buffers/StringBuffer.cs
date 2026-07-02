using System;
using System.IO;
using System.Text;


namespace OceanApocalypseStudios.RSML.Buffers
{

	internal class StringBuffer : IBuffer<char>
	{

		readonly string data;

		/// <inheritdoc/>
		public int Length => data.Length;

		/// <inheritdoc/>
		public bool IsEmpty => Length == 0;

		/// <inheritdoc/>
		public char this[int index] => data[index];

		public StringBuffer(string content) => data = content;

		public StringBuffer(ReadOnlySpan<char> content) => data = content.ToString();

		public StringBuffer(char[] content) => data = new(content);

		public StringBuffer(byte[] content, Encoding? encoding = null) => data = encoding?.GetString(content) ?? Encoding.Default.GetString(content);

		public unsafe StringBuffer(byte* content, int byteCount, Encoding? encoding = null) => data = encoding?.GetString(content, byteCount) ?? Encoding.Default.GetString(content, byteCount);

		public StringBuffer(TextReader reader) => data = reader.ReadToEnd();

		/// <inheritdoc/>
		public int CountUntilWhitespace(int index)
		{

			if (IsBufferEmpty())
				return 0;

			if (index < 0)
				index = data.Length + index; // -1 is (Length-1) etc

			if (IsOutOfBounds(index))
				return -1;

			var span = data.AsSpan(index);
			int count = 0;

			while (count < span.Length && !Char.IsWhiteSpace(span[count]))
				count++;

			return count;

		}

		private int CountUntilMatch(int index, char character)
		{

			if (IsBufferEmpty())
				return 0;

			if (index < 0)
				index = data.Length + index; // -1 is (Length-1) etc

			if (IsOutOfBounds(index))
				return -1;

			var span = data.AsSpan(index);
			int count = 0;

			while (count < span.Length && span[count] == character)
				count++;

			return count;

		}

		private int CountUntilNotMatch(int index, char character)
		{

			if (IsBufferEmpty())
				return 0;

			if (index < 0)
				index = data.Length + index; // -1 is (Length-1) etc

			if (IsOutOfBounds(index))
				return -1;

			var span = data.AsSpan(index);
			int count = 0;

			while (count < span.Length && span[count] != character)
				count++;

			return count;

		}

		/// <inheritdoc/>
		public int CountUntilNewline(int index)
		{

			if (IsBufferEmpty())
				return 0;

			if (index < 0)
				index = data.Length + index; // -1 is (Length-1) etc

			if (IsOutOfBounds(index))
				return -1;

			var span = data.AsSpan(index);
			int count = 0;

			while (count < span.Length && !span[count].IsNewline())
				count++;

			return count;

		}

		private int CountUntilNotNewline(int index)
		{

			if (IsBufferEmpty())
				return 0;

			if (index < 0)
				index = data.Length + index; // -1 is (Length-1) etc

			if (IsOutOfBounds(index))
				return -1;

			var span = data.AsSpan(index);
			int count = 0;

			while (count < span.Length && span[count].IsNewline())
				count++;

			return count;

		}

		private bool IsOutOfBounds(int index) => index >= data.Length;

		private bool IsBufferEmpty() => data.Length == 0;

		/// <inheritdoc/>
		public int CountWhile(Func<int, char, bool> predicate, int index)
		{

			if (IsBufferEmpty())
				return 0;

			if (index < 0)
				index = data.Length + index; // -1 is (Length-1) etc

			if (IsOutOfBounds(index))
				return -1;

			var span = data.AsSpan(index);
			int count = 0;

			while (count < span.Length && predicate(count, span[count]))
				count++;

			return count;

		}

		/// <inheritdoc/>
		public bool TryGetChar(int index, out char item)
		{

			item = '\0'; // default

			if (IsBufferEmpty())
				return false;

			if (index < 0)
				index = data.Length + index; // -1 is (Length-1) etc

			if (IsOutOfBounds(index))
				return false;

			item = data[index];
			return true;

		}

		private bool IsStartOfLine(int index) => index == 0 || data[index - 1].IsNewline();

		/// <inheritdoc/>
		public bool TryGetLine(int index, Span<char> line, out int charCount)
		{

			charCount = 0;

			if (IsBufferEmpty())
				return false;

			if (index < 0)
				index += data.Length;

			if (IsOutOfBounds(index))
				return false;

			if (!IsStartOfLine(index))
				index += CountUntilNewline(index);

			index += CountUntilNotNewline(index); // wont be out of bounds and if index points to newline, will return 0

			if (IsOutOfBounds(index))
				return false;

			var actualLineLength = CountUntilNewline(index);
			var charsToCopyAmount = Math.Min(actualLineLength, line.Length);
			data.AsSpan(index, charsToCopyAmount).CopyTo(line);

			charCount = charsToCopyAmount;

			return line.Length >= actualLineLength;

		}

		/// <inheritdoc/>
		public bool TryGetWord(int index, Span<char> destination, out bool isWhitespace, out int charCount)
		{

			isWhitespace = false;
			charCount = 0;

			if (IsBufferEmpty())
				return false;

			if (index < 0)
				index += data.Length;

			if (IsOutOfBounds(index))
				return false;

			isWhitespace = Char.IsWhiteSpace(data[index]);

			// if first index is whitespace, the word is whitespace (ends when not whitespace/end of buffer)
			// if first index not whitespace, the word is not whitespace (ends on whitespace/end of buffer)
			int actualLineLength = isWhitespace ? CountUntilNotWhitespace(index) : CountUntilWhitespace(index);
			int charsToCopyAmount = Math.Min(actualLineLength, destination.Length);
			data.AsSpan(index, charsToCopyAmount).CopyTo(destination);

			charCount = charsToCopyAmount;

			return destination.Length >= actualLineLength;

		}

		/// <inheritdoc/>
		public bool TryGetWord(int index, char itemKind, Span<char> destination, out bool isItemKind, out int charCount)
		{

			isItemKind = false;
			charCount = 0;

			if (IsBufferEmpty())
				return false;

			if (index < 0)
				index += data.Length;

			if (IsOutOfBounds(index))
				return false;

			isItemKind = data[index] == itemKind;

			// if first index is KIND, the word is KIND (ends when not KIND/end of buffer)
			// if first index not KIND, the word is not KIND (ends on KIND/end of buffer)
			int actualLineLength = isItemKind ? CountUntilNotMatch(index, itemKind) : CountUntilMatch(index, itemKind);
			int charsToCopyAmount = Math.Min(actualLineLength, destination.Length);
			data.AsSpan(index, charsToCopyAmount).CopyTo(destination);

			charCount = charsToCopyAmount;

			return destination.Length >= actualLineLength;

		}
		
		/// <inheritdoc/>
		public int CountUntilNotWhitespace(int index)
		{

			if (IsBufferEmpty())
				return 0;

			if (index < 0)
				index = data.Length + index; // -1 is (Length-1) etc

			if (IsOutOfBounds(index))
				return -1;

			var span = data.AsSpan(index);
			int count = 0;

			while (count < span.Length && Char.IsWhiteSpace(span[count]))
				count++;

			return count;

		}

		/// <inheritdoc/>
		public char[] Slice(int start, int length) => data.ToCharArray(start, length);

		/// <inheritdoc/>
		public void Dispose() { } // noop :(

		/// <inheritdoc/>
		public bool TryGetWord(int index, Func<int, char, bool> itemKindPredicate, Span<char> destination, out bool isItemKind, out int charCount)
		{

			isItemKind = false;
			charCount = 0;

			if (IsBufferEmpty())
				return false;

			if (index < 0)
				index += data.Length;

			if (IsOutOfBounds(index))
				return false;

			isItemKind = itemKindPredicate(index, data[index]);

			// if first index is KIND, the word is KIND (ends when not KIND/end of buffer)
			// if first index not KIND, the word is not KIND (ends on KIND/end of buffer)
			int actualLineLength = isItemKind ? CountWhile((i, c) => !itemKindPredicate(i, c), index) : CountWhile(itemKindPredicate, index);
			int charsToCopyAmount = Math.Min(actualLineLength, destination.Length);
			data.AsSpan(index, charsToCopyAmount).CopyTo(destination);

			charCount = charsToCopyAmount;

			return destination.Length >= actualLineLength;

		}

		/// <inheritdoc/>
		public void Slice(int start, Span<char> slice) => data.AsSpan(start, slice.Length).CopyTo(slice);

	}

}
