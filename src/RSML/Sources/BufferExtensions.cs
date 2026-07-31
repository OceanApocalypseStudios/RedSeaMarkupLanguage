using System;
using System.Collections.Generic;

using OceanApocalypseStudios.RSML.Diagnostics;
using OceanApocalypseStudios.RSML.Diagnostics.ErrorCodes;

namespace OceanApocalypseStudios.RSML.Sources;

internal static class BufferExtensions
{
	private const string EmptyBufferMessage = "The buffer is empty.";
	private const string IndexGreaterThanLengthMessage = "The index is greater than the buffer's length.";

	/// <summary>
	/// True if index is greater than or equal to the length.
	/// This prevents throwing <see cref="ArgumentOutOfRangeException"/>
	/// or <see cref="IndexOutOfRangeException"/>.
	/// </summary>
	private static bool IsOutOfRange(int index, int length) => index < 0 || index >= length;

	/// <param name="lineStarts"></param>
	/// <param name="insertionPoint">The insertion point of the next line start.</param>
	/// <param name="lsListIndex">The index, in <paramref name="lineStarts"/> where the line start is.</param>
	/// <returns>The line start index, in the span.</returns>
	private static void GetPreviousLineStartFromInsertionPoint(ReadOnlySpan<int> lineStarts, int insertionPoint, out int lsListIndex)
	{
		if (insertionPoint >= 0)
		{
			// 1st Case: the used index (might have been index + 1 based on the method that called this)
			// points to an actual line start
			lsListIndex = insertionPoint;
			return; // this would return lineStarts indexed at insertionPoint;
		}

		int previousIndex = ~insertionPoint;

		if (previousIndex == lineStarts.Length)
		{
			// 2nd Case: the next line start is outside of the buffer (EOF convention)
			// however we're gonna decrement one because otherwise all indexes from the last line that are after
			// start of said line suddenly become part of the EOF line
			// however this might also mean we're at last character
			lsListIndex = previousIndex;
			return; // this would return length;
		}

		lsListIndex = previousIndex == 0 ? 0 : previousIndex - 1;
		// this would return lineStarts[lsListIndex]; // 3rd Case: we found the previous line start
	}

	private static void GetPreviousOrCurrentLineStartPosition(ReadOnlySpan<int> lineStarts, int index, out int lsListIndex)
	{
		if (index > 0)
		{
			// when the index is greater than 0, we can safely get the previous line start without getting a big shitty error
			// we use 0 as the start following standard conventions
			GetPreviousLineStartFromInsertionPoint(lineStarts, lineStarts.BinarySearch(index), out lsListIndex);
		}
		else
		{
			lsListIndex = 0;
		}
	}

	/// <summary>
	/// True if index is greater than the length.
	/// This allows for the EOF convention, where the EOF is a possible output for
	/// some methods.
	/// Does not protect against <see cref="ArgumentOutOfRangeException"/> and
	/// <see cref="IndexOutOfRangeException"/>.
	/// </summary>
	private static bool IsConventionallyOutOfRange(int index, int length) => index < 0 || index > length;

	private static bool IsLastLine(int index, ReadOnlySpan<int> lineStarts) => lineStarts.Length == 1 || index >= lineStarts[^2] && index < lineStarts[^1];

	private static int GetNextLineStartPosition(ReadOnlySpan<int> lineStarts, int index, out int lsListIndex, int spanLength) =>
		// we use index + 1 to skip to the next line start if we're already standing on one :)
		GetNextLineStartFromInsertionPoint(lineStarts, lineStarts.BinarySearch(index + 1), out lsListIndex, spanLength);

	private static int GetNextLineStartFromInsertionPoint(ReadOnlySpan<int> lineStarts, int insertionPoint, out int lsListIndex, int length)
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
			return length;
		}

		lsListIndex = nextIndex;

		return lineStarts[nextIndex]; // 3rd Case: we found the next line start
	}

	extension(ReadOnlySpan<char> span)
	{
		public Result<int> CountUntilEndOfLine(int index, out bool isCrLf, ReadOnlySpan<int> lineStarts, ReadOnlySpan<int> precededByCrLf)
		{
			isCrLf = false;

			if (span.Length == 0)
				return Result.Failure<int>(new(InternalErrorCodes.EmptyBuffer, EmptyBufferMessage));

			if (index < 0)
				index += span.Length;

			if (IsConventionallyOutOfRange(index, span.Length))
				return Result.Failure<int>(new(InternalErrorCodes.IndexOutOfRange, IndexGreaterThanLengthMessage));

			if (index == span.Length)
				return Result.Success(0); // consumed the entire buffer

			int lineSep = GetNextLineStartPosition(lineStarts, index, out _, span.Length);
			isCrLf = precededByCrLf.Contains(lineSep) && span[index] is not '\n'; // to us, CRLF is only when we're not standing on the LF

			if (isCrLf)
				lineSep--; // skip the extra line separator in the CRLF sequence

			if (!(IsLastLine(index, lineStarts) && !span[^1].IsNewline())) // if we're not on the last line and it doesn't end with a newline then
			{
				lineSep--;
			}

			return Result.Success(lineSep - index);
		}

		public Result<int> CountUntilNotWhitespace(int index)
		{
			if (span.Length == 0)
				return Result.Failure<int>(new(InternalErrorCodes.EmptyBuffer, EmptyBufferMessage));

			if (index < 0)
				index = span.Length + index; // -1 is (Length-1) etc

			if (IsConventionallyOutOfRange(index, span.Length))
				return Result.Failure<int>(new(InternalErrorCodes.IndexOutOfRange, IndexGreaterThanLengthMessage));

			if (index == span.Length)
				return Result.Success(0); // consumed the entire buffer

			var slice = span[index..];
			int count = 0;

			while (count < slice.Length && Char.IsWhiteSpace(slice[count]))
				count++;

			return Result.Success(count);
		}

		public Result<int> CountUntilWhitespace(int index)
		{
			if (span.Length == 0)
				return Result.Failure<int>(new(InternalErrorCodes.EmptyBuffer, EmptyBufferMessage));

			if (index < 0)
				index = span.Length + index; // -1 is (Length-1) etc

			if (IsConventionallyOutOfRange(index, span.Length))
				return Result.Failure<int>(new(InternalErrorCodes.IndexOutOfRange, IndexGreaterThanLengthMessage));

			if (index == span.Length)
				return Result.Success(0); // consumed the entire buffer

			var slice = span[index..];
			int count = 0;

			while (count < slice.Length && !Char.IsWhiteSpace(slice[count]))
				count++;

			return Result.Success(count);
		}

		public Result<int> CountWhile(Func<int, char, bool> predicate, int index)
		{
			if (span.Length == 0)
				return Result.Failure<int>(new(InternalErrorCodes.EmptyBuffer, EmptyBufferMessage));

			if (index < 0)
				index = span.Length + index; // -1 is (Length-1) etc

			if (IsConventionallyOutOfRange(index, span.Length))
				return Result.Failure<int>(new(InternalErrorCodes.IndexOutOfRange, IndexGreaterThanLengthMessage));

			if (index == span.Length)
				return Result.Success(0); // consumed the entire buffer

			var slice = span[index..];
			int count = 0;

			while (count < slice.Length && predicate(count, slice[count]))
				count++;

			return Result.Success(count);
		}

		public Result<int> GetLengthOfLine(int lineNumber, ReadOnlySpan<int> lineStarts, ReadOnlySpan<int> precededByCrLf)
		{
			if (span.Length == 0)
				return Result.Failure<int>(new(InternalErrorCodes.EmptyBuffer, EmptyBufferMessage));

			if (lineNumber < 0 || lineNumber >= lineStarts.Length)
				return Result.Failure<int>(new(InternalErrorCodes.LineNumberOutOfRange, "The line number must be non-negative and less than the amount of lines in the buffer."));

			if (lineNumber == lineStarts.Length - 1)
				return Result.Success(0); // EOF means the line is empty

			int start = lineStarts[lineNumber];
			int end = lineStarts[lineNumber + 1];

			if (precededByCrLf.Contains(end))
				end--; // skip the extra line separator in the CRLF sequence

			if (!(lineNumber + 2 == lineStarts.Length && !span[^1].IsNewline()))
			{
				// if we're not on the last line and it doesn't end with newline then
				end--; // skip one more line separator
			}

			return Result.Success(end - start);
		}

		public Result<int> GetLineNumberFromIndex(int index, ReadOnlySpan<int> lineStarts)
		{
			if (span.Length == 0)
				return Result.Failure<int>(new(InternalErrorCodes.EmptyBuffer, EmptyBufferMessage));

			if (index < 0)
				index += span.Length;

			if (IsConventionallyOutOfRange(index, span.Length))
				return Result.Failure<int>(new(InternalErrorCodes.IndexOutOfRange, IndexGreaterThanLengthMessage));

			GetPreviousOrCurrentLineStartPosition(lineStarts, index, out int lineSepIndex);
			return Result.Success(lineSepIndex);
		}

		public Result<string> GetLine(int lineNumber, ReadOnlySpan<int> lineStarts, ReadOnlySpan<int> precededByCrLf)
		{
			if (span.Length == 0)
				return Result.Failure<string>(new(InternalErrorCodes.EmptyBuffer, EmptyBufferMessage));

			if (lineNumber < 0 || lineNumber >= lineStarts.Length)
				return Result.Failure<string>(new(InternalErrorCodes.LineNumberOutOfRange, "The line number must be non-negative and less than the amount of lines in the buffer."));

			if (lineNumber + 1 == lineStarts.Length)
				return Result.Success(""); // EOF means the line is empty

			int start = lineStarts[lineNumber];
			int length = span.GetLengthOfLine(lineNumber, lineStarts, precededByCrLf).Value; // this will never be an error because we have done exact same checks right above

			return Result.Success(span.Slice(start, length).ToString());
		}

		public ReadOnlySpan<char> GetLineAsSpan(int lineNumber, ReadOnlySpan<int> lineStarts, ReadOnlySpan<int> precededByCrLf)
		{
			if (span.Length == 0)
				return [];

			if (lineNumber < 0 || lineNumber >= lineStarts.Length)
				return [];

			if (lineNumber + 1 == lineStarts.Length)
				return []; // EOF means the line is empty

			int start = lineStarts[lineNumber];
			int length = span.GetLengthOfLine(lineNumber, lineStarts, precededByCrLf).Value; // this will never be an error because we have done exact same checks right above

			return span.Slice(start, length);
		}

		public Result<SourceLocation> GetSourceLocation(int index, ReadOnlySpan<int> lineStarts)
		{
			if (span.Length == 0)
				return Result.Failure<SourceLocation>(new(InternalErrorCodes.EmptyBuffer, EmptyBufferMessage));

			if (index < 0)
				index += span.Length;

			if (IsOutOfRange(index, span.Length))
				return Result.Failure<SourceLocation>(new(InternalErrorCodes.IndexOutOfRange, "The index is greater than the buffer's length or points to EOF."));

			if (index == 0) // best "best" case = triple zero
				return Result.Success<SourceLocation>(new(0, 0, 0));

			int lineNumber = span.GetLineNumberFromIndex(index, lineStarts).Value; // we have done the same checks this emthod does so it'll never be an error

			return Result.Success<SourceLocation>(new(index, lineNumber, index - lineStarts[lineNumber]));
		}

		public Result<SourceSpan> GetSourceSpan(int startIndex, int endIndex, ReadOnlySpan<int> lineStarts)
		{
			var start = span.GetSourceLocation(startIndex, lineStarts);
			var end = span.GetSourceLocation(endIndex, lineStarts);

			return start.IsSuccessful && end.IsSuccessful
				? Result.Success<SourceSpan>(new(start.Value, end.Value))
				: Result.Failure<SourceSpan>(
					start.IsError switch
					{
						true => start.Error,
						false => end.Error
					});
		}

		public Result<string> SliceAsResult(int start, int length)
		{
			if (length < 0)
				return Result.Failure<string>(new(InternalErrorCodes.IndexOutOfRange, "The slice length must be positive."));

			if (start < 0)
				start += span.Length;

			return IsConventionallyOutOfRange(start + length, span.Length)
				? Result.Failure<string>(new(InternalErrorCodes.IndexOutOfRange, "The slice's end index is greater than the buffer's length or points to EOF."))
				: Result.Success(span.Slice(start, length).ToString());
		}

		public bool TryGetChar(int index, out char item)
		{
			item = '\0'; // default

			if (span.Length == 0)
				return false;

			if (index < 0)
				index = span.Length + index; // -1 is (Length-1) etc

			if (IsOutOfRange(index, span.Length))
				return false;

			item = span[index];

			return true;
		}

		public bool TryGetLine(int lineNumber, scoped Span<char> destination, ReadOnlySpan<int> lineStarts, ReadOnlySpan<int> precededByCrLf)
		{
			if (span.Length == 0 || lineNumber < 0 || lineNumber >= lineStarts.Length)
				return false;

			if (lineNumber + 1 == lineStarts.Length)
				return true; // EOF means the line is empty (and destination is by default empty)

			int start = lineStarts[lineNumber];
			int length = span.GetLengthOfLine(lineNumber, lineStarts, precededByCrLf).Value; // guaranteed to not error out cuz the checks above protect from it

			return span.Slice(start, length).TryCopyTo(destination);
		}

		public List<int>[] ComputeLineStarts()
		{
			List<int> privateLineStarts = [];
			List<int> privatePrecededByCrLf = [];

			privateLineStarts.Capacity = Math.Max(privateLineStarts.Capacity, span.Length / 25 + 32); // just a rough guess
			privatePrecededByCrLf.Capacity = Math.Max(privatePrecededByCrLf.Capacity, span.Length / 25 + 16); // just a rough guess

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
			int lastIndex = span.EndsWith("\r\n") ? span.Length - 2 : span.Length - 1;
			int i = 0;

			privateLineStarts.Add(0); // 0 is by convention the start of line (and also the start of the buffer)

			while (i < lastIndex)
			{
				if (!span[i].IsNewline())
				{
					i++;
					continue;
				}

				bool isCrLf = i < lastIndex && span[i] == '\r' && span[i + 1] == '\n';
				int nextStart = i + (isCrLf ? 2 : 1);
				privateLineStarts.Add(nextStart);

				if (isCrLf)
					privatePrecededByCrLf.Add(nextStart);

				i = nextStart;
			}

			privateLineStarts.Add(span.Length); // add the EOF as the start of a line

			// xxx: avoid allocating this array
			return [privateLineStarts, privatePrecededByCrLf];
		}
	}
}
