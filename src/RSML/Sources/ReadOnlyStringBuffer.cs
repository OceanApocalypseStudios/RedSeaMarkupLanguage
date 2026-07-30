using System;
using System.Collections.Generic;
using System.Text;

using OceanApocalypseStudios.RSML.Cache;
using OceanApocalypseStudios.RSML.Diagnostics;


namespace OceanApocalypseStudios.RSML.Sources;

/// <summary>
/// A read-only buffer backed by a string. All operations opt for performance
/// primarily via the internal use of <see cref="ReadOnlySpan{Char}"/> over string allocations
/// and also via caching.
/// </summary>
/// <remarks>
/// :::tip[High-performance alternatives]
/// If you wish to avoid allocating this buffer, it's recommended to take a look at <see cref="ReadOnlySpanBuffer"/>.
/// :::
/// </remarks>
public class ReadOnlyStringBuffer : IReadOnlyBuffer, ISupportsCache, IEquatable<string?>
{
	private bool isDisposed;

	private int[] lineStarts = [];

	private int[] precededByCrLf = [];

	private readonly string data;

	/// <inheritdoc/>
	public bool CacheExists { get; private set; }

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

			return lineStarts.Length;
		}
	}

	/// <inheritdoc/>
	public char? this[int index] => TryGetChar(index, out char item) ? item : null;

	/// <inheritdoc/>
	public char? this[SourceLocation location] => this[location.Index];

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

	/// <remarks>
	/// :::info[EOF Conventions]
	/// This method allows the EOF index as in-range. The convention is as follows:
	/// - If the index is EOF (<see cref="Length"/>), then the output is always 0 and <paramref name="isCrLf"/> is always <c>false</c>.
	/// - If the index is the last (<see cref="Length"/> - 1), then the output is always 0.
	/// :::
	/// 
	/// :::info[Value of 'isCrLf' parameter]
	/// <paramref name="isCrLf"/> is only <c>true</c> if all the following conditions are true:
	/// - The next line start counting from <paramref name="index"/> is preceded by a CRLF sequence.
	/// - <paramref name="index"/> does not point to the LF in the CRLF sequence.
	/// - <paramref name="index"/> does not point to EOF.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public Result<int> CountUntilEndOfLine(int index, out bool isCrLf)
	{
		ComputeLineStarts();
		return data.AsSpan().CountUntilEndOfLine(index, out isCrLf, lineStarts, precededByCrLf);
	}

	/// <remarks>
	/// :::info[EOF Conventions]
	/// This method allows the EOF index as in-range.
	/// If the index is EOF (<see cref="Length"/>), then the output is always 0.
	/// :::
	/// 
	/// :::tip[About the return value]
	/// The return value, when summed with <paramref name="index"/>, becomes the index of the first character that
	/// is not whitespace, counting from <paramref name="index"/>.
	/// The only exception is if the buffer has been consumed (you pass EOF index or there's no more characters that are
	/// not whitespace), meaning the return value, when summed with <paramref name="index"/> is the value of
	/// <see cref="Length"/>, which is also the EOF index.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public Result<int> CountUntilNotWhitespace(int index) =>
		data.AsSpan().CountUntilNotWhitespace(index);

	/// <remarks>
	/// :::info[EOF Conventions]
	/// This method allows the EOF index as in-range.
	/// If the index is EOF (<see cref="Length"/>), then the output is always 0.
	/// :::
	/// 
	/// :::tip[About the return value]
	/// The return value, when summed with <paramref name="index"/>, becomes the index of the first character that
	/// is whitespace, counting from <paramref name="index"/>.
	/// The only exception is if the buffer has been consumed (you pass EOF index or there's no more characters that are
	/// whitespace), meaning the return value, when summed with <paramref name="index"/> is the value of
	/// <see cref="Length"/>, which is also the EOF index.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public Result<int> CountUntilWhitespace(int index) =>
		data.AsSpan().CountUntilWhitespace(index);

	/// <remarks>
	/// :::info[EOF Conventions]
	/// This method allows the EOF index as in-range.
	/// If the index is EOF (<see cref="Length"/>), then the output is always 0.
	/// :::
	/// 
	/// :::tip[About the return value]
	/// The return value, when summed with <paramref name="index"/>, becomes the index of the first character that
	/// fails to verify the <paramref name="predicate"/>, counting from <paramref name="index"/>.
	/// The only exception is if the buffer has been consumed (you pass EOF index or there's no more characters that fail to verify
	/// the <paramref name="predicate"/>), meaning the return value, when summed with <paramref name="index"/> is the value of
	/// <see cref="Length"/>, which is also the EOF index.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public Result<int> CountWhile(Func<int, char, bool> predicate, int index) =>
		data.AsSpan().CountWhile(predicate, index); // todo: fix CA1062

	/// <remarks>
	/// :::info[EOF Conventions]
	/// This method follows EOF conventions.
	/// EOF is considered a 0-character sequence in line N, where N is <see cref="LineCount"/>.
	/// Keep in mind N does not point to an actual line (it's just a convention), as line numbers are 0-based
	/// (meaning the actual last line is located at N - 1).
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public Result<int> GetLengthOfLine(int lineNumber)
	{
		ComputeLineStarts();
		return data.AsSpan().GetLengthOfLine(lineNumber, lineStarts, precededByCrLf);
	}

	/// <remarks>
	/// :::info[EOF Conventions]
	/// This method follows EOF conventions.
	/// EOF is considered a 0-character sequence in line N, where N is <see cref="LineCount"/>.
	/// Keep in mind N does not point to an actual line (it's just a convention), as line numbers are 0-based
	/// (meaning the actual last line is located at N - 1).
	/// :::
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
	/// :::info[EOF Conventions]
	/// This method follows EOF conventions.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public Result<int> GetLineNumberFromIndex(int index)
	{
		ComputeLineStarts();
		return data.AsSpan().GetLineNumberFromIndex(index, lineStarts);
	}

	/// <remarks>
	/// :::info[EOF Conventions]
	/// This method follows EOF conventions.
	/// EOF is considered a 0-character sequence in line N, where N is <see cref="LineCount"/>.
	/// Keep in mind N does not point to an actual line (it's just a convention), as line numbers are 0-based
	/// (meaning the actual last line is located at N - 1).
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public Result<string> GetLine(int lineNumber)
	{
		ComputeLineStarts();
		return data.AsSpan().GetLine(lineNumber, lineStarts, precededByCrLf);
	}

	/// <inheritdoc/>
	public Result<string> GetLineFromIndex(int index)
	{
		var lineNumber = GetLineNumberFromIndex(index);

		if (lineNumber.IsError)
			return Result.Failure<string>(lineNumber.Error); // reuse the error big brain moment

		return GetLine(lineNumber.Value);
	}

	/// <remarks>
	/// :::info[EOF Conventions]
	/// This method follows EOF conventions.
	/// EOF is considered a 0-character sequence in line N, where N is <see cref="LineCount"/>.
	/// Keep in mind N does not point to an actual line (it's just a convention), as line numbers are 0-based
	/// (meaning the actual last line is located at N - 1).
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public ReadOnlySpan<char> GetLineAsSpan(int lineNumber)
	{
		ComputeLineStarts();
		return data.AsSpan().GetLineAsSpan(lineNumber, lineStarts, precededByCrLf);
	}

	/// <inheritdoc/>
	public ReadOnlySpan<char> GetLineAsSpanFromIndex(int index)
	{
		var lineNumber = GetLineNumberFromIndex(index);

		return lineNumber.IsError ? [] : GetLineAsSpan(lineNumber.Value);
	}

	/// <remarks>
	/// :::warning[EOF Conventions]
	/// Unlike with other <see cref="ReadOnlyStringBuffer"/> methods, this one
	/// does not follow EOF conventions and, because of that, does not accept the 
	/// EOF index (index at <see cref="Length"/>), because it is not
	/// considered a location.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public Result<SourceLocation> GetSourceLocation(int index)
	{
		ComputeLineStarts();
		return data.AsSpan().GetSourceLocation(index, lineStarts);
	}

	/// <inheritdoc/>
	public Result<SourceSpan> GetSourceSpan(int startIndex, int endIndex)
	{
		ComputeLineStarts();
		return data.AsSpan().GetSourceSpan(startIndex, endIndex, lineStarts);
	}

	/// <remarks>
	/// :::warning[EOF Conventions]
	/// Unlike with other <see cref="ReadOnlyStringBuffer"/> methods, this one
	/// does not follow EOF conventions and, because of that, does not accept the 
	/// EOF index (index at <see cref="Length"/>), because it is not
	/// considered part of any slice.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public Result<string> Slice(int start, int length) =>
		data.AsSpan().SliceAsResult(start, length);

	/// <remarks>
	/// :::warning[EOF Conventions]
	/// Unlike with other <see cref="ReadOnlyStringBuffer"/> methods, this one
	/// does not follow EOF conventions and, because of that, does not accept the 
	/// EOF index (index at <see cref="Length"/>), because it is not
	/// considered part of any slice.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public bool TrySlice(int start, Span<char> slice) => data.AsSpan(start < 0 ? start + Length : start, slice.Length).TryCopyTo(slice);

	/// <remarks>
	/// :::warning[EOF Conventions]
	/// Unlike with other <see cref="ReadOnlyStringBuffer"/> methods, this one
	/// does not follow EOF conventions and, because of that, does not accept the 
	/// EOF index (index at <see cref="Length"/>), because it is not
	/// considered part of any slice.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public bool TrySlice(SourceSpan sourceSpan, Span<char> slice) => data.AsSpan(sourceSpan.Start.Index, sourceSpan.Length).TryCopyTo(slice);

	/// <remarks>
	/// :::info[EOF Conventions]
	/// This method follows the EOF convention where the EOF character
	/// is 0 (<c>'\0'</c>) and the return value is <c>false</c>, due to EOF
	/// not being an actual buffer location.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public bool TryGetChar(int index, out char item) =>
		data.AsSpan().TryGetChar(index, out item);

	/// <remarks>
	/// :::info[EOF Conventions]
	/// This method follows the EOF convention where the EOF character
	/// is 0 (<c>'\0'</c>) and the return value is <c>false</c>, due to EOF
	/// not being an actual buffer location.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public bool TryGetChar(SourceLocation location, out char item) =>
		TryGetChar(location.Index, out item);

	/// <remarks>
	/// :::info[EOF Conventions]
	/// This method follows EOF conventions.
	/// EOF is considered a 0-character sequence in line N, where N is <see cref="LineCount"/>.
	/// Keep in mind N does not point to an actual line (it's just a convention), as line numbers are 0-based
	/// (meaning the actual last line is located at N - 1).
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public bool TryGetLine(int lineNumber, Span<char> destination)
	{
		ComputeLineStarts();
		return data.AsSpan().TryGetLine(lineNumber, destination, lineStarts, precededByCrLf);
	}

	/// <remarks>
	/// :::info[EOF Conventions]
	/// This method follows EOF conventions.
	/// EOF is considered a 0-character sequence in line N, where N is <see cref="LineCount"/>.
	/// Keep in mind N does not point to an actual line (it's just a convention), as line numbers are 0-based
	/// (meaning the actual last line is located at N - 1). If <paramref name="index" /> is EOF, the
	/// line will also be EOF.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public bool TryGetLineFromIndex(int index, Span<char> destination)
	{
		var lineNumber = GetLineNumberFromIndex(index);
		return !lineNumber.IsError && TryGetLine(lineNumber.Value, destination);
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <inheritdoc/>
	public override bool Equals(
#if NETCOREAPP3_0_OR_GREATER
		[NotNullWhen(true)]
		object? obj
#else
		object obj
#endif
	) => obj switch
	{
		string str => Equals(str),
		char[] charArray => Equals(charArray),
		IReadOnlyBuffer buffer => Equals(buffer),
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
	public bool Equals(IReadOnlyBuffer? other) => other is not null && data.Equals(other.ToString(), StringComparison.Ordinal);

	/// <summary>
	/// Checks if a read-only contiguous region of memory is equal to the current instance.
	/// </summary>
	/// <param name="other">The region of memory.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(ReadOnlyMemory<char> other) => Length == other.Length && data.SequenceEqual(other.Span);

	/// <summary>
	/// Checks if a read-only contiguous region of memory is equal to the current instance.
	/// </summary>
	/// <param name="other">The region of memory.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(ReadOnlySpan<char> other) => Length == other.Length && data.SequenceEqual(other);

	/// <summary>
	/// Checks if a string is equal to the current instance.
	/// </summary>
	/// <param name="other">The string.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(string? other) => other is not null && Length == other.Length && data.Equals(other, StringComparison.Ordinal);

	/// <inheritdoc/>
	public override int GetHashCode() => unchecked(HashCode.Combine(data, lineStarts, precededByCrLf));

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

	/// <summary>
	/// Disposes of both managed and unmanaged resources.
	/// </summary>
	/// <param name="disposing">When set to <c>false</c>, disposes of unmanaged resources only.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (isDisposed)
			return;

		if (disposing)
			CacheExists = false;

		isDisposed = true;
	}

	private void ComputeLineStarts(bool forceCache = false)
	{
		if (CacheExists && !forceCache)
			return;

		var result = data.AsSpan().ComputeLineStarts();

		// xxx: avoid allocating these arrays
		lineStarts = result[0].ToArray();
		precededByCrLf = result[1].ToArray();

		CacheExists = true;
	}
}
