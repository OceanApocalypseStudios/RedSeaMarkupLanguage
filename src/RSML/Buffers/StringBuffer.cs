using System;
using System.IO;
using System.Text;

using OceanApocalypseStudios.RSML.Exceptions;


namespace OceanApocalypseStudios.RSML.Buffers
{

	internal class StringBuffer : IBuffer<char>
	{

		readonly string data;

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

		private void ThrowIfOutOfBounds(int index)
		{

			if (index >= data.Length)
				throw new ArgumentOutOfRangeException(nameof(index), "Index is out of bounds.");

		}

		private void ThrowIfEmptyBuffer()
		{

			if (data.Length == 0)
				throw new BufferException("Buffer is empty");

		}

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

		public bool TryGetChar(int index, out char item)
		{

			item = default; // default

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

		public bool TryGetLine(int index, Span<char> line, out int charCount)
		{

			charCount = 0;

			if (IsBufferEmpty())
				return false;

			if (index < 0)
				index = data.Length + index;

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

			return true;

		}

		public bool TryGetWord(int index, Span<char> destination, out bool isWhitespace, out int charCount) => throw new NotImplementedException();
		
		public bool TryGetWord(int index, char itemKind, Span<char> destination, out bool isItemKind, out int charCount) => throw new NotImplementedException();
		
		public int CountUntilNotWhitespace(int index) => throw new NotImplementedException();

		public char[] Slice(int start, int length) => throw new NotImplementedException();

		public void Slice(int start, Span<char> slice) => throw new NotImplementedException();

		public void Dispose() { } // noop :(

	}

}
