using System;


namespace OceanApocalypseStudios.RSML.Buffers
{

	/// <summary>
	/// Represents a buffer.
	/// </summary>
	/// <typeparam name="TItem">The datatype of the values returned by the buffer methods</typeparam>
	public interface ISequentialBuffer<TItem> : IDisposable
	{

		/// <summary>
		/// Returns the index of the cursor.
		/// </summary>
		/// <returns>The index of the cursor.</returns>
		int GetCursorIndex();

		/// <summary>
		/// Resets the cursor to the immediately previous state.
		/// </summary>
		/// <remarks>
		/// If multiple actions (let's call them Actions A, B, C and D)
		/// modified the cursor position in the following order:
		/// A -> B -> C -> D; then calling this method after D will set
		/// the cursor position to the one before D was called (the position set by C).
		/// However, calling it a second time will have no effect, as the cursor hasn't been modified.
		/// Resetting the cursor is a non-reversible action.
		/// </remarks>
		void ResetCursorToPreviousState();

		/// <summary>
		/// Counts the amount of items until the next whitespace item in the buffer, relative to the cursor position.
		/// Line separators are included in the whitespace category.
		/// </summary>
		/// <returns>The amount of items until the next whitespace item, relative to the cursor position.</returns>
		int CountUntilWhitespace();

		/// <summary>
		/// Counts the amount of items until the next newline item in the buffer, relative to the cursor position.
		/// Only line separators count - regular whitespace do not.
		/// </summary>
		/// <returns>The index of the next line separator, relative to the cursor position.</returns>
		int CountUntilNewline();

		/// <summary>
		/// Counts the amount of items, starting from the cursor position,
		/// while a <paramref name="predicate"/> returns <c>true</c>.
		/// </summary>
		/// <param name="predicate">
		/// A function that takes the current index (relative to the cursor position),
		/// which is incremented every item, and the item associated with it. Execution stops when
		/// the predicate returns <c>false</c> or the buffer is fully consumed.
		/// </param>
		/// <returns>The amount of items counted.</returns>
		int CountWhile(Func<int, TItem, bool> predicate);

		/// <summary>
		/// Tries to read the next item in the buffer, relative to the cursor index.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>False if the buffer was fully consumed or an exception occured.</returns>
		bool TryRead(out TItem item);

		/// <summary>
		/// Tries to read the next line in the buffer, relative to the cursor index.
		/// </summary>
		/// <param name="line">The destination span that will contain the line.</param>
		/// <param name="consumed">The amount of items that were consumed.</param>
		/// <returns>False if the buffer was fully consumed, there are no more lines to read or an exception occured.</returns>
		bool TryReadLine(Span<TItem> line, out int consumed);

		/// <summary>
		/// Tries to read the current word, which can be a span of whitespace (if the starting index points to whitespace)
		/// or a span of non-whitespace content (if the starting index does not point to whitespace).
		/// </summary>
		/// <param name="destination">The span which will be the destination for the read content.</param>
		/// <param name="isWhitespace">True if the span is fully comprised of whitespace. If False, no whitespace is present.</param>
		/// <param name="consumed">The amount of characters that were consumed.</param>
		/// <remarks>
		/// This method may lead to partial reading (for example if <paramref name="destination"/> is not
		/// large enough). When this happens, the return value will be <strong>False</strong> but <paramref name="consumed"/>
		/// will be set to something greater than <c>0</c>.
		/// </remarks>
		/// <returns>False if the buffer was fully consumed or an exception occured.</returns>
		bool TryReadWord(Span<TItem> destination, out bool isWhitespace, out int consumed);

		/// <summary>
		/// Tries to read the current word, which can be a span of <paramref name="itemKind"/> (if the starting index
		/// points to <paramref name="itemKind"/>) or a span of non-<paramref name="itemKind"/> content (if
		/// the starting index does not point to <paramref name="itemKind"/>).
		/// </summary>
		/// <param name="itemKind">
		/// The item that will serve as the delimiter. For example, if item kind is <c>,</c>, then 
		/// the current word if the buffer is <c> My awesome buffer, isn't it cool?</c> starting from the cursor
		/// position, is <c> My awesome buffer</c>. If item kind is <c>;</c>, then 
		/// the current word if the buffer is <c>;;so very awesome</c> starting from the cursor
		/// position, is <c>;;</c>.
		/// </param>
		/// <param name="destination">The span which will be the destination for the read content.</param>
		/// <param name="isItemKind">
		/// True if the span is fully comprised of <paramref name="itemKind"/>.
		/// If False, no <paramref name="itemKind"/> is present.
		/// </param>
		/// <param name="consumed">The amount of characters that were consumed.</param>
		/// <remarks>
		/// This method may lead to partial reading (for example if <paramref name="destination"/> is not
		/// large enough). When this happens, the return value will be <strong>False</strong> but <paramref name="consumed"/>
		/// will be set to something greater than <c>0</c>.
		/// </remarks>
		/// <returns>False if the buffer was fully consumed or an exception occured.</returns>
		bool TryReadWord(TItem itemKind, Span<TItem> destination, out bool isItemKind, out int consumed);

		/// <summary>
		/// Tries to read until a whitespace character is hit or the buffer is fully consumed.
		/// No whitespaces characters are included in the output value.
		/// </summary>
		/// <param name="destination">The span used as the destination for all the read content.</param>
		/// <param name="consumed">The amount of consumed items.</param>
		/// <returns>False if the buffer was fully consumed or an exception occured.</returns>
		bool TryReadUntilWhitespace(Span<TItem> destination, out int consumed);

		/// <summary>
		/// Reads from the buffer while a <paramref name="predicate"/> returns true.
		/// Starts at the cursor position.
		/// </summary>
		/// <param name="predicate">
		/// A function that takes the current index (relative to the cursor position),
		/// which is incremented every item, and the item associated with it. Execution stops when
		/// the predicate returns <c>false</c> or the buffer is fully consumed.
		/// </param>
		/// <param name="destination">The span used as the destination for all the read content.</param>
		/// <returns>
		/// The amount of read (consumed) items; <c>-1</c> if no items were consumed or an error occured (in which case <paramref name="destination"/> will be empty).
		/// </returns>
		int ReadWhile(Func<int, TItem, bool> predicate, Span<TItem> destination);

		/// <summary>
		/// Consumes from the buffer while a <paramref name="predicate"/> returns true.
		/// Starts at the cursor position. Does not save items.
		/// </summary>
		/// <param name="predicate">
		/// A function that takes the current index (relative to the cursor position),
		/// which is incremented every item, and the item associated with it. Execution stops when
		/// the predicate returns <c>false</c> or the buffer is fully consumed.
		/// </param>
		/// <returns>
		/// The amount of consumed items; <c>-1</c> if no items were consumed or an error occured.
		/// </returns>
		int SkipWhile(Func<int, TItem, bool> predicate);

		/// <summary>
		/// Consumes all whitespace starting at the cursor position and ending at the
		/// first non-whitespace character or the end of the buffer.
		/// </summary>
		/// <returns>
		/// The amount of consumed items; <c>-1</c> if no items were consumed or an error occured.
		/// </returns>
		int SkipWhitespace();

		/// <summary>
		/// Tries to consume, starting at the cursor position, the next item. If the next item
		/// matches <paramref name="item"/>, the method returns True and consumes the item.
		/// Otherwise, the output is False and no item is consumed.
		/// </summary>
		/// <param name="item">The item to consume if it matches.</param>
		/// <returns>
		/// False if the expected <paramref name="item"/> doesn't match the next item, the buffer has been fully consumed
		/// or an error occured.
		/// </returns>
		bool TryConsume(TItem item);

		/// <summary>
		/// Tries to consume, starting at the cursor position, the next sequence of items. If the next items
		/// matchematch, sequentially, <paramref name="items"/>, the method returns True and consumes the items.
		/// Otherwise, the output is False and no item is consumed.
		/// </summary>
		/// <param name="items">The sequence of items to consume if it matches.</param>
		/// <returns>
		/// False if the expected <paramref name="items"/> doesn't match the next item, the buffer has been fully consumed
		/// or an error occured.
		/// </returns>
		bool TryConsume(ReadOnlySpan<TItem> items);

	}

}
