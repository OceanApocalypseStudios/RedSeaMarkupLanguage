using System;


namespace OceanApocalypseStudios.RSML.Sources;

/// <summary>
/// Represents a buffer.
/// </summary>
/// <typeparam name="TItem">The datatype of the values returned by the buffer methods</typeparam>
public interface IBuffer<TItem> : ISource, IEquatable<IBuffer<TItem>?>
{
	// todo: add methods that can mutate the buffer (coming to v3.0.0-prerelease2)

	/// <summary>
	/// The total amount of lines in the buffer.
	/// </summary>
	/// <remarks>
	/// Keep in mind lines might be empty.
	/// </remarks>
	int LineCount { get; }

	/// <summary>
	/// Gets a single item out of the buffer.
	/// </summary>
	/// <param name="index">The index of the item to retrieve.</param>
	/// <returns>The item.</returns>
	TItem this[int index] { get; }

	/// <summary>
	/// Gets a single item out of the buffer.
	/// </summary>
	/// <param name="location">The location of the item to retrieve.</param>
	/// <returns>The item.</returns>
	TItem this[SourceLocation location] { get; }

	/// <summary>
	/// Gets a range of items out of the buffer.
	/// </summary>
	/// <param name="span">The span of items to get.</param>
	/// <returns>The items.</returns>
	TItem[] this[SourceSpan span] { get; }

	/// <summary>
	/// Counts the amount of items until the next line separator in the buffer, relative to a given <paramref name="index"/>.
	/// Only line separators count - regular whitespace do not. CRLF counts as a single line separator, to avoid double counting.
	/// </summary>
	/// <param name="index">The index at which to start counting.</param>
	/// <param name="isCrLf">
	/// Whether the line separator at which the method stopped is the CR in a CRLF sequence. If true, the next item in the buffer is LF.
	/// </param>
	/// <returns>The index of the next line separator, relative to an <paramref name="index"/>.</returns>
	int CountUntilLineSeparator(int index, out bool isCrLf);

	/// <summary>
	/// Counts the amount of items until the next non-whitespace item in the buffer, relative to a given <paramref name="index"/>.
	/// Line separators are included in the whitespace category.
	/// </summary>
	/// <param name="index">The index at which to start counting.</param>
	/// <returns>The index of the next non-whitespace item, relative to a <paramref name="index"/>.</returns>
	int CountUntilNotWhitespace(int index);

	/// <summary>
	/// Counts the amount of items until the next whitespace item in the buffer, relative to a given <paramref name="index"/>.
	/// Line separators are included in the whitespace category.
	/// </summary>
	/// <param name="index">The index at which to start counting.</param>
	/// <returns>The index of the next whitespace item, relative to a <paramref name="index"/>.</returns>
	int CountUntilWhitespace(int index);

	/// <summary>
	/// Counts the amount of items, starting from a given <paramref name="index"/>,
	/// while a <paramref name="predicate"/> returns <c>true</c>.
	/// </summary>
	/// <param name="predicate">
	/// A function that takes the current index (relative to <paramref name="index"/>),
	/// which is incremented every item, and the item associated with it. Execution stops when
	/// the predicate returns <c>false</c> or the index is out of bounds.
	/// </param>
	/// <param name="index">
	/// The index at which to start counting; all indexes will also be given to the
	/// <paramref name="predicate"/> as an offset that when added to the index of the position
	/// equal the actual index.
	/// </param>
	/// <returns>The amount of items counted.</returns>
	int CountWhile(Func<int, TItem, bool> predicate, int index);

	/// <summary>
	/// Returns the length of a line given its 0-based line number.
	/// Line separators do not count towards the length.
	/// </summary>
	/// <param name="lineNumber">The 0-based line number.</param>
	/// <returns>The length of the line.</returns>
	int GetLengthOfLine(int lineNumber);

	/// <summary>
	/// Returns the length of a line given a 0-based index of one
	/// of its items.
	/// Line separators do not count towards the length.
	/// </summary>
	/// <param name="index">The 0-based index whose line is considered.</param>
	/// <returns>The length of the line.</returns>
	int GetLengthOfLineFromIndex(int index);

	/// <summary>
	/// Given a 0-based line number, returns the matching line as an array of buffer items.
	/// </summary>
	/// <param name="lineNumber">The 0-based line number.</param>
	/// <returns>The line as an array of items.</returns>
	TItem[] GetLine(int lineNumber);

	/// <summary>
	/// Tries to read the line that contains the item at <paramref name="index"/>.
	/// No end of line characters are added.
	/// </summary>
	/// <param name="index">The index at which to determine what the current line is.</param>
	/// <returns>The line, as an array of items.</returns>
	TItem[] GetLineFromIndex(int index);

	/// <summary>
	/// Determines the 0-based line number of the line that contains the item located at <paramref name="index"/>.
	/// </summary>
	/// <param name="index">The index whose parent line's number is to be returned.</param>
	/// <returns>The 0-based number of the line that contains item located at <paramref name="index"/>.</returns>
	int GetLineNumberFromIndex(int index);

	/// <summary>
	/// Converts an index into a location.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The location.</returns>
	SourceLocation GetSourceLocation(int index);

	/// <summary>
	/// Tries to read the word at index <paramref name="index"/>, which can be a span of items verified by
	/// <paramref name="itemKindPredicate"/> (if the starting index is verified by <paramref name="itemKindPredicate"/>)
	/// or a span of content not verified by <paramref name="itemKindPredicate"/> (if
	/// the starting index does not point to anything verified by <paramref name="itemKindPredicate"/>).
	/// </summary>
	/// <param name="index">The index at which the word starts.</param>
	/// <param name="itemKindPredicate">
	/// A predicate that verifies if the first item in the selected range serves as the delimiter
	/// (the predicate returns <c>true</c> if so). For example, if the predicate verifies <c>,</c>, then 
	/// the current word if the buffer is <c>My awesome buffer, isn't it cool?</c> starting from <paramref name="index"/>,
	/// is <c>My awesome buffer</c>. If the predicate verifies <c>;</c>, then the current word
	/// if the buffer is <c>;;so very awesome</c> starting from <paramref name="index"/>, is <c>;;</c>.
	/// </param>
	/// <param name="isItemKind">
	/// True if the array is fully comprised of the item kind verified by <paramref name="itemKindPredicate"/>.
	/// If False, no items verified by <paramref name="itemKindPredicate"/> is present.
	/// </param>
	/// <returns>The line, as an array of items.</returns>
	TItem[] GetWord(int index, Func<int, TItem, bool> itemKindPredicate, out bool isItemKind);

	/// <summary>
	/// Tries to read the word at index <paramref name="index"/>, which can be a span of whitespace (if the starting index points to whitespace)
	/// or a span of non-whitespace content (if the starting index does not point to whitespace).
	/// </summary>
	/// <param name="index">The index at which the word starts.</param>
	/// <param name="isWhitespace">True if the array is fully comprised of whitespace. If False, no whitespace is present.</param>
	/// <returns>The line, as an array of items.</returns>
	TItem[] GetWord(int index, out bool isWhitespace);

	/// <summary>
	/// Tries to read the word at index <paramref name="index"/>, which can be a span of <paramref name="itemKind"/> (if the starting index
	/// points to <paramref name="itemKind"/>) or a span of non-<paramref name="itemKind"/> content (if
	/// the starting index does not point to <paramref name="itemKind"/>).
	/// </summary>
	/// <param name="index">The index at which the word starts.</param>
	/// <param name="itemKind">
	/// The item that will serve as the delimiter. For example, if item kind is <c>,</c>, then 
	/// the current word if the buffer is <c> My awesome buffer, isn't it cool?</c> starting from <paramref name="index"/>,
	/// is <c> My awesome buffer</c>. If item kind is <c>;</c>, then 
	/// the current word if the buffer is <c>;;so very awesome</c> starting from <paramref name="index"/>, is <c>;;</c>.
	/// </param>
	/// <param name="isItemKind">
	/// True if the array is fully comprised of <paramref name="itemKind"/>.
	/// If False, no <paramref name="itemKind"/> is present.
	/// </param>
	/// <returns>The line, as an array of items.</returns>
	TItem[] GetWord(int index, TItem itemKind, out bool isItemKind);

	/// <summary>
	/// Slices a region of the buffer.
	/// </summary>
	/// <param name="start">The index of the first item in the slice.</param>
	/// <param name="length">The amount of items to slice starting at <paramref name="start"/>.</param>
	/// <returns>A slice, as an array of items.</returns>
	TItem[] Slice(int start, int length);

	/// <summary>
	/// Slices a region of the buffer into a performant span.
	/// </summary>
	/// <param name="start">The index of the first item in the slice.</param>
	/// <param name="slice">The span serving as the destination for the slice.</param>
	void Slice(int start, Span<TItem> slice);

	/// <summary>
	/// Slices a region of the buffer into a performant span.
	/// </summary>
	/// <param name="sourceSpan">The span indicating what the slice is.</param>
	/// <param name="slice">The span serving as the destination for the slice.</param>
	void Slice(SourceSpan sourceSpan, Span<TItem> slice);

	/// <summary>
	/// Tries to read the next item in the buffer, relative to an <paramref name="index"/>.
	/// </summary>
	/// <param name="index">The index of the character.</param>
	/// <param name="item">The item.</param>
	/// <returns>False if the buffer is out of bounds or an exception occured.</returns>
	bool TryGetChar(int index, out TItem item);

	/// <summary>
	/// Given a 0-based line number, assigns the exact line to a result buffer (<paramref name="destination"/>).
	/// </summary>
	/// <param name="lineNumber">The 0-based line number.</param>
	/// <param name="destination">The destination buffer for the line.</param>
	/// <param name="itemCount">The amount of items returned.</param>
	/// <returns>True if successful.</returns>
	bool TryGetLine(int lineNumber, Span<TItem> destination, out int itemCount);

	/// <summary>
	/// Tries to read the line that contains the item at <paramref name="index"/>.
	/// No end of line characters are added.
	/// </summary>
	/// <param name="index">The index at which to determine what the current line is.</param>
	/// <param name="destination">The destination span that will contain the line.</param>
	/// <param name="itemCount">The amount of items that were written to <paramref name="destination"/>.</param>
	/// <returns>False if the buffer is out of bounds, there are no more lines to read or an exception occured.</returns>
	bool TryGetLineFromIndex(int index, Span<TItem> destination, out int itemCount);

	/// <summary>
	/// Tries to read the word at index <paramref name="index"/>, which can be a span of items verified by
	/// <paramref name="itemKindPredicate"/> (if the starting index is verified by <paramref name="itemKindPredicate"/>)
	/// or a span of content not verified by <paramref name="itemKindPredicate"/> (if
	/// the starting index does not point to anything verified by <paramref name="itemKindPredicate"/>).
	/// </summary>
	/// <param name="index">The index at which the word starts.</param>
	/// <param name="itemKindPredicate">
	/// A predicate that verifies if the first item in the selected range serves as the delimiter
	/// (the predicate returns <c>true</c> if so). For example, if the predicate verifies <c>,</c>, then 
	/// the current word if the buffer is <c>My awesome buffer, isn't it cool?</c> starting from <paramref name="index"/>,
	/// is <c>My awesome buffer</c>. If the predicate verifies <c>;</c>, then the current word
	/// if the buffer is <c>;;so very awesome</c> starting from <paramref name="index"/>, is <c>;;</c>.
	/// </param>
	/// <param name="destination">The span which will be the destination for the read content.</param>
	/// <param name="isItemKind">
	/// True if the span is fully comprised of the item kind verified by <paramref name="itemKindPredicate"/>.
	/// If False, no items verified by <paramref name="itemKindPredicate"/> is present.
	/// </param>
	/// <param name="itemCount">The amount of characters that were written to <paramref name="destination"/>.</param>
	/// <remarks>
	/// This method may lead to partial reading (for example if <paramref name="destination"/> is not
	/// large enough). When this happens, the return value will be <strong>False</strong> but <paramref name="itemCount"/>
	/// will be set to something greater than <c>0</c>.
	/// </remarks>
	/// <returns>False if the buffer is out of bounds, the reading was only partial or an exception occured.</returns>
	bool TryGetWord(int index, Func<int, TItem, bool> itemKindPredicate, Span<TItem> destination, out bool isItemKind, out int itemCount);

	/// <summary>
	/// Tries to read the word at index <paramref name="index"/>, which can be a span of whitespace (if the starting index points to whitespace)
	/// or a span of non-whitespace content (if the starting index does not point to whitespace).
	/// </summary>
	/// <param name="index">The index at which the word starts.</param>
	/// <param name="destination">The span which will be the destination for the read content.</param>
	/// <param name="isWhitespace">True if the span is fully comprised of whitespace. If False, no whitespace is present.</param>
	/// <param name="itemCount">The amount of characters that were written to <paramref name="destination"/>.</param>
	/// <remarks>
	/// This method may lead to partial reading (for example if <paramref name="destination"/> is not
	/// large enough). When this happens, the return value will be <strong>False</strong> but <paramref name="itemCount"/>
	/// will be set to something greater than <c>0</c>.
	/// </remarks>
	/// <returns>False if the buffer is out of bounds or an exception occured.</returns>
	bool TryGetWord(int index, Span<TItem> destination, out bool isWhitespace, out int itemCount);

	/// <summary>
	/// Tries to read the word at index <paramref name="index"/>, which can be a span of <paramref name="itemKind"/> (if the starting index
	/// points to <paramref name="itemKind"/>) or a span of non-<paramref name="itemKind"/> content (if
	/// the starting index does not point to <paramref name="itemKind"/>).
	/// </summary>
	/// <param name="index">The index at which the word starts.</param>
	/// <param name="itemKind">
	/// The item that will serve as the delimiter. For example, if item kind is <c>,</c>, then 
	/// the current word if the buffer is <c> My awesome buffer, isn't it cool?</c> starting from <paramref name="index"/>,
	/// is <c> My awesome buffer</c>. If item kind is <c>;</c>, then 
	/// the current word if the buffer is <c>;;so very awesome</c> starting from <paramref name="index"/>, is <c>;;</c>.
	/// </param>
	/// <param name="destination">The span which will be the destination for the read content.</param>
	/// <param name="isItemKind">
	/// True if the span is fully comprised of <paramref name="itemKind"/>.
	/// If False, no <paramref name="itemKind"/> is present.
	/// </param>
	/// <param name="itemCount">The amount of items that were written to <paramref name="destination"/>.</param>
	/// <remarks>
	/// This method may lead to partial reading (for example if <paramref name="destination"/> is not
	/// large enough). When this happens, the return value will be <strong>False</strong> but <paramref name="itemCount"/>
	/// will be set to something greater than <c>0</c>.
	/// </remarks>
	/// <returns>False if the buffer is out of bounds, the reading was only partial or an exception occured.</returns>
	bool TryGetWord(int index, TItem itemKind, Span<TItem> destination, out bool isItemKind, out int itemCount);
}
