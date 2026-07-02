using System;


namespace OceanApocalypseStudios.RSML.Buffers
{

	/// <summary>
	/// Represents a buffer.
	/// </summary>
	/// <typeparam name="TItem">The datatype of the values returned by the buffer methods</typeparam>
	public interface IBuffer<TItem> : IDisposable
	{

		/// <summary>
		/// The length of the buffer.
		/// </summary>
		int Length { get; }

		/// <summary>
		/// Whether the buffer is completely empty.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Counts the amount of items until the next whitespace item in the buffer, relative to a given <paramref name="index"/>.
		/// Line separators are included in the whitespace category.
		/// </summary>
		/// <returns>The index of the next whitespace item, relative to a <paramref name="index"/> or -1 if out of bounds.</returns>
		int CountUntilWhitespace(int index);

		/// <summary>
		/// Counts the amount of items until the next newline item in the buffer, relative to a given <paramref name="index"/>.
		/// Only line separators count - regular whitespace do not.
		/// </summary>
		/// <returns>The index of the next line separator, relative to an <paramref name="index"/> or -1 if out of bounds.</returns>
		int CountUntilNewline(int index);

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
		/// <returns>The amount of items counted or -1 if out of bounds.</returns>
		int CountWhile(Func<int, TItem, bool> predicate, int index);

		/// <summary>
		/// Tries to read the next item in the buffer, relative to an <paramref name="index"/>.
		/// </summary>
		/// <param name="index">The index of the character.</param>
		/// <param name="item">The item.</param>
		/// <returns>False if the buffer is out of bounds or an exception occured.</returns>
		bool TryGetChar(int index, out TItem item);

		/// <summary>
		/// Tries to read the next line in the buffer, relative to a given <paramref name="index"/>.
		/// If the <paramref name="index"/> is the start of a line, then that line is considered instead of the next.
		/// No end of line characters are added.
		/// </summary>
		/// <param name="index">The index at which to determine what the next line is.</param>
		/// <param name="line">The destination span that will contain the line.</param>
		/// <param name="charCount">The amount of items that were written to <paramref name="line"/>.</param>
		/// <returns>False if the buffer is out of bounds, there are no more lines to read or an exception occured.</returns>
		bool TryGetLine(int index, Span<TItem> line, out int charCount);

		/// <summary>
		/// Tries to read the word at index <paramref name="index"/>, which can be a span of whitespace (if the starting index points to whitespace)
		/// or a span of non-whitespace content (if the starting index does not point to whitespace).
		/// </summary>
		/// <param name="index">The index at which the word starts.</param>
		/// <param name="destination">The span which will be the destination for the read content.</param>
		/// <param name="isWhitespace">True if the span is fully comprised of whitespace. If False, no whitespace is present.</param>
		/// <param name="charCount">The amount of characters that were written to <paramref name="destination"/>.</param>
		/// <remarks>
		/// This method may lead to partial reading (for example if <paramref name="destination"/> is not
		/// large enough). When this happens, the return value will be <strong>False</strong> but <paramref name="charCount"/>
		/// will be set to something greater than <c>0</c>.
		/// </remarks>
		/// <returns>False if the buffer is out of bounds or an exception occured.</returns>
		bool TryGetWord(int index, Span<TItem> destination, out bool isWhitespace, out int charCount);

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
		/// <param name="charCount">The amount of characters that were written to <paramref name="destination"/>.</param>
		/// <remarks>
		/// This method may lead to partial reading (for example if <paramref name="destination"/> is not
		/// large enough). When this happens, the return value will be <strong>False</strong> but <paramref name="charCount"/>
		/// will be set to something greater than <c>0</c>.
		/// </remarks>
		/// <returns>False if the buffer is out of bounds, the reading was only partial or an exception occured.</returns>
		bool TryGetWord(int index, TItem itemKind, Span<TItem> destination, out bool isItemKind, out int charCount);

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
		/// <param name="charCount">The amount of characters that were written to <paramref name="destination"/>.</param>
		/// <remarks>
		/// This method may lead to partial reading (for example if <paramref name="destination"/> is not
		/// large enough). When this happens, the return value will be <strong>False</strong> but <paramref name="charCount"/>
		/// will be set to something greater than <c>0</c>.
		/// </remarks>
		/// <returns>False if the buffer is out of bounds, the reading was only partial or an exception occured.</returns>
		bool TryGetWord(int index, Func<int, TItem, bool> itemKindPredicate, Span<TItem> destination, out bool isItemKind, out int charCount);

		/// <summary>
		/// Counts the amount of items until the next non-whitespace item in the buffer, relative to a given <paramref name="index"/>.
		/// Line separators are included in the whitespace category.
		/// </summary>
		/// <returns>The index of the next non-whitespace item, relative to a <paramref name="index"/> or -1 if out of bounds.</returns>
		int CountUntilNotWhitespace(int index);

		/// <summary>
		/// Slices a region of the buffer.
		/// </summary>
		/// <param name="start">The index of the first item in the slice.</param>
		/// <param name="length">The amount of items to slice starting at <paramref name="start"/>.</param>
		TItem[] Slice(int start, int length);

		/// <summary>
		/// Slices a region of the buffer into a performant span.
		/// </summary>
		/// <param name="start">The index of the first item in the slice.</param>
		/// <param name="slice">The span serving as the destination for the slice.</param>
		void Slice(int start, Span<TItem> slice);

		/// <summary>
		/// Gets a single item out of the buffer.
		/// </summary>
		/// <param name="index">The index of the item to retrieve.</param>
		/// <returns>The item.</returns>
		TItem this[int index] { get; }

	}

}
