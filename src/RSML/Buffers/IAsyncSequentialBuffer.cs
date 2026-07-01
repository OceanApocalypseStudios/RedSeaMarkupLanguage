using System;
using System.Threading;
using System.Threading.Tasks;


namespace OceanApocalypseStudios.RSML.Buffers
{

	/// <summary>
	/// Represents a sequential buffer that supports async functionality.
	/// </summary>
	/// <typeparam name="TItem">The datatype of the values returned by the buffer methods</typeparam>
	public partial interface IAsyncSequentialBuffer<TItem>
	{

		/// <summary>
		/// Tries to read the next item in the buffer, relative to the cursor index.
		/// </summary>
		/// <param name="token">A cancellation token.</param>
		/// <param name="item">The item.</param>
		/// <returns>False if the buffer was fully consumed or an exception occured.</returns>
		Task<bool> TryReadAsync(CancellationToken? token, out TItem item);

		/// <summary>
		/// Tries to read the next line in the buffer, relative to the cursor index.
		/// </summary>
		/// <param name="line">The destination span that will contain the line.</param>
		/// <param name="token">A cancellation token.</param>
		/// <param name="consumed">The amount of items that were consumed.</param>
		/// <returns>False if the buffer was fully consumed, there are no more lines to read or an exception occured.</returns>
		Task<bool> TryReadLineAsync(ReadOnlySpan<TItem> line, CancellationToken? token, out int consumed);

		/// <summary>
		/// Tries to read the current word, which can be a span of whitespace (if the starting index points to whitespace)
		/// or a span of non-whitespace content (if the starting index does not point to whitespace).
		/// </summary>
		/// <param name="destination">The span which will be the destination for the read content.</param>
		/// <param name="token">A cancellation token.</param>
		/// <param name="isWhitespace">True if the span is fully comprised of whitespace. If False, no whitespace is present.</param>
		/// <param name="consumed">The amount of characters that were consumed.</param>
		/// <remarks>
		/// This method may lead to partial reading (for example if <paramref name="destination"/> is not
		/// large enough). When this happens, the return value will be <strong>False</strong> but <paramref name="consumed"/>
		/// will be set to something greater than <c>0</c>.
		/// </remarks>
		/// <returns>False if the buffer was fully consumed or an exception occured.</returns>
		Task<bool> TryReadWordAsync(ReadOnlySpan<TItem> destination, CancellationToken? token, out bool isWhitespace, out int consumed);

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
		/// <param name="token">A cancellation token.</param>
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
		Task<bool> TryReadWordAsync(TItem itemKind, ReadOnlySpan<TItem> destination, CancellationToken? token, out bool isItemKind, out int consumed);

		/// <summary>
		/// Tries to read until a whitespace character is hit or the buffer is fully consumed.
		/// No whitespaces characters are included in the output value.
		/// </summary>
		/// <param name="destination">The span used as the destination for all the read content.</param>
		/// <param name="token">A cancellation token.</param>
		/// <param name="consumed">The amount of consumed items.</param>
		/// <returns>False if the buffer was fully consumed or an exception occured.</returns>
		Task<bool> TryReadUntilWhitespaceAsync(ReadOnlySpan<TItem> destination, CancellationToken? token, out int consumed);

		/// <summary>
		/// Consumes all whitespace starting at the cursor position and ending at the
		/// first non-whitespace character or the end of the buffer.
		/// </summary>
		/// <param name="token">A cancellation token.</param>
		/// <returns>
		/// The amount of consumed items; <c>-1</c> if no items were consumed or an error occured.
		/// </returns>
		Task<int> SkipWhitespaceAsync(CancellationToken? token);

		/// <summary>
		/// Tries to consume, starting at the cursor position, the next item. If the next item
		/// matches <paramref name="item"/>, the method returns True and consumes the item.
		/// Otherwise, the output is False and no item is consumed.
		/// </summary>
		/// <param name="item">The item to consume if it matches.</param>
		/// <param name="token">A cancellation token.</param>
		/// <returns>
		/// False if the expected <paramref name="item"/> doesn't match the next item, the buffer has been fully consumed
		/// or an error occured.
		/// </returns>
		Task<bool> TryConsumeAsync(TItem item, CancellationToken? token);

		/// <summary>
		/// Tries to consume, starting at the cursor position, the next sequence of items. If the next items
		/// matchematch, sequentially, <paramref name="items"/>, the method returns True and consumes the items.
		/// Otherwise, the output is False and no item is consumed.
		/// </summary>
		/// <param name="items">The sequence of items to consume if it matches.</param>
		/// <param name="token">A cancellation token.</param>
		/// <returns>
		/// False if the expected <paramref name="items"/> doesn't match the next item, the buffer has been fully consumed
		/// or an error occured.
		/// </returns>
		Task<bool> TryConsumeAsync(ReadOnlySpan<TItem> items, CancellationToken? token);

	}

}
