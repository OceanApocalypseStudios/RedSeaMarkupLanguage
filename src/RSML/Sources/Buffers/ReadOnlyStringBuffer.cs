using System;
using System.Collections.Generic;
using System.Text;

using OceanApocalypseStudios.RSML.Common;


namespace OceanApocalypseStudios.RSML.Sources.Buffers;

/// <summary>
/// A read-only buffer backed by a string. All operations opt for performance
/// primarily via the internal use of <see cref="Span{Char}"/> over string allocations
/// and also via caching.
/// </summary>
public class ReadOnlyStringBuffer : IReadOnlyBuffer<char>,
									ISupportsCache
{
	private readonly List<int> lineSeparators = [ ];

	private readonly List<int> crFollowedByLf = [ ];

	private readonly string data;

	/// <inheritdoc/>
	public int Length => data.Length;

	/// <inheritdoc/>
	public bool IsEmpty => Length == 0;

	/// <inheritdoc/>
	public int LineCount
	{
		get
		{
			ComputeLineSeparators();

			return lineSeparators.Count;
		}
	}

	/// <inheritdoc/>
	public bool CacheExists { get; private set; } = false;

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
		data = encoding?.GetString(contentPtr, byteCount) ?? Encoding.Default.GetString(contentPtr, byteCount);

	private int GetLineSeparatorAfter(int index, out int lsListIndex)
	{
		int insertionPoint = lineSeparators.BinarySearch(index);

		lsListIndex = insertionPoint;

		if (insertionPoint >= 0)
			return lineSeparators[insertionPoint];

		lsListIndex = -1;

		if (~insertionPoint == lineSeparators.Count)
			return data.Length - 1; // last character

		lsListIndex = ~insertionPoint;

		return lineSeparators[~insertionPoint];
	}

	private int GetLineSeparatorBefore(int index, out int lsListIndex)
	{
		lsListIndex = -1;

		if (index == 0)
			return 0;

		int insertionPoint = lineSeparators.BinarySearch(index);

		lsListIndex = insertionPoint;

		if (insertionPoint >= 0)
			return lineSeparators[lsListIndex];

		lsListIndex = lineSeparators.Count - 1;

		if (~insertionPoint == lineSeparators.Count)
			return lineSeparators[lsListIndex];

		lsListIndex = ~insertionPoint - 1;

		if (lsListIndex == -1)
			return 0;

		return lineSeparators[lsListIndex];
	}

	private void ComputeLineSeparators(bool forceCache = false)
	{
		if (CacheExists && !forceCache)
			return;

		var span = data.AsSpan();

		lineSeparators.Clear();
		crFollowedByLf.Clear();

		lineSeparators.Capacity = Math.Max(lineSeparators.Capacity, span.Length / 25 + 32); // just a rough guess
		crFollowedByLf.Capacity = Math.Max(crFollowedByLf.Capacity, span.Length / 25 + 16); // just a rough guess

		int lastIndex = span.Length - 1;

		for (int i = 0; i < span.Length; i++)
		{
			if (!span[i].IsNewline())
				continue; // immediate continue brotato

			lineSeparators.Add(i);

			if (i < lastIndex && span[i] == '\r' && span[i + 1] == '\n')
			{
				crFollowedByLf.Add(i);
				i++; // skip the fucking LF associated to the CRLF
			}
		}

		CacheExists = true;
	}

	/// <inheritdoc/>
	public int CountUntilWhitespace(int index)
	{
		if (IsEmpty)
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
		if (index < 0)
			index = data.Length + index; // -1 is (Length-1) etc

		var span = data.AsSpan(index);
		int count = 0;

		while (count < span.Length && span[count] == character)
			count++;

		return count;
	}

	private int CountUntilNotMatch(int index, char character)
	{
		if (index < 0)
			index = data.Length + index; // -1 is (Length-1) etc

		var span = data.AsSpan(index);
		int count = 0;

		while (count < span.Length && span[count] != character)
			count++;

		return count;
	}

	/// <inheritdoc/>
	public int CountUntilLineSeparator(int index, out bool isCrLf)
	{
		isCrLf = false;

		if (IsEmpty)
			return 0;

		if (index < 0)
			index = data.Length + index; // -1 is (Length-1) etc

		if (IsOutOfBounds(index))
			return -1;

		if (data[index].IsNewline())
			return 0;

		ComputeLineSeparators();
		int lineSep = GetLineSeparatorAfter(index, out _);

		isCrLf = crFollowedByLf.Contains(lineSep);

		return lineSep - index;
	}

	/// <remarks>
	/// Ignores the LF in CRLF sequences (returns the CR if the matching sequence is CRLF) - avoid double counting and other issues.
	/// </remarks>
	private int ReverseCountUntilNewline(int index)
	{
		if (index < 0)
			index = data.Length + index; // -1 is (Length-1) etc

		ComputeLineSeparators();

		return index - GetLineSeparatorBefore(index, out _);
	}

	private bool IsOutOfBounds(int index) => index >= data.Length;

	/// <inheritdoc/>
	public int CountWhile(Func<int, char, bool> predicate, int index)
	{
		if (IsEmpty)
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

		if (IsEmpty)
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
	public bool TryGetNextLineFrom(int index, Span<char> line, out int itemCount)
	{
		itemCount = 0;

		if (IsEmpty)
			return false;

		if (index < 0)
			index += data.Length;

		if (IsOutOfBounds(index))
			return false;

		ComputeLineSeparators();

		if (crFollowedByLf.Contains(index - 1)) // is the current index pointing to a LF inside a CRLF?
			index--;                            // use the CR in the CRLF sequence instead of using the LF

		index += CountUntilLineSeparator(index, out bool isCrLf); // skip to the next line separator
		index += isCrLf ? 2 : 1; // skip to the actual start of the next line (skip twice if CRLF)

		if (IsOutOfBounds(index))
			return false;

		// when summed with the index, returns the line separator index - which must be excluded from the return value
		int actualLineLength = CountUntilLineSeparator(index, out _);
		int charsToCopyAmount = Math.Min(actualLineLength, line.Length);
		data.AsSpan(index, charsToCopyAmount).CopyTo(line);

		itemCount = charsToCopyAmount;

		return line.Length >= actualLineLength;
	}

	/// <inheritdoc/>
	public bool TryGetWord(int index, Span<char> destination, out bool isWhitespace, out int itemCount)
	{
		isWhitespace = false;
		itemCount = 0;

		if (IsEmpty)
			return false;

		if (index < 0)
			index += data.Length;

		if (IsOutOfBounds(index))
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
	public bool TryGetWord(int index, char itemKind, Span<char> destination, out bool isItemKind, out int itemCount)
	{
		isItemKind = false;
		itemCount = 0;

		if (IsEmpty)
			return false;

		if (index < 0)
			index += data.Length;

		if (IsOutOfBounds(index))
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
	public int CountUntilNotWhitespace(int index)
	{
		if (IsEmpty)
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
	public void Dispose() => GC.SuppressFinalize(this);

	/// <inheritdoc/>
	public bool TryGetWord(int index, Func<int, char, bool> itemKindPredicate, Span<char> destination, out bool isItemKind, out int itemCount)
	{
		isItemKind = false;
		itemCount = 0;

		if (IsEmpty)
			return false;

		if (index < 0)
			index += data.Length;

		if (IsOutOfBounds(index))
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
	public void Slice(int start, Span<char> slice) => data.AsSpan(start, slice.Length).CopyTo(slice);

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
			index += data.Length;

		if (IsOutOfBounds(index))
			return false;

		if (index == 0) // best "best" case = triple zero
		{
			location = new(0, 0, 0);

			return true;
		}

		ComputeLineSeparators();

		location = new(index, CountLinesBefore(index), ReverseCountUntilNewline(index));

		return true;
	}

	internal int CountLinesBefore(int index)
	{
		if (index == data.Length - 1)
			return LineCount;

		GetLineSeparatorBefore(index, out int lineSepIndex);

		return lineSepIndex + 1; // the amount of lines is the amount of line separators plus 1 ALWAYS
	}

	/// <inheritdoc/>
	public bool TryGetLine(int lineNumber, Span<char> destination, out int itemCount)
	{
		itemCount = 0;

		if (IsEmpty || lineNumber < 0)
			return false;

		ComputeLineSeparators();

		if (lineNumber >= lineSeparators.Count)
			return false;

		int spanEnd = lineSeparators[lineNumber];

		int spanStartIndex = lineNumber == 0
								 ? 0
								 : lineNumber - 1;

		if (crFollowedByLf.Contains(lineSeparators[spanStartIndex]))
			spanStartIndex++; // skip past the CR in the CRLF sequence

		spanStartIndex++; // get the actual start of the next line instead of the line sep

		int spanStart = lineSeparators[spanStartIndex];
		int actualLineLength = spanEnd - spanStart + 1;
		itemCount = Math.Min(actualLineLength, destination.Length);

		data.AsSpan(spanStart, itemCount).CopyTo(destination);

		return actualLineLength > destination.Length;
	}

	/// <inheritdoc/>
	public bool TryGetLineAt(int index, Span<char> line, out int itemCount)
	{
		itemCount = 0;

		if (IsEmpty)
			return false;

		if (index < 0)
			index += data.Length;

		if (IsOutOfBounds(index))
			return false;

		ComputeLineSeparators();

		int spanStart = GetLineSeparatorBefore(index, out int lineSepBeforeIndex);

		if (lineSepBeforeIndex != -1) // only skip necessary characters if the start of the span isn't 0
		{
			if (crFollowedByLf.Contains(spanStart))
				spanStart++; // skip the LF in the CRLF sequence (if applicable)

			if (index < data.Length - 1)
				spanStart++; // skip to the actual start of the line (but don't error out doing so)
		}

		int spanEnd = GetLineSeparatorAfter(index, out _);

		if (spanEnd > 0)
			spanEnd--; // don't include the line separator

		if (spanStart < spanEnd)
			spanStart = spanEnd; // empty span

		int actualLineLength = spanEnd - spanStart + 1;
		itemCount = Math.Min(actualLineLength, line.Length);

		data.AsSpan(spanStart, itemCount).CopyTo(line);

		return actualLineLength <= line.Length;
	}

	/// <inheritdoc/>
	public void Slice(SourceSpan sourceSpan, Span<char> slice)
	{
		if (slice.Length < sourceSpan.Length)
			throw new ArgumentException("The slice's length is less than the span's length and therefore cannot contain the sliced region.", nameof(slice));

		data.AsSpan(sourceSpan.Start.Index, sourceSpan.Length).CopyTo(slice);
	}

	public int GetLengthOfLine(int lineNumber)
	{
		// todo: this should return the minimum length of a line (#54)
		throw new NotImplementedException();
	}

	public int GetLengthOfLineAt(int index)
	{
		// todo: this should return the minimum length of a line (#54)
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public bool TryGetNextLineFrom(int index, Span<char> line, bool skipEmptyLines, out int itemCount)
	{
		// todo: this should return the next line (see TryGetNextLineFrom(int, Span<char>, int))
		// but skip empty lines if skipEmptyLines = true
		// see #52
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public void BuildCache() => ComputeLineSeparators();

	/// <inheritdoc/>
	public void BuildCache(bool forceRebuild) => ComputeLineSeparators(forceRebuild);
}
