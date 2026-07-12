using System;
using System.Collections.Generic;
using System.Text;

using OceanApocalypseStudios.RSML.Cache;
using OceanApocalypseStudios.RSML.Exceptions;


namespace OceanApocalypseStudios.RSML.Sources;

/// <summary>
/// A read-only buffer backed by a string. All operations opt for performance
/// primarily via the internal use of <see cref="ReadOnlySpan{Char}"/> over string allocations
/// and also via caching.
/// </summary>
public class ReadOnlyStringBuffer : IBuffer<char>, ISupportsCache, IEquatable<ReadOnlyStringBuffer?>, IEquatable<string?>
{
	private readonly List<int> lineStarts = [];

	private readonly List<int> precededByCrLf = [];

	private readonly string data;

	/// <inheritdoc/>
	public bool CacheExists { get; private set; } = false;

	/// <remarks>
	/// Always returns <c>-1</c>, as <see cref="ReadOnlyStringBuffer"/> doesn't
	/// support cursor positioning (everything is done via indexing).
	/// </remarks>
	/// <inheritdoc/>
	public int CursorIndex => -1;

	/// <inheritdoc/>
	public bool IsEmpty => Length == 0;

	/// <remarks>
	/// Always returns <c>true</c>, as <see cref="ReadOnlyStringBuffer"/> only
	/// supports read-only content (hence the name).
	/// </remarks> 
	/// <inheritdoc/>
	public bool IsReadOnly => true;

	/// <inheritdoc/>
	public int Length => data.Length;

	/// <remarks>
	/// <see cref="LineCount"/> automatically builds cache if
	/// no cached data exists. No <see cref="BuildCache()"/> calls
	/// are necessary.
	/// </remarks>
	/// <inheritdoc/>
	public int LineCount => RawLineCount - 1; // ignore the "fake" EOF line (convention)

	private int RawLineCount
	{
		get
		{
			ComputeLineStarts();

			return lineStarts.Count;
		}
	}

	/// <inheritdoc/>
	public char this[int index] => data[index];

	/// <inheritdoc/>
	public char this[SourceLocation location] => this[location.Index];

	/// <inheritdoc/>
	public char[] this[SourceSpan span] => Slice(span.Start.Index, span.Length);

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
	/// <remarks>
	/// > [!NOTE]
	/// > This method allows the EOF index as in-range. The convention is as follows:
	/// > - If the index is EOF (<see cref="Length"/>), then the output is always 0 and <paramref name="isCrLf"/> is always <c>false</c>.
	/// > - If the index is the last (<see cref="Length"/> - 1), then the output is always 0.
	/// > [!NOTE]
	/// > <paramref name="isCrLf"/> is only <c>true</c> if all the following conditions are true:
	/// > - The next line start counting from <paramref name="index"/> is preceded by a CRLF sequence.
	/// > - <paramref name="index"/> does not point to the LF in the CRLF sequence.
	/// > - <paramref name="index"/> does not point to EOF.
	/// </remarks>
	/// <inheritdoc/>
	public int CountUntilLineSeparator(int index, out bool isCrLf)
	{
		isCrLf = false;

		if (IsEmpty)
			throw new BufferException("The buffer cannot be empty.");

		if (index < 0)
			index = Length + index; // -1 is (Length-1) etc

		if (IsConventionallyOutOfRange(index))
			throw new IndexOutOfRangeException("The index must be less than or equal to the buffer's length.");

		if (index == Length)
			return 0; // consumed the entire buffer

		ComputeLineStarts();
		int lineStart = GetNextLineStartPosition(index, out _);
		int lineSep = lineStart - 1;
		isCrLf = precededByCrLf.Contains(lineStart) && data[index] is not '\n'; // to us, CRLF is only when we're not standing on the LF

		if (isCrLf)
			lineSep--; // skip the extra line separator in the CRLF sequence		

		return lineSep - index;
	}

	/// <exception cref="BufferException">
	/// The buffer is empty.
	/// </exception>
	/// <exception cref="IndexOutOfRangeException">
	/// <paramref name="index"/> was set to something greater than the buffer's length.
	/// </exception>
	/// <remarks>
	/// > [!NOTE]
	/// > This method allows the EOF index as in-range.
	/// > If the index is EOF (<see cref="Length"/>), then the output is always 0.
	/// </remarks>
	/// <inheritdoc/>
	public int CountUntilNotWhitespace(int index)
	{
		if (IsEmpty)
			throw new BufferException("The buffer cannot be empty.");

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
	/// <remarks>
	/// > [!NOTE]
	/// > This method allows the EOF index as in-range.
	/// > If the index is EOF (<see cref="Length"/>), then the output is always 0.
	/// </remarks>
	/// <inheritdoc/>
	public int CountUntilWhitespace(int index)
	{
		if (IsEmpty)
			throw new BufferException("The buffer cannot be empty.");

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
	/// <remarks>
	/// > [!NOTE]
	/// > This method allows the EOF index as in-range.
	/// > If the index is EOF (<see cref="Length"/>), then the output is always 0.
	/// > [!NOTE]
	/// > The return value, when summed with <paramref name="index"/>, becomes the index of the first character that
	/// > fails to verify the <paramref name="predicate"/>, counting from <paramref name="index"/>.
	/// > The only exception is if the buffer has been consumed (you pass EOF index or there's no more characters that fail to verify
	/// > the <paramref name="predicate"/>), meaning the return value, when summed with <paramref name="index"/> is the value of
	/// > <see cref="Length"/>, which is also the EOF index.
	/// </remarks>
	/// <inheritdoc/>
	public int CountWhile(Func<int, char, bool> predicate, int index)
	{
		if (IsEmpty)
			throw new BufferException("The buffer cannot be empty.");

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

	/// <exception cref="BufferException">
	/// The buffer is empty.
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// The given <paramref name="lineNumber"/> is negative or
	/// is greater than the amount of lines (see <see cref="LineCount"/>).
	/// </exception>
	/// <remarks>
	/// > [!NOTE]
	/// > This method follows EOF conventions.
	/// > EOF is considered a 0-character sequence in line N, where N is <see cref="LineCount"/>.
	/// > Keep in mind N does not point to an actual line (it's just a convention), as line numbers are 0-based
	/// > (meaning the actual last line is located at N - 1).
	/// </remarks>
	/// <inheritdoc/>
	public int GetLengthOfLine(int lineNumber)
	{
		if (IsEmpty)
			throw new BufferException("The buffer cannot be empty.");

		ComputeLineStarts();

		if (lineNumber < 0 || lineNumber >= RawLineCount)
			throw new ArgumentOutOfRangeException(nameof(lineNumber), "The 0-based line number must be non-negative and less than the amount of lines in the buffer.");

		if (lineNumber == LineCount)
			return 0; // EOF means the line is empty

		int start = lineStarts[lineNumber];
		int end = lineStarts[lineNumber + 1];

		if (precededByCrLf.Contains(end))
			end--; // skip the extra line separator in the CRLF sequence

		if (!(lineNumber + 2 == RawLineCount && !data[Length - 1].IsNewline())) // if we're not on the last line then
			end--; // skip one more line separator

		return end - start;
	}

	/// <inheritdoc/>
	public int GetLengthOfLineFromIndex(int index) => GetLengthOfLine(GetLineNumberFromIndex(index));

	/// <exception cref="BufferException">
	/// The buffer is empty.
	/// </exception>
	/// <exception cref="IndexOutOfRangeException">
	/// <paramref name="index"/> was set to something greater than the buffer's length.
	/// </exception>
	/// <inheritdoc/>
	public int GetLineNumberFromIndex(int index)
	{
		if (IsEmpty)
			throw new BufferException("The buffer cannot be empty.");

		if (index < 0)
			index += Length;

		if (IsConventionallyOutOfRange(index))
			throw new IndexOutOfRangeException("The index must be less than or equal to the buffer's length.");

		ComputeLineStarts();

		GetPreviousOrCurrentLineStartPosition(index, out int lineSepIndex);
		return lineSepIndex;
	}

	/// <inheritdoc/>
	public char[] GetLine(int lineNumber)
	{
		if (IsEmpty)
			throw new BufferException("The buffer cannot be empty.");

		ComputeLineStarts();

		if (lineNumber < 0 || lineNumber >= RawLineCount)
			throw new ArgumentOutOfRangeException(nameof(lineNumber), "The 0-based line number must be non-negative and less than the amount of lines in the buffer.");

		if (lineNumber + 1 == RawLineCount)
			return []; // EOF means the line is empty

		int start = lineStarts[lineNumber];
		int length = GetLengthOfLine(lineNumber);

		return data.AsSpan(start, length).ToArray();
	}

	/// <inheritdoc/>
	public char[] GetLineFromIndex(int index) => GetLine(GetLineNumberFromIndex(index));

	/// <remarks>
	/// Unlike with other <see cref="ReadOnlyStringBuffer"/> methods, this one
	/// does not accept the EOF index (index at <see cref="Length"/>), because it is not
	/// considered a location.
	/// </remarks>
	/// <inheritdoc/>
	public SourceLocation GetSourceLocation(int index)
	{
		if (IsEmpty)
			throw new BufferException("The buffer cannot be empty.");

		if (index < 0)
			index += Length;

		if (IsOutOfRange(index))
			throw new IndexOutOfRangeException("The index must be less than or equal to the buffer's length.");

		if (index == 0) // best "best" case = triple zero
			return new(0, 0, 0);

		ComputeLineStarts();
		int lineNumber = GetLineNumberFromIndex(index);

		if (lineNumber >= lineStarts.Count)
			throw new IndexOutOfRangeException("The index points to a line number that is out of range.");

		return new(index, lineNumber, index - lineStarts[lineNumber]);
	}

	/// <remarks>
	/// Unlike with other <see cref="ReadOnlyStringBuffer"/> methods, this one
	/// does not accept the EOF index (index at <see cref="Length"/>), because it is not
	/// considered part of any word.
	/// </remarks>
	/// <inheritdoc/>
	public char[] GetWord(int index, out bool isWhitespace)
	{
		isWhitespace = false;

		if (IsEmpty)
			throw new BufferException("The buffer cannot be empty.");

		if (index < 0)
			index += Length;

		if (IsOutOfRange(index))
			throw new IndexOutOfRangeException("The index must be less than the buffer's length.");

		isWhitespace = Char.IsWhiteSpace(data[index]);

		// if first index is whitespace, the word is whitespace (ends when not whitespace/end of buffer)
		// if first index not whitespace, the word is not whitespace (ends on whitespace/end of buffer)
		int length = isWhitespace
							? CountUntilNotWhitespace(index)
							: CountUntilWhitespace(index);

		return data.AsSpan(index, length).ToArray();
	}

	/// <remarks>
	/// Unlike with other <see cref="ReadOnlyStringBuffer"/> methods, this one
	/// does not accept the EOF index (index at <see cref="Length"/>), because it is not
	/// considered part of any word.
	/// </remarks>
	/// <inheritdoc/>
	public char[] GetWord(int index, char itemKind, out bool isItemKind)
	{
		isItemKind = false;

		if (IsEmpty)
			throw new BufferException("The buffer cannot be empty.");

		if (index < 0)
			index += Length;

		if (IsOutOfRange(index))
			throw new IndexOutOfRangeException("The index must be less than the buffer's length.");

		isItemKind = data[index] == itemKind;

		// if first index is KIND, the word is KIND (ends when not KIND/end of buffer)
		// if first index not KIND, the word is not KIND (ends on KIND/end of buffer)
		int length = isItemKind
							? CountUntilNotMatch(index, itemKind)
							: CountUntilMatch(index, itemKind);

		return data.AsSpan(index, length).ToArray();
	}

	/// <remarks>
	/// Unlike with other <see cref="ReadOnlyStringBuffer"/> methods, this one
	/// does not accept the EOF index (index at <see cref="Length"/>), because it is not
	/// considered part of any word.
	/// </remarks>
	/// <inheritdoc/>
	public char[] GetWord(int index, Func<int, char, bool> itemKindPredicate, out bool isItemKind)
	{
		isItemKind = false;

		if (IsEmpty)
			throw new BufferException("The buffer cannot be empty.");

		if (index < 0)
			index += Length;

		if (IsOutOfRange(index))
			throw new IndexOutOfRangeException("The index must be less than the buffer's length.");

		isItemKind = itemKindPredicate(index, data[index]);

		// if first index is KIND, the word is KIND (ends when not KIND/end of buffer)
		// if first index not KIND, the word is not KIND (ends on KIND/end of buffer)
		int length = isItemKind
							? CountWhile((i, c) => !itemKindPredicate(i, c), index)
							: CountWhile(itemKindPredicate, index);

		return data.AsSpan(index, length).ToArray();
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

		ComputeLineStarts();

		if (IsEmpty || lineNumber < 0 || lineNumber >= RawLineCount)
			return false;

		if (lineNumber + 1 == RawLineCount)
			return true; // EOF means the line is empty (and destination is by default empty)

		int start = lineStarts[lineNumber];
		int actualLineLength = GetLengthOfLine(lineNumber);
		itemCount = Math.Min(actualLineLength, destination.Length);

		data.AsSpan(start, itemCount).CopyTo(destination);

		return actualLineLength <= destination.Length;
	}

	/// <inheritdoc/>
	public bool TryGetLineFromIndex(int index, Span<char> line, out int itemCount) =>
		TryGetLine(GetLineNumberFromIndex(index), line, out itemCount);

	/// <remarks>
	/// Unlike with other <see cref="ReadOnlyStringBuffer"/> methods, this one
	/// does not accept the EOF index (index at <see cref="Length"/>), because it is not
	/// considered part of any word.
	/// </remarks>
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

		itemCount = Math.Min(actualLineLength, destination.Length);
		data.AsSpan(index, itemCount).CopyTo(destination);;

		return destination.Length >= actualLineLength;
	}
	
	/// <remarks>
	/// Unlike with other <see cref="ReadOnlyStringBuffer"/> methods, this one
	/// does not accept the EOF index (index at <see cref="Length"/>), because it is not
	/// considered part of any word.
	/// </remarks>
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

		itemCount = Math.Min(actualLineLength, destination.Length);
		data.AsSpan(index, itemCount).CopyTo(destination);

		return destination.Length >= actualLineLength;
	}

	/// <remarks>
	/// Unlike with other <see cref="ReadOnlyStringBuffer"/> methods, this one
	/// does not accept the EOF index (index at <see cref="Length"/>), because it is not
	/// considered part of any word.
	/// </remarks>
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

		itemCount = Math.Min(actualLineLength, destination.Length);
		data.AsSpan(index, itemCount).CopyTo(destination);

		return destination.Length >= actualLineLength;
	}

	/// <inheritdoc/>
	public void Dispose() => GC.SuppressFinalize(this);

	/// <inheritdoc/>
	public override bool Equals(
#if NET8_0_OR_GREATER
		[NotNullWhen(true)]
		object? obj
#else
		object obj
#endif
	) => obj switch
	{
		string str => Equals(str),
		ReadOnlyStringBuffer readOnlyStringBuffer => Equals(readOnlyStringBuffer),
		IBuffer<char> buffer => Equals(buffer),
		Memory<char> memory => Equals(memory),
		ReadOnlyMemory<char> readOnlyMemory => Equals(readOnlyMemory),
		null => false,
		_ => false
	};

	/// <summary>
	/// Checks if an array of characters is equal to the current instance.
	/// </summary>
	/// <param name="other">The array.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(char[]? other) => other is not null && data.Equals(other.AsSpan(), StringComparison.Ordinal);

	/// <summary>
	/// Checks if another read-only buffer is equal to the current instance.
	/// </summary>
	/// <param name="other">The other read-only buffer.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(IBuffer<char>? other) => other is ReadOnlyStringBuffer buffer && Equals(buffer);

	/// <summary>
	/// Checks if a contiguous region of memory is equal to the current instance.
	/// </summary>
	/// <param name="other">The region of memory.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(Memory<char>? other) => other is not null && Length == other.Value.Length && data.Equals(other.Value.Span, StringComparison.Ordinal);

	/// <summary>
	/// Checks if a read-only contiguous region of memory is equal to the current instance.
	/// </summary>
	/// <param name="other">The region of memory.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(ReadOnlyMemory<char>? other) => other is not null && Length == other.Value.Length && data.Equals(other.Value.Span, StringComparison.Ordinal);

	/// <summary>
	/// Checks if a read-only contiguous region of memory is equal to the current instance.
	/// </summary>
	/// <param name="other">The region of memory.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(ReadOnlySpan<char> other) => Length == other.Length && data.Equals(other, StringComparison.Ordinal);

	/// <summary>
	/// Checks if another read-only string buffer is equal to the current instance.
	/// </summary>
	/// <param name="other">The other read-only string buffer.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(ReadOnlyStringBuffer? other) =>
		other is not null && data == other.data && Length == other.Length && IsEmpty == other.IsEmpty && LineCount == other.LineCount;

	/// <summary>
	/// Checks if a contiguous region of memory is equal to the current instance.
	/// </summary>
	/// <param name="other">The region of memory.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(Span<char> other) => data.Equals(other, StringComparison.Ordinal);

	/// <summary>
	/// Checks if a string is equal to the current instance.
	/// </summary>
	/// <param name="other">The string.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(string? other) =>
		other is not null && Length == other.Length && data == other;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = Constants.HashCodeSeed * Constants.HashCodeMultiplier + EqualityComparer<string>.Default.GetHashCode(data);
			hashCode = hashCode * Constants.HashCodeMultiplier + Length.GetHashCode();
			hashCode = hashCode * Constants.HashCodeMultiplier + IsEmpty.GetHashCode();
			return hashCode * Constants.HashCodeMultiplier + LineCount.GetHashCode();
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

		lineStarts.Add(Length); // add the EOF as the start of a line

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
			lsListIndex = nextIndex;
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
			// however this might also mean we're at last character
			lsListIndex = previousIndex;
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
	private bool IsConventionallyOutOfRange(int index) => index < 0 || index > Length;

	/// <summary>
	/// True if index is greater than or equal to the length.
	/// This prevents throwing <see cref="ArgumentOutOfRangeException"/>
	/// or <see cref="IndexOutOfRangeException"/>.
	/// </summary>
	private bool IsOutOfRange(int index) => index < 0 || index >= Length;
}
