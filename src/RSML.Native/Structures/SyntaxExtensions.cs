using System;
using System.Runtime.CompilerServices;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;


namespace OceanApocalypseStudios.RSML.Native.Structures
{

	/// <summary>
	/// Extensions to ease the conversions between
	/// <see cref="SyntaxToken" /> and <see cref="SyntaxLine" /> and
	/// their native counterparts (<see cref="NativeToken" />
	/// and <see cref="NativeLine" />).
	/// </summary>
	public static class SyntaxExtensions
	{

		/// <summary>
		/// Copies a <strong>managed</strong> RSML syntax line
		/// into a <strong>native</strong> RSML syntax line via dereferencing.
		/// </summary>
		/// <param name="line">The line to copy</param>
		/// <param name="linePtr">A pointer referencing the output</param>
		public static unsafe void CopyToNative(
			this SyntaxLine line,
			NativeLine* linePtr
		) =>
			*linePtr = line.ToNativeLine();

		/// <summary>
		/// Converts a <strong>native</strong> RSML syntax line,
		/// referenced by the given <paramref name="linePtr" />
		/// pointer, to a <strong>managed</strong> RSML syntax line.
		/// </summary>
		/// <param name="linePtr">A pointer to the native line to convert</param>
		/// <returns>A managed (regular) line</returns>
		public static unsafe SyntaxLine PtrToLine(NativeLine* linePtr) =>
			new(
				linePtr->item1.ToToken(),
				linePtr->item2.ToToken(),
				linePtr->item3.ToToken(),
				linePtr->item4.ToToken(),
				linePtr->item5.ToToken(),
				linePtr->item6.ToToken(),
				linePtr->item7.ToToken(),
				linePtr->item8.ToToken()
			);

		/// <summary>
		/// Converts a <strong>native</strong> RSML syntax line to
		/// a <strong>managed</strong> RSML syntax line.
		/// </summary>
		/// <param name="line">The native line to convert</param>
		/// <returns>A managed (regular) line</returns>
		public static SyntaxLine ToLine(this NativeLine line) =>
			new(
				line.item1.ToToken(),
				line.item2.ToToken(),
				line.item3.ToToken(),
				line.item4.ToToken(),
				line.item5.ToToken(),
				line.item6.ToToken(),
				line.item7.ToToken(),
				line.item8.ToToken()
			);

		/// <summary>
		/// Converts a <strong>managed</strong> RSML syntax line to a
		/// <strong>native</strong> RSML syntax line.
		/// </summary>
		/// <param name="line">The line to convert</param>
		/// <returns>A native line</returns>
		public static NativeLine ToNativeLine(this SyntaxLine line) =>
			new()
			{

				item1 = line.Item1.ToNativeToken(),
				item2 = line.Item2.ToNativeToken(),
				item3 = line.Item3.ToNativeToken(),
				item4 = line.Item4.ToNativeToken(),
				item5 = line.Item5.ToNativeToken(),
				item6 = line.Item6.ToNativeToken(),
				item7 = line.Item7.ToNativeToken(),
				item8 = line.Item8.ToNativeToken()

			};

		/// <summary>
		/// Converts a <strong>managed</strong> RSML syntax token to a
		/// <strong>native</strong> RSML syntax token.
		/// </summary>
		/// <param name="token">The token to convert</param>
		/// <returns>A native token</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NativeToken ToNativeToken(this SyntaxToken token) =>
			new()
			{

				kind = (byte)token.Kind,
				startIndex = token.BufferRange.Start.IsFromEnd
								 ? -token.BufferRange.Start.Value
								 : token.BufferRange.Start.Value,
				endIndex = token.BufferRange.End.IsFromEnd
							   ? -token.BufferRange.End.Value
							   : token.BufferRange.End.Value

			};

		/// <summary>
		/// Converts a <strong>native</strong> RSML syntax token to a
		/// <strong>managed</strong> RSML syntax token.
		/// </summary>
		/// <param name="token">The native token to convert</param>
		/// <returns>A managed (regular) token</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SyntaxToken ToToken(this NativeToken token) =>
			new((TokenKind)token.kind, new(Math.Abs(token.startIndex), token.startIndex < 0), new(Math.Abs(token.endIndex), token.endIndex < 0));

	}

}
