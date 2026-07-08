using System;
using System.Collections.Generic;
using System.Text;

using OceanApocalypseStudios.RSML.Common;
using OceanApocalypseStudios.RSML.Exceptions;


namespace OceanApocalypseStudios.RSML.Sources.Buffers;

/// <summary>
/// A read-only buffer backed by a string. All operations opt for performance
/// primarily via the internal use of <see cref="ReadOnlySpan{Char}"/> over string allocations
/// and also via caching.
/// </summary>
public class ReadOnlyStringBuffer : IReadOnlyBuffer<char>, ISupportsCache, IEquatable<ReadOnlyStringBuffer?>
{
	private readonly List<int> lineStarts = [];

	private readonly List<int> precededByCrLf = [];

	private readonly string data;

	/// <inheritdoc/>
	public bool CacheExists { get; private set; } = false;

	/// <inheritdoc/>
	public bool IsEmpty => Length == 0;

	/// <inheritdoc/>
	public int Length => data.Length;

	/// <inheritdoc/>
	public int LineCount
	{
		get
		{
			ComputeLineStarts();

			return lineStarts.Count;
		}
	}

	/// <inheritdoc/>
	public char this[int index] => data[index];

	/// <summary>
	/// Initializes a new <see cref="ReadOnlyStringBuffer"/>
	/// with a string.
	/// </summary>
	/// <param name="content">The string that the buffer will wrap.</param>
	public ReadOnlyStringBuffer(string content) => data = content;

	/// <summary>
	/// Initializes a new <see cref="ReadOnlyStringBuffer"/>
	/// by allocating a string from a <see cref="ReadOnlySpan{Char}"/>.
	/// </summary>
	/// <param name="content">The span pointing to the string's data.</param>
	public ReadOnlyStringBuffer(ReadOnlySpan<char> content) => data = content.ToString();

	/// <summary>
	/// Initializes a new <see cref="ReadOnlyStringBuffer"/>
	/// with an array of characters.
	/// </summary>
	/// <param name="content">The array of characters to use for the buffer.</param>
	public ReadOnlyStringBuffer(char[] content) => data = new(content);

	/// <summary>
	/// Initializes a new <see cref="ReadOnlyStringBuffer"/>
	/// with an array of bytes and the encoding to use when decoding them.
	/// </summary>
	/// <param name="content">The array of bytes to use for the buffer.</param>
	/// <param name="encoding">
	/// The encoding to use when decoding <paramref name="content"/>.
	/// Use <c>null</c> for the <see cref="Encoding.Default"/> encoding.
	/// </param>
	public ReadOnlyStringBuffer(byte[] content, Encoding? encoding = null) => data = encoding?.GetString(content) ?? Encoding.Default.GetString(content);

	/// <summary>
	/// Initializes a new <see cref="ReadOnlyStringBuffer"/>
	/// with a pointer referencing an array of bytes and the encoding
	/// to use when decoding them.
	/// </summary>
	/// <param name="contentPtr">The pointer referecing the array of bytes to use for the buffer.</param>
	/// <param name="byteCount">The amount of bytes in the array referenced by <paramref name="contentPtr"/>.</param>
	/// <param name="encoding">
	/// The encoding to use when decoding <paramref name="contentPtr"/>.
	/// Use <c>null</c> for the <see cref="Encoding.Default"/> encoding.
	/// </param>
	/// <remarks>This method is not CLS-compliant due to the unsafe context and the use of pointers.</remarks>
	[CLSCompliant(false)]
	public unsafe ReadOnlyStringBuffer(byte* contentPtr, int byteCount, Encoding? encoding = null) =>
		data = (encoding ?? Encoding.Default).GetString(contentPtr, byteCount);

	/// <inheritdoc/>
	public void BuildCache() => ComputeLineStarts();

	/// <inheritdoc/>
	public void BuildCache(bool forceRebuild) => ComputeLineStarts(forceRebuild);

	/// <exception cref="BufferException">
	/// The buffer is empty.
	/// </exception>
	/// <exception cref="IndexOutOfRangeException">
	/// <paramref name="index"/> was set to something greater than the buffer's length.
	/// </exception>
	/// <inheritdoc/>
	public int CountUntilLineSeparator(int index, out bool isCrLf)
	{
		isCrLf = false;

		if (IsEmpty)
			throw new BufferException("The buffer is empty.");

		if (index < 0)
			index = Length + index; // -1 is (Length-1) etc

		if (IsConventionallyOutOfRange(index))
			throw new IndexOutOfRangeException("The index must be less than or equal to the buffer's length.");

		if (index == Length)
			return 0; // consumed the entire buffer

		ComputeLineStarts();
		int lineSep = GetNextOrCurrentLineStartPosition(index, out _);

		isCrLf = precededByCrLf.Contains(lineSep);

		return lineSep - index;
	}

	/// <exception cref="BufferException">
	/// The buffer is empty.
	/// </exception>
	/// <exception cref="IndexOutOfRangeException">
	/// <paramref name="index"/> was set to something greater than the buffer's length.
	/// </exception>
	/// <inheritdoc/>
	public int CountUntilNotWhitespace(int index)
	{
		if (IsEmpty)
			throw new BufferException("The buffer is empty.");

		if (index < 0)
			index = Length + index; // -1 is (Length-1) etc

		if (IsConventionallyOutOfRange(index))
			throw new IndexOutOfRangeException("The index must be less than or equal to the buffer's length.");
		
		if (index == Length)
			return 0; // consumed the entire buffer

		var span = data.AsSpan(index);
		int count = 0;

		while (count < span.Length && Char.IsWhiteSpace(span[count]))
			count++;

		return count;
	}

	/// <exception cref="BufferException">
	/// The buffer is empty.
	/// </exception>
	/// <exception cref="IndexOutOfRangeException">
	/// <paramref name="index"/> was set to something greater than the buffer's length.
	/// </exception>
	/// <inheritdoc/>
	public int CountUntilWhitespace(int index)
	{
		if (IsEmpty)
			throw new BufferException("The buffer is empty.");

		if (index < 0)
			index = Length + index; // -1 is (Length-1) etc

		if (IsConventionallyOutOfRange(index))
			throw new IndexOutOfRangeException("The index must be less than or equal to the buffer's length.");

		if (index == Length)
			return 0; // consumed the entire buffer

		var span = data.AsSpan(index);
		int count = 0;

		while (count < span.Length && !Char.IsWhiteSpace(span[count]))
			count++;

		return count;
	}

	/// <exception cref="BufferException">
	/// The buffer is empty.
	/// </exception>
	/// <exception cref="IndexOutOfRangeException">
	/// <paramref name="index"/> was set to something greater than the buffer's length.
	/// </exception>
	/// <inheritdoc/>
	public int CountWhile(Func<int, char, bool> predicate, int index)
	{
		if (IsEmpty)
			throw new BufferException("The buffer is empty.");

		if (index < 0)
			index = Length + index; // -1 is (Length-1) etc

		if (IsConventionallyOutOfRange(index))
			throw new IndexOutOfRangeException("The index must be less than or equal to the buffer's length.");

		if (index == Length)
			return 0; // consumed the entire buffer

		var span = data.AsSpan(index);
		int count = 0;

		while (count < span.Length && predicate(count, span[count]))
			count++;

		return count;
	}

	public int GetLengthOfLine(int lineNumber)
	{
		// todo: this should return the exact length of a line (#54)
		throw new NotImplementedException();
	}

	public int GetLengthOfLineAt(int index)
	{
		// todo: this should return the exact length of a line (#54)
		throw new NotImplementedException();
	}

	/// <exception cref="BufferException">
	/// The buffer is empty.
	/// </exception>
	/// <exception cref="IndexOutOfRangeException">
	/// <paramref name="index"/> was set to something greater than the buffer's length.
	/// </exception>
	/// <inheritdoc/>
	public int GetLineNumberForIndex(int index)
	{
		if (IsEmpty)
			throw new BufferException("The buffer is empty.");

		if (index < 0)
			index += Length;

		if (IsConventionallyOutOfRange(index))
			throw new IndexOutOfRangeException("The index must be less than or equal to the buffer's length.");

		if (index == Length)
			return LineCount;

		GetPreviousOrCurrentLineStartPosition(index, out int lineSepIndex);
		return lineSepIndex;
	}

	/// <inheritdoc/>
	public char[] Slice(int start, int length) => data.ToCharArray(start, length);

	/// <inheritdoc/>
	public void Slice(int start, Span<char> slice) => data.AsSpan(start, slice.Length).CopyTo(slice);

	/// <inheritdoc/>
	public void Slice(SourceSpan sourceSpan, Span<char> slice)
	{
		if (slice.Length < sourceSpan.Length)
			throw new ArgumentException("The slice's length is less than the span's length and therefore cannot contain the sliced region.", nameof(slice));

		data.AsSpan(sourceSpan.Start.Index, sourceSpan.Length).CopyTo(slice);
	}

	/// <inheritdoc/>
	public bool TryGetChar(int index, out char item)
	{
		item = '\0'; // default

		if (IsEmpty)
			return false;

		if (index < 0)
			index = Length + index; // -1 is (Length-1) etc

		if (IsOutOfRange(index))
			return false;

		item = data[index];

		return true;
	}

	/// <inheritdoc/>
	public bool TryGetLine(int lineNumber, Span<char> destination, out int itemCount)
	{
		itemCount = 0;

		if (IsEmpty || lineNumber < 0)
			return false;

		ComputeLineStarts();

		if (lineNumber >= lineStarts.Count)
			return false;

		int spanEnd = lineStarts[lineNumber];

		int spanStartIndex = lineNumber == 0
								 ? 0
								 : lineNumber - 1;

		if (precededByCrLf.Contains(lineStarts[spanStartIndex]))
			spanStartIndex++; // skip past the CR in the CRLF sequence

		spanStartIndex++; // get the actual start of the next line instead of the line sep

		int spanStart = lineStarts[spanStartIndex];
		int actualLineLength = spanEnd - spanStart + 1;
		itemCount = Math.Min(actualLineLength, destination.Length);

		data.AsSpan(spanStart, itemCount).CopyTo(destination);

		return actualLineLength > destination.Length;
	}

	/// <inheritdoc/>
	public bool TryGetLineFromIndex(int index, Span<char> line, out int itemCount)
	{
		itemCount = 0;

		if (IsEmpty)
			return false;

		if (index < 0)
			index += Length;

		if (IsConventionallyOutOfRange(index))
			return false;

		if (index == Length) // EOF means the current line is empty (line is empty by default)
			return true;

		ComputeLineStarts();
		int spanStart = GetPreviousOrCurrentLineStartPosition(index, out _);
		int spanEnd = GetNextLineStartPosition(index, out int nextLineStartIndex) - 1; // don't include the line separator

		if (precededByCrLf.Contains(nextLineStartIndex))
			spanEnd--; // remove one extra line separator if it's CRLF

		int actualLineLength = spanEnd - spanStart;
		itemCount = Math.Min(actualLineLength, line.Length);

		data.AsSpan(spanStart, itemCount).CopyTo(line);

		return actualLineLength <= line.Length;
	}

	/// <inheritdoc/>
	public bool TryGetLineAfterIndex(int index, Span<char> line, out int itemCount)
	{
		itemCount = 0;

		if (IsEmpty)
			return false;

		if (index < 0)
			index += Length;

		if (IsOutOfRange(index))
			return false;

		ComputeLineStarts();

		if (precededByCrLf.Contains(index - 1)) // is the current index pointing to a LF inside a CRLF?
			index--;                            // use the CR in the CRLF sequence instead of using the LF

		index += CountUntilLineSeparator(index, out bool isCrLf); // skip to the next line separator
		index += isCrLf ? 2 : 1; // skip to the actual start of the next line (skip twice if CRLF)

		if (IsOutOfRange(index))
			return false;

		// when summed with the index, returns the line separator index - which must be excluded from the return value
		int actualLineLength = CountUntilLineSeparator(index, out _);
		int charsToCopyAmount = Math.Min(actualLineLength, line.Length);
		data.AsSpan(index, charsToCopyAmount).CopyTo(line);

		itemCount = charsToCopyAmount;

		return line.Length >= actualLineLength;
	}

	/// <inheritdoc/>
	public bool TryGetLineAfterIndex(int index, Span<char> line, bool skipEmptyLines, out int itemCount)
	{
		// todo: this should return the next line (see TryGetNextLineFrom(int, Span<char>, int))
		// but skip empty lines if skipEmptyLines = true
		// see #52
		throw new NotImplementedException();
	}

	/// <remarks>
	/// This implementation, unlike others, is official and does not suffer from major performance issues.
	/// </remarks>
	/// <inheritdoc/>
	public bool TryGetSourceLocation(int index, out SourceLocation location)
	{
		location = new();

		if (IsEmpty)
			return false;

		if (index < 0)
			index += Length;

		if (IsOutOfRange(index))
			return false;

		if (index == 0) // best "best" case = triple zero
		{
			location = new(0, 0, 0);

			return true;
		}

		ComputeLineStarts();

		location = new(index, GetLineNumberForIndex(index), ReverseCountUntilNewline(index));

		return true;
	}

	/// <inheritdoc/>
	public bool TryGetWord(int index, char itemKind, Span<char> destination, out bool isItemKind, out int itemCount)
	{
		isItemKind = false;
		itemCount = 0;

		if (IsEmpty)
			return false;

		if (index < 0)
			index += Length;

		if (IsOutOfRange(index))
			return false;

		isItemKind = data[index] == itemKind;

		// if first index is KIND, the word is KIND (ends when not KIND/end of buffer)
		// if first index not KIND, the word is not KIND (ends on KIND/end of buffer)
		int actualLineLength = isItemKind
								   ? CountUntilNotMatch(index, itemKind)
								   : CountUntilMatch(index, itemKind);

		int charsToCopyAmount = Math.Min(actualLineLength, destination.Length);
		data.AsSpan(index, charsToCopyAmount).CopyTo(destination);

		itemCount = charsToCopyAmount;

		return destination.Length >= actualLineLength;
	}

	/// <inheritdoc/>
	public bool TryGetWord(int index, Span<char> destination, out bool isWhitespace, out int itemCount)
	{
		isWhitespace = false;
		itemCount = 0;

		if (IsEmpty)
			return false;

		if (index < 0)
			index += Length;

		if (IsOutOfRange(index))
			return false;

		isWhitespace = Char.IsWhiteSpace(data[index]);

		// if first index is whitespace, the word is whitespace (ends when not whitespace/end of buffer)
		// if first index not whitespace, the word is not whitespace (ends on whitespace/end of buffer)
		int actualLineLength = isWhitespace
								   ? CountUntilNotWhitespace(index)
								   : CountUntilWhitespace(index);

		int charsToCopyAmount = Math.Min(actualLineLength, destination.Length);
		data.AsSpan(index, charsToCopyAmount).CopyTo(destination);

		itemCount = charsToCopyAmount;

		return destination.Length >= actualLineLength;
	}

	/// <inheritdoc/>
	public bool TryGetWord(int index, Func<int, char, bool> itemKindPredicate, Span<char> destination, out bool isItemKind, out int itemCount)
	{
		isItemKind = false;
		itemCount = 0;

		if (IsEmpty)
			return false;

		if (index < 0)
			index += Length;

		if (IsOutOfRange(index))
			return false;

		isItemKind = itemKindPredicate(index, data[index]);

		// if first index is KIND, the word is KIND (ends when not KIND/end of buffer)
		// if first index not KIND, the word is not KIND (ends on KIND/end of buffer)
		int actualLineLength = isItemKind
								   ? CountWhile((i, c) => !itemKindPredicate(i, c), index)
								   : CountWhile(itemKindPredicate, index);

		int charsToCopyAmount = Math.Min(actualLineLength, destination.Length);
		data.AsSpan(index, charsToCopyAmount).CopyTo(destination);

		itemCount = charsToCopyAmount;

		return destination.Length >= actualLineLength;
	}

	/// <inheritdoc/>
	public void Dispose() => GC.SuppressFinalize(this);

	/// <inheritdoc/>
#if NET10_0_OR_GREATER
	public override bool Equals([NotNullWhen(true)] object? obj) =>
#elif NETSTANDARD2_0
	public override bool Equals(object obj) =>
#endif
		Equals(obj as ReadOnlyStringBuffer);

	/// <summary>
	/// Checks if another read-only buffer is equal to the current instance.
	/// </summary>
	/// <param name="other">The other read-only buffer.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(IReadOnlyBuffer<char>? other) => other is ReadOnlyStringBuffer buffer && Equals(buffer);

	/// <summary>
	/// Checks if another read-only string buffer is equal to the current instance.
	/// </summary>
	/// <param name="other">The other read-only string buffer.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(ReadOnlyStringBuffer? other) =>
		other is not null && data == other.data && Length == other.Length && IsEmpty == other.IsEmpty && LineCount == other.LineCount;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = InternalUtils.HashCodeSeed * InternalUtils.HashCodeMultiplier + EqualityComparer<string>.Default.GetHashCode(data);
			hashCode = hashCode * InternalUtils.HashCodeMultiplier + Length.GetHashCode();
			hashCode = hashCode * InternalUtils.HashCodeMultiplier + IsEmpty.GetHashCode();
			return hashCode * InternalUtils.HashCodeMultiplier + LineCount.GetHashCode();
		}
	}

	/// <summary>
	/// Returns the buffer's content as a <see cref="String"/>.
	/// </summary>
	/// <returns>The buffer's content.</returns>
	public override string ToString() => data;

	/// <summary>
	/// Checks if two read-only string buffers are equals.
	/// </summary>
	/// <returns>True if equals.</returns>
	public static bool operator ==(ReadOnlyStringBuffer left, ReadOnlyStringBuffer right) =>
		EqualityComparer<ReadOnlyStringBuffer>.Default.Equals(left, right);

	/// <summary>
	/// Checks if two read-only string buffers are different.
	/// </summary>
	/// <returns>True if different.</returns>
	public static bool operator !=(ReadOnlyStringBuffer left, ReadOnlyStringBuffer right) => !(left == right);

	private void ComputeLineStarts(bool forceCache = false)
	{
		if (CacheExists && !forceCache)
			return;

		var span = data.AsSpan();

		lineStarts.Clear();
		precededByCrLf.Clear();

		lineStarts.Capacity = Math.Max(lineStarts.Capacity, span.Length / 25 + 32); // just a rough guess
		precededByCrLf.Capacity = Math.Max(precededByCrLf.Capacity, span.Length / 25 + 16); // just a rough guess

		int lastIndex = span.Length - 1;
		int i = 0;

		lineStarts.Add(0); // 0 is by convention the start of line (and also the start of the buffer)

		while (i < lastIndex)
		{
			if (!span[i].IsNewline())
			{
				i++;
				continue;
			}

			bool isCrLf = i < lastIndex && span[i] == '\r' && span[i + 1] == '\n';
			int nextStart = i + (isCrLf ? 2 : 1);
			lineStarts.Add(nextStart);

			if (isCrLf)
				precededByCrLf.Add(nextStart);

			i = nextStart;
		}

		CacheExists = true;
	}

	private int CountUntilMatch(int index, char character)
	{
		if (index < 0)
			index = Length + index; // -1 is (Length-1) etc

		var span = data.AsSpan(index);
		int count = 0;

		while (count < span.Length && span[count] == character)
			count++;

		return count;
	}

	private int CountUntilNotMatch(int index, char character)
	{
		if (index < 0)
			index = Length + index; // -1 is (Length-1) etc

		var span = data.AsSpan(index);
		int count = 0;

		while (count < span.Length && span[count] != character)
			count++;

		return count;
	}

	/// <summary>
	/// Shared code context for <see cref="GetNextLineStartPosition(Int32, out Int32)"/> and
	/// <see cref="GetNextOrCurrentLineStartPosition(Int32, out Int32)"/>.
	/// </summary>
	/// <param name="insertionPoint">The insertion point of the next line start.</param>
	/// <param name="lsListIndex">The index, in <see cref="lineStarts"/> where the line start is.</param>
	/// <returns>The line start index, in <see cref="data"/>.</returns>
	private int GetNextLineStartFromInsertionPoint(int insertionPoint, out int lsListIndex)
	{
		if (insertionPoint >= 0)
		{
			// 1st Case: the used index (might have been index + 1 based on the method that called this)
			// points to an actual line start
			lsListIndex = insertionPoint;
			return lineStarts[insertionPoint];
		}

		int nextIndex = ~insertionPoint;

		if (nextIndex == lineStarts.Count)
		{
			// 2nd Case: the next line start is outside of the buffer (EOF convention)
			lsListIndex = -1;
			return Length;
		}

		lsListIndex = nextIndex;

		return lineStarts[nextIndex]; // 3rd Case: we found the next line start
	}

	private int GetNextLineStartPosition(int index, out int lsListIndex) =>
		// we use index + 1 to skip to the next line start if we're already standing on one :)
		GetNextLineStartFromInsertionPoint(lineStarts.BinarySearch(index + 1), out lsListIndex);
	private int GetNextOrCurrentLineStartPosition(int index, out int lsListIndex) =>
		// we use the actual index because we might already be standing on the line start
		GetNextLineStartFromInsertionPoint(lineStarts.BinarySearch(index), out lsListIndex);

	/// <summary>
	/// Shared code context for <see cref="GetPreviousLineStartPosition(Int32, out Int32)"/> and
	/// <see cref="GetPreviousOrCurrentLineStartPosition(Int32, out Int32)"/>.
	/// </summary>
	/// <param name="insertionPoint">The insertion point of the next line start.</param>
	/// <param name="lsListIndex">The index, in <see cref="lineStarts"/> where the line start is.</param>
	/// <returns>The line start index, in <see cref="data"/>.</returns>
	private int GetPreviousLineStartFromInsertionPoint(int insertionPoint, out int lsListIndex)
	{
		if (insertionPoint >= 0)
		{
			// 1st Case: the used index (might have been index + 1 based on the method that called this)
			// points to an actual line start
			lsListIndex = insertionPoint;
			return lineStarts[insertionPoint];
		}

		int previousIndex = ~insertionPoint;

		if (previousIndex == lineStarts.Count)
		{
			// 2nd Case: the next line start is outside of the buffer (EOF convention)
			// however we're gonna decrement one because otherwise all indexes from the last line that are after
			// start of said line suddenly become part of the EOF line
			lsListIndex = previousIndex - 1;
			return Length;
		}

		lsListIndex = previousIndex == 0 ? 0 : previousIndex - 1;

		return lineStarts[lsListIndex]; // 3rd Case: we found the previous line start
	}

	private int GetPreviousLineStartPosition(int index, out int lsListIndex)
	{
		if (index > 0)
		{
			// when the index is greater than 0, we can safely get the previous line start without getting a big shitty error
			// we use 0 as the start following standard conventions
			return GetPreviousLineStartFromInsertionPoint(lineStarts.BinarySearch(index - 1), out lsListIndex);
		}

		lsListIndex = 0;
		return 0;
	}

	private int GetPreviousOrCurrentLineStartPosition(int index, out int lsListIndex)
	{
		if (index > 0)
		{
			// when the index is greater than 0, we can safely get the previous line start without getting a big shitty error
			// we use 0 as the start following standard conventions
			return GetPreviousLineStartFromInsertionPoint(lineStarts.BinarySearch(index), out lsListIndex);
		}

		lsListIndex = 0;
		return 0;
	}

	/// <summary>
	/// True if index is greater than the length.
	/// This allows for the EOF convention, where the EOF is a possible output for
	/// some methods.
	/// Does not protect against <see cref="ArgumentOutOfRangeException"/> and
	/// <see cref="IndexOutOfRangeException"/>.
	/// </summary>
	private bool IsConventionallyOutOfRange(int index) => index > Length;

	/// <summary>
	/// True if index is greater than or equal to the length.
	/// This prevents throwing <see cref="ArgumentOutOfRangeException"/>
	/// or <see cref="IndexOutOfRangeException"/>.
	/// </summary>
	private bool IsOutOfRange(int index) => index >= Length;

	/// <remarks>
	/// Ignores the LF in CRLF sequences (returns the CR if the matching sequence is CRLF) - avoid double counting and other issues.
	/// </remarks>
	private int ReverseCountUntilNewline(int index)
	{
		if (index < 0)
			index = Length + index; // -1 is (Length-1) etc

		ComputeLineStarts();

		return index - GetPreviousLineStartPosition(index, out _);
	}
}
