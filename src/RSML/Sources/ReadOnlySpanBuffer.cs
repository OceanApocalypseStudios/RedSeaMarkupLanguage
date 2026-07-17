using System;
using System.Collections.Generic;

using OceanApocalypseStudios.RSML.Cache;
using OceanApocalypseStudios.RSML.Diagnostics;


namespace OceanApocalypseStudios.RSML.Sources;

// xxx: this struct is to be kept up-to-date all the time with the ReadOnlyCharBuffer class

/// <summary>
/// A read-only buffer backed by a span of characters. All operations opt for performance
/// primarily via the internal use of <see cref="ReadOnlySpan{Char}"/> over string allocations
/// and also via caching.
/// </summary>
/// <remarks>
/// > [!TIP]
/// > The main advantage of this type over <see cref="ReadOnlyStringBuffer"/>
/// > is that you don't need to allocate a class and, on initialization, you don't need to allocate
/// > a string.
/// </remarks>
public ref struct ReadOnlySpanBuffer : IBuffer, ISupportsCache
{
	private readonly ReadOnlySpan<char> data;
	private ReadOnlySpan<int> precededByCrLf;
	private ReadOnlySpan<int> lineStarts;

	/// <inheritdoc/>
	public bool CacheExists { get; private set; } = false;

	/// <remarks>
	/// Always returns <c>-1</c>, as <see cref="ReadOnlySpanBuffer"/> doesn't
	/// support cursor positioning (everything is done via indexing).
	/// </remarks>
	/// <inheritdoc/>
	public readonly int CursorIndex => -1;

	/// <inheritdoc/>
	public readonly bool IsEmpty => Length == 0;

	/// <remarks>
	/// Always returns <c>true</c>, as <see cref="ReadOnlySpanBuffer"/> only
	/// supports read-only content (hence the name).
	/// </remarks> 
	/// <inheritdoc/>
	public readonly bool IsReadOnly => true;

	/// <inheritdoc/>
	public readonly int Length => data.Length;

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

			return lineStarts.Length;
		}
	}

	/// <inheritdoc/>
	public readonly char? this[int index] => TryGetChar(index, out char item) ? item : null;

	/// <inheritdoc/>
	public readonly char? this[SourceLocation location] => this[location.Index];

	/// <inheritdoc/>
	public readonly Result<string> this[SourceSpan span] => Slice(span.Start.Index, span.Length);

	/// <summary>
	/// Initializes a new <see cref="ReadOnlySpanBuffer"/>
	/// with a string.
	/// </summary>
	/// <param name="content">The string that the buffer will wrap.</param>
	public ReadOnlySpanBuffer(string content) => data = content.AsSpan();

	/// <summary>
	/// Initializes a new <see cref="ReadOnlySpanBuffer"/> with a span.
	/// </summary>
	/// <param name="content">The span pointing to the string's data.</param>
	public ReadOnlySpanBuffer(ReadOnlySpan<char> content) => data = content;

	/// <summary>
	/// Initializes a new <see cref="ReadOnlySpanBuffer"/>
	/// with an array of characters.
	/// </summary>
	/// <param name="content">The array of characters to use for the buffer.</param>
	public ReadOnlySpanBuffer(char[] content) => data = new(content);

	/// <inheritdoc/>
	public void BuildCache() => ComputeLineStarts();

	/// <inheritdoc/>
	public void BuildCache(bool forceRebuild) => ComputeLineStarts(forceRebuild);

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
	public Result<int> CountUntilEndOfLine(int index, out bool isCrLf)
	{
		isCrLf = false;

		if (IsEmpty)
			return Result<int>.Fail(new(new(ErrorCategory.Internal, 1), SourceSpan.Empty, "The buffer is empty.", Severity.None));

		if (index < 0)
			index += Length;

		if (IsConventionallyOutOfRange(index))
			return Result<int>.Fail(new(new(ErrorCategory.Internal, 2), SourceSpan.Empty, "The index is greater than the buffer's length.", Severity.None));

		if (index == Length)
			return Result<int>.Success(0); // consumed the entire buffer

		ComputeLineStarts();

		int lineSep = GetNextLineStartPosition(index, out _);
#if NET8_0_OR_GREATER
		isCrLf = precededByCrLf.Contains(lineSep) && data[index] is not '\n'; // to us, CRLF is only when we're not standing on the LF
#else
		isCrLf = precededByCrLf.IndexOf(lineSep) != -1 && data[index] is not '\n';
#endif

		if (isCrLf)
			lineSep--; // skip the extra line separator in the CRLF sequence

#if NET8_0_OR_GREATER
		if (!(IsLastLine(index) && !data[^1].IsNewline())) // if we're not on the last line and it doesn't end with a newline then
#else
		if (!(IsLastLine(index) && !data[Length - 1].IsNewline()))
#endif
		{
			lineSep--;
		}

		return Result<int>.Success(lineSep - index);
	}

	/// <remarks>
	/// > [!NOTE]
	/// > This method allows the EOF index as in-range.
	/// > If the index is EOF (<see cref="Length"/>), then the output is always 0.
	/// > [!NOTE]
	/// > The return value, when summed with <paramref name="index"/>, becomes the index of the first character that
	/// > is not whitespace, counting from <paramref name="index"/>.
	/// > The only exception is if the buffer has been consumed (you pass EOF index or there's no more characters that are
	/// > not whitespace), meaning the return value, when summed with <paramref name="index"/> is the value of
	/// > <see cref="Length"/>, which is also the EOF index.
	/// </remarks>
	/// <inheritdoc/>
	public readonly Result<int> CountUntilNotWhitespace(int index)
	{
		if (IsEmpty)
			return Result<int>.Fail(new(new(ErrorCategory.Internal, 1), SourceSpan.Empty, "The buffer is empty.", Severity.None));

		if (index < 0)
			index = Length + index; // -1 is (Length-1) etc

		if (IsConventionallyOutOfRange(index))
			return Result<int>.Fail(new(new(ErrorCategory.Internal, 2), SourceSpan.Empty, "The index is greater than the buffer's length.", Severity.None));

		if (index == Length)
			return Result<int>.Success(0); // consumed the entire buffer

		var span = data.Slice(index);
		int count = 0;

		while (count < span.Length && Char.IsWhiteSpace(span[count]))
			count++;

		return Result<int>.Success(count);
	}

	/// <remarks>
	/// > [!NOTE]
	/// > This method allows the EOF index as in-range.
	/// > If the index is EOF (<see cref="Length"/>), then the output is always 0.
	/// > [!NOTE]
	/// > The return value, when summed with <paramref name="index"/>, becomes the index of the first character that
	/// > is whitespace, counting from <paramref name="index"/>.
	/// > The only exception is if the buffer has been consumed (you pass EOF index or there's no more characters that are
	/// > whitespace), meaning the return value, when summed with <paramref name="index"/> is the value of
	/// > <see cref="Length"/>, which is also the EOF index.
	/// </remarks>
	/// <inheritdoc/>
	public readonly Result<int> CountUntilWhitespace(int index)
	{
		if (IsEmpty)
			return Result<int>.Fail(new(new(ErrorCategory.Internal, 1), SourceSpan.Empty, "The buffer is empty.", Severity.None));

		if (index < 0)
			index = Length + index; // -1 is (Length-1) etc

		if (IsConventionallyOutOfRange(index))
			return Result<int>.Fail(new(new(ErrorCategory.Internal, 2), SourceSpan.Empty, "The index musis greater than the buffer's length.", Severity.None));

		if (index == Length)
			return Result<int>.Success(0); // consumed the entire buffer

		var span = data.Slice(index);
		int count = 0;

		while (count < span.Length && !Char.IsWhiteSpace(span[count]))
			count++;

		return Result<int>.Success(count);
	}

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
	public readonly Result<int> CountWhile(Func<int, char, bool> predicate, int index)
	{
		if (IsEmpty)
			return Result<int>.Fail(new(new(ErrorCategory.Internal, 1), SourceSpan.Empty, "The buffer is empty.", Severity.None));

		if (index < 0)
			index = Length + index; // -1 is (Length-1) etc

		if (IsConventionallyOutOfRange(index))
			return Result<int>.Fail(new(new(ErrorCategory.Internal, 2), SourceSpan.Empty, "The index is greater than the buffer's length.", Severity.None));

		if (index == Length)
			return Result<int>.Success(0); // consumed the entire buffer

		var span = data.Slice(index);
		int count = 0;

		while (count < span.Length && predicate(count, span[count]))
			count++;

		return Result<int>.Success(count);
	}

	/// <remarks>
	/// > [!NOTE]
	/// > This method follows EOF conventions.
	/// > EOF is considered a 0-character sequence in line N, where N is <see cref="LineCount"/>.
	/// > Keep in mind N does not point to an actual line (it's just a convention), as line numbers are 0-based
	/// > (meaning the actual last line is located at N - 1).
	/// </remarks>
	/// <inheritdoc/>
	public Result<int> GetLengthOfLine(int lineNumber)
	{
		if (IsEmpty)
			return Result<int>.Fail(new(new(ErrorCategory.Internal, 1), SourceSpan.Empty, "The buffer is empty.", Severity.None));

		ComputeLineStarts();

		if (lineNumber < 0 || lineNumber >= RawLineCount)
			return Result<int>.Fail(new(new(ErrorCategory.Internal, 2), SourceSpan.Empty, "The 0-based line number must be non-negative and less than the amount of lines in the buffer.", Severity.None));

		if (lineNumber == RawLineCount - 1)
			return Result<int>.Success(0); // EOF means the line is empty

		int start = lineStarts[lineNumber];
		int end = lineStarts[lineNumber + 1];

#if NET8_0_OR_GREATER
		if (precededByCrLf.Contains(end))
			end--; // skip the extra line separator in the CRLF sequence
#else
		if (precededByCrLf.IndexOf(end) != -1)
			end--; // skip the extra line separator in the CRLF sequence
#endif

#if NET8_0_OR_GREATER
		if (!(lineNumber + 2 == RawLineCount && !data[^1].IsNewline()))
#else
		if (!(lineNumber + 2 == RawLineCount && !data[Length - 1].IsNewline()))
#endif
		{
			// if we're not on the last line and it doesn't end with newline then
			end--; // skip one more line separator
		}

		return Result<int>.Success(end - start);
	}

	/// <remarks>
	/// > [!NOTE]
	/// > This method follows EOF conventions.
	/// > EOF is considered a 0-character sequence in line N, where N is <see cref="LineCount"/>.
	/// > Keep in mind N does not point to an actual line (it's just a convention), as line numbers are 0-based
	/// > (meaning the actual last line is located at N - 1).
	/// </remarks>
	/// <inheritdoc/>
	public Result<int> GetLengthOfLineFromIndex(int index)
	{
		var lineNumber = GetLineNumberFromIndex(index);

		if (lineNumber.IsError)
			return lineNumber; // return the error result

		return GetLengthOfLine(lineNumber.Value);
	}

	/// <remarks>
	/// > [!NOTE]
	/// > This method follows EOF conventions.
	/// </remarks>
	/// <inheritdoc/>
	public Result<int> GetLineNumberFromIndex(int index)
	{
		if (IsEmpty)
			return Result<int>.Fail(new(new(ErrorCategory.Internal, 1), SourceSpan.Empty, "The buffer is empty.", Severity.None));

		if (index < 0)
			index += Length;

		if (IsConventionallyOutOfRange(index))
			return Result<int>.Fail(new(new(ErrorCategory.Internal, 2), SourceSpan.Empty, "The index is greater than the buffer's length.", Severity.None));

		ComputeLineStarts();

		GetPreviousOrCurrentLineStartPosition(index, out int lineSepIndex);
		return Result<int>.Success(lineSepIndex);
	}

	/// <remarks>
	/// > [!NOTE]
	/// > This method follows EOF conventions.
	/// > EOF is considered a 0-character sequence in line N, where N is <see cref="LineCount"/>.
	/// > Keep in mind N does not point to an actual line (it's just a convention), as line numbers are 0-based
	/// > (meaning the actual last line is located at N - 1).
	/// </remarks>
	/// <inheritdoc/>
	public Result<string> GetLine(int lineNumber)
	{
		if (IsEmpty)
			return Result<string>.Fail(new(new(ErrorCategory.Internal, 1), SourceSpan.Empty, "The buffer is empty.", Severity.None));

		ComputeLineStarts();

		if (lineNumber < 0 || lineNumber >= RawLineCount)
			return Result<string>.Fail(new(new(ErrorCategory.Internal, 2), SourceSpan.Empty, "The 0-based line number must be non-negative and less than the amount of lines in the buffer.", Severity.None));

		if (lineNumber + 1 == RawLineCount)
			return Result<string>.Success(""); // EOF means the line is empty

		int start = lineStarts[lineNumber];
		int length = GetLengthOfLine(lineNumber).Value; // this will never be an error because we have done exact same checks right above

		return Result<string>.Success(data.Slice(start, length).ToString());
	}

	/// <inheritdoc/>
	public Result<string> GetLineFromIndex(int index)
	{
		var lineNumber = GetLineNumberFromIndex(index);

		if (lineNumber.IsError)
			return Result<string>.Fail(lineNumber.Error); // reuse the error big brain moment

		return GetLine(lineNumber.Value);
	}

	/// <remarks>
	/// > [!IMPORTANT]
	/// > Unlike with other <see cref="ReadOnlySpanBuffer"/> methods, this one
	/// > does not follow EOF conventions and, because of that, does not accept the 
	/// > EOF index (index at <see cref="Length"/>), because it is not
	/// > considered a location.
	/// </remarks>
	/// <inheritdoc/>
	public Result<SourceLocation> GetSourceLocation(int index)
	{
		if (IsEmpty)
			return Result<SourceLocation>.Fail(new(new(ErrorCategory.Internal, 1), SourceSpan.Empty, "The buffer is empty.", Severity.None));

		if (index < 0)
			index += Length;

		if (IsOutOfRange(index))
			return Result<SourceLocation>.Fail(new(new(ErrorCategory.Internal, 2), SourceSpan.Empty, "The index must be less than the buffer's length.", Severity.None));

		if (index == 0) // best "best" case = triple zero
			return Result<SourceLocation>.Success(new(0, 0, 0));

		ComputeLineStarts();
		int lineNumber = GetLineNumberFromIndex(index).Value; // we have done the same checks this emthod does so it'll never be an error

		return Result<SourceLocation>.Success(new(index, lineNumber, index - lineStarts[lineNumber]));
	}

	/// <inheritdoc/>
	public Result<SourceSpan> GetSourceSpan(int startIndex, int endIndex)
	{
		var start = GetSourceLocation(startIndex);
		var end = GetSourceLocation(endIndex);

		if (start.IsSuccessful && end.IsSuccessful)
			return Result<SourceSpan>.Success(new(start.Value, end.Value));

		return Result<SourceSpan>.Fail(start.IsError ? start.Error : end.Error);
	}

	/// <remarks>
	/// > [!IMPORTANT]
	/// > Unlike with other <see cref="ReadOnlySpanBuffer"/> methods, this one
	/// > does not follow EOF conventions and, because of that, does not accept the 
	/// > EOF index (index at <see cref="Length"/>), because it is not
	/// > considered part of any slice.
	/// </remarks>
	/// <inheritdoc/>
	public readonly Result<string> Slice(int start, int length)
	{
		if (length < 0)
			return Result<string>.Fail(new(new(ErrorCategory.Internal, 2), SourceSpan.Empty, "Both the index and the slice length must be positive.", Severity.None));

		if (start < 0)
			start += Length;

		if (start + length > Length)
			return Result<string>.Fail(new(new(ErrorCategory.Internal, 2), SourceSpan.Empty, "The slice's end index is out of range.", Severity.None));

		return Result<string>.Success(data.Slice(start, length).ToString());
	}

	/// <remarks>
	/// > [!IMPORTANT]
	/// > Unlike with other <see cref="ReadOnlySpanBuffer"/> methods, this one
	/// > does not follow EOF conventions and, because of that, does not accept the 
	/// > EOF index (index at <see cref="Length"/>), because it is not
	/// > considered part of any slice.
	/// </remarks>
	/// <inheritdoc/>
	public readonly bool TrySlice(int start, Span<char> slice) => data.Slice(start < 0 ? start + Length : start, slice.Length).TryCopyTo(slice);

	/// <remarks>
	/// > [!IMPORTANT]
	/// > Unlike with other <see cref="ReadOnlySpanBuffer"/> methods, this one
	/// > does not follow EOF conventions and, because of that, does not accept the 
	/// > EOF index (index at <see cref="Length"/>), because it is not
	/// > considered part of any slice.
	/// </remarks>
	/// <inheritdoc/>
	public readonly bool TrySlice(SourceSpan sourceSpan, Span<char> slice) => data.Slice(sourceSpan.Start.Index, sourceSpan.Length).TryCopyTo(slice);

	/// <remarks>
	/// > [!NOTE]
	/// > This method follows the EOF convention where the EOF character
	/// > is 0 (<c>'\0'</c>) and the return value is <c>false</c>, due to EOF
	/// > not being an actual buffer location.
	/// </remarks>
	/// <inheritdoc/>
	public readonly bool TryGetChar(int index, out char item)
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

	/// <remarks>
	/// > [!NOTE]
	/// > This method follows the EOF convention where the EOF character
	/// > is 0 (<c>'\0'</c>) and the return value is <c>false</c>, due to EOF
	/// > not being an actual buffer location.
	/// </remarks>
	/// <inheritdoc/>
	public readonly bool TryGetChar(SourceLocation location, out char item) => TryGetChar(location.Index, out item);

	/// <remarks>
	/// > [!NOTE]
	/// > This method follows EOF conventions.
	/// > EOF is considered a 0-character sequence in line N, where N is <see cref="LineCount"/>.
	/// > Keep in mind N does not point to an actual line (it's just a convention), as line numbers are 0-based
	/// > (meaning the actual last line is located at N - 1).
	/// </remarks>
	/// <inheritdoc/>
	public bool TryGetLine(int lineNumber, scoped Span<char> destination)
	{
		ComputeLineStarts();

		if (IsEmpty || lineNumber < 0 || lineNumber >= RawLineCount)
			return false;

		if (lineNumber + 1 == RawLineCount)
			return true; // EOF means the line is empty (and destination is by default empty)

		int start = lineStarts[lineNumber];
		int length = GetLengthOfLine(lineNumber).Value; // guaranteed to not error out cuz the checks above protect from it

		return data.Slice(start, length).TryCopyTo(destination);
	}

	/// <remarks>
	/// > [!NOTE]
	/// > This method follows EOF conventions.
	/// > EOF is considered a 0-character sequence in line N, where N is <see cref="LineCount"/>.
	/// > Keep in mind N does not point to an actual line (it's just a convention), as line numbers are 0-based
	/// > (meaning the actual last line is located at N - 1). If <paramref name="index" /> is EOF, the
	/// > line will also be EOF.
	/// </remarks>
	/// <inheritdoc/>
	public bool TryGetLineFromIndex(int index, scoped Span<char> destination)
	{
		var lineNumber = GetLineNumberFromIndex(index);
		return !lineNumber.IsError && TryGetLine(lineNumber.Value, destination);
	}

	/// <inheritdoc/>
	public readonly void Dispose() { } // literally do nothing

	/// <inheritdoc/>
	public override readonly bool Equals(
#if NET8_0_OR_GREATER
		[NotNullWhen(true)]
		object? obj
#else
		object obj
#endif
	) => obj switch
	{
		string str => Equals(str),
		char[] charArray => Equals(charArray),
		IBuffer buffer => Equals(buffer),
		ReadOnlyMemory<char> readOnlyMemory => Equals(readOnlyMemory),
		null => false,
		_ => false
	};

	/// <summary>
	/// Checks if an array of characters is equal to the current instance.
	/// </summary>
	/// <param name="other">The array.</param>
	/// <returns>True if equals.</returns>
	public readonly bool Equals(char[]? other) => other is not null && Length == other.Length && data.SequenceEqual(other.AsSpan());

	/// <summary>
	/// Checks if another read-only buffer is equal to the current instance.
	/// </summary>
	/// <param name="other">The other read-only buffer.</param>
	/// <returns>True if equals.</returns>
	public readonly bool Equals(IBuffer? other) => other is not null && Length == other.Length && data.SequenceEqual(other.ToString());

	/// <summary>
	/// Checks if a read-only contiguous region of memory is equal to the current instance.
	/// </summary>
	/// <param name="other">The region of memory.</param>
	/// <returns>True if equals.</returns>
	public readonly bool Equals(ReadOnlyMemory<char> other) => Length == other.Length && data.SequenceEqual(other.Span);

	/// <summary>
	/// Checks if a read-only contiguous region of memory is equal to the current instance.
	/// </summary>
	/// <param name="other">The region of memory.</param>
	/// <returns>True if equals.</returns>
	public readonly bool Equals(ReadOnlySpan<char> other) => Length == other.Length && data.SequenceEqual(other);

	/// <summary>
	/// Checks if another read-only span buffer is equal to the current instance.
	/// </summary>
	/// <param name="other">The other read-only span buffer.</param>
	/// <returns>True if equals.</returns>
	public readonly bool Equals(ReadOnlySpanBuffer other) => Length == other.Length && data.SequenceEqual(other.data);

	/// <summary>
	/// Checks if a string is equal to the current instance.
	/// </summary>
	/// <param name="other">The string.</param>
	/// <returns>True if equals.</returns>
	public readonly bool Equals(string? other) => other is not null && Length == other.Length && data.SequenceEqual(other);

	/// <summary>
	/// Returns the buffer's content as a <see cref="String"/>.
	/// </summary>
	/// <returns>The buffer's content.</returns>
	public override readonly string ToString() => data.ToString();

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = Constants.HashCodeSeed * Constants.HashCodeMultiplier + Length.GetHashCode();
			hashCode = hashCode * Constants.HashCodeMultiplier + IsEmpty.GetHashCode();
			return hashCode * Constants.HashCodeMultiplier + LineCount.GetHashCode();
		}
	}

	/// <summary>
	/// Checks if two read-only span buffers are equals.
	/// </summary>
	/// <returns>True if equals.</returns>
	public static bool operator ==(ReadOnlySpanBuffer left, ReadOnlySpanBuffer right) => left.Equals(right);

	/// <summary>
	/// Checks if two read-only span buffers are different.
	/// </summary>
	/// <returns>True if different.</returns>
	public static bool operator !=(ReadOnlySpanBuffer left, ReadOnlySpanBuffer right) => !(left == right);

	private void ComputeLineStarts(bool forceCache = false)
	{
		if (CacheExists && !forceCache)
			return;

		List<int> privateLineStarts = [];
		List<int> privatePrecededByCrLf = [];

		privateLineStarts.Capacity = Math.Max(privateLineStarts.Capacity, data.Length / 25 + 32); // just a rough guess
		privatePrecededByCrLf.Capacity = Math.Max(privatePrecededByCrLf.Capacity, data.Length / 25 + 16); // just a rough guess

		/* the following line ensures that if the last line:
		 * ends with CR, LF, U2028 or U2029
		 * ends with CRLF
		 * does not end with any of the above
		 * 
		 * it will be counted as a line no matter the outcome of the previous condition
		 * this makes it more obvious from a human side like "bruv my string is abc\ndef two lines right"
		 * it looks normal that there are 2 lines but someone will go "acshua'y, that's erm 1 line :skull:"
		 * not with RSML's official buffers nah bro
		*/
		int lastIndex = data.EndsWith("\r\n") ? data.Length - 2 : data.Length - 1;
		int i = 0;

		privateLineStarts.Add(0); // 0 is by convention the start of line (and also the start of the buffer)

		while (i < lastIndex)
		{
			if (!data[i].IsNewline())
			{
				i++;
				continue;
			}

			bool isCrLf = i < lastIndex && data[i] == '\r' && data[i + 1] == '\n';
			int nextStart = i + (isCrLf ? 2 : 1);
			privateLineStarts.Add(nextStart);

			if (isCrLf)
				privatePrecededByCrLf.Add(nextStart);

			i = nextStart;
		}

		privateLineStarts.Add(Length); // add the EOF as the start of a line

		// xxx: find a way to optimize this so we don't have to allocate an array just for this
		lineStarts = new(privateLineStarts.ToArray());
		precededByCrLf = new(privatePrecededByCrLf.ToArray());

		CacheExists = true;
	}

	/// <param name="insertionPoint">The insertion point of the next line start.</param>
	/// <param name="lsListIndex">The index, in <see cref="lineStarts"/> where the line start is.</param>
	/// <returns>The line start index, in <see cref="data"/>.</returns>
	private readonly int GetNextLineStartFromInsertionPoint(int insertionPoint, out int lsListIndex)
	{
		if (insertionPoint >= 0)
		{
			// 1st Case: the used index (might have been index + 1 based on the method that called this)
			// points to an actual line start
			lsListIndex = insertionPoint;
			return lineStarts[insertionPoint];
		}

		int nextIndex = ~insertionPoint;

		if (nextIndex == lineStarts.Length)
		{
			// 2nd Case: the next line start is outside of the buffer (EOF convention)
			lsListIndex = nextIndex;
			return Length;
		}

		lsListIndex = nextIndex;

		return lineStarts[nextIndex]; // 3rd Case: we found the next line start
	}

	private readonly int GetNextLineStartPosition(int index, out int lsListIndex) =>
		// we use index + 1 to skip to the next line start if we're already standing on one :)
		GetNextLineStartFromInsertionPoint(lineStarts.BinarySearch(index + 1), out lsListIndex);

	/// <param name="insertionPoint">The insertion point of the next line start.</param>
	/// <param name="lsListIndex">The index, in <see cref="lineStarts"/> where the line start is.</param>
	/// <returns>The line start index, in <see cref="data"/>.</returns>
	private readonly int GetPreviousLineStartFromInsertionPoint(int insertionPoint, out int lsListIndex)
	{
		if (insertionPoint >= 0)
		{
			// 1st Case: the used index (might have been index + 1 based on the method that called this)
			// points to an actual line start
			lsListIndex = insertionPoint;
			return lineStarts[insertionPoint];
		}

		int previousIndex = ~insertionPoint;

		if (previousIndex == lineStarts.Length)
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

	private readonly int GetPreviousOrCurrentLineStartPosition(int index, out int lsListIndex)
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
	private readonly bool IsConventionallyOutOfRange(int index) => index < 0 || index > Length;

	private bool IsLastLine(int index)
	{
		if (RawLineCount == 1)
			return true; // only raw line is last line

#if NET8_0_OR_GREATER
		if (index >= lineStarts[^2] && index < lineStarts[^1])
#else
		if (index >= lineStarts[RawLineCount - 2] && index < lineStarts[RawLineCount - 1])
#endif
			return true;

		return false;
	}

	/// <summary>
	/// True if index is greater than or equal to the length.
	/// This prevents throwing <see cref="ArgumentOutOfRangeException"/>
	/// or <see cref="IndexOutOfRangeException"/>.
	/// </summary>
	private readonly bool IsOutOfRange(int index) => index < 0 || index >= Length;
}
