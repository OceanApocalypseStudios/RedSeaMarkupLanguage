using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;


namespace OceanApocalypseStudios.RSML.Native.Structures
{

	/// <summary>
	/// A native-friendly line of RSML containing at most 8 tokens.
	/// </summary>
	[NoReorder]
	[StructLayout(LayoutKind.Sequential)]
	public struct NativeLine(SyntaxLine line)
	{

		/// <summary>
		/// Token 1.
		/// </summary>
		public NativeToken item1 = line.Item1;

		/// <summary>
		/// Token 2.
		/// </summary>
		public NativeToken item2 = line.Item2;

		/// <summary>
		/// Token 3.
		/// </summary>
		public NativeToken item3 = line.Item3;

		/// <summary>
		/// Token 4.
		/// </summary>
		public NativeToken item4 = line.Item4;

		/// <summary>
		/// Token 5.
		/// </summary>
		public NativeToken item5 = line.Item5;

		/// <summary>
		/// Token 6.
		/// </summary>
		public NativeToken item6 = line.Item6;

		/// <summary>
		/// Token 7.
		/// </summary>
		public NativeToken item7 = line.Item7;

		/// <summary>
		/// Token 8.
		/// </summary>
		public NativeToken item8 = line.Item8;

		/// <summary>
		/// Empty native line.
		/// </summary>
		public static NativeLine Empty => new SyntaxLine();

		/// <summary>
		/// Checks if 2 native lines are the same.
		/// </summary>
		/// <param name="left">One of the lines</param>
		/// <param name="right">One of the lines</param>
		/// <returns><c>true</c> if they're equals</returns>
		public static bool operator ==(
			NativeLine left,
			NativeLine right
		) =>
			left.Equals(right);

		/// <summary>
		/// Checks if 2 native lines are different.
		/// </summary>
		/// <param name="left">One of the lines</param>
		/// <param name="right">One of the lines</param>
		/// <returns><c>true</c> if they're different</returns>
		public static bool operator !=(
			NativeLine left,
			NativeLine right
		) =>
			!(left == right);

		/// <inheritdoc />
		public readonly override bool Equals([NotNullWhen(true)] object? obj)
		{

			if (obj is NativeLine nativeLine)
				return Equals(nativeLine);

			if (obj is SyntaxLine managedLine)
				return Equals(managedLine);

			return false;

		}

		/// <summary>
		/// Checks whether two native lines are equals.
		/// </summary>
		/// <param name="line">The other line</param>
		/// <returns>True if equals</returns>
		public readonly bool Equals(NativeLine line) =>
			item1 == line.item1 &&
			item2 == line.item2 &&
			item3 == line.item3 &&
			item4 == line.item4 &&
			item5 == line.item5 &&
			item6 == line.item6 &&
			item7 == line.item7 &&
			item8 == line.item8;

		/// <summary>
		/// Checks whether this native line is equals to a given managed line.
		/// </summary>
		/// <param name="managedLine">The managed line</param>
		/// <returns>True if equals</returns>
		public readonly bool Equals(SyntaxLine managedLine) =>
			item1.Equals(managedLine.Item1) &&
			item2.Equals(managedLine.Item2) &&
			item3.Equals(managedLine.Item3) &&
			item4.Equals(managedLine.Item4) &&
			item5.Equals(managedLine.Item5) &&
			item6.Equals(managedLine.Item6) &&
			item7.Equals(managedLine.Item7) &&
			item8.Equals(managedLine.Item8);

		/// <inheritdoc />
		public readonly override int GetHashCode() =>
			HashCode.Combine(
				item1, item2, item3, item4,
				item5, item6, item7, item8
			);

		/// <summary>
		/// Converts the native RSML syntax line to
		/// a <strong>managed</strong> RSML syntax line.
		/// </summary>
		/// <returns>A managed (regular) line</returns>
		public readonly SyntaxLine ToLine() =>
			new(
				item1.ToToken(),
				item2.ToToken(),
				item3.ToToken(),
				item4.ToToken(),
				item5.ToToken(),
				item6.ToToken(),
				item7.ToToken(),
				item8.ToToken()
			);

		/// <summary>
		/// Converts a <strong>native</strong> RSML syntax line,
		/// referenced by the given <paramref name="linePtr" />
		/// pointer, to a <strong>managed</strong> RSML syntax line.
		/// </summary>
		/// <param name="linePtr">A pointer to the native line to convert</param>
		/// <returns>A managed (regular) line</returns>
		public static unsafe SyntaxLine PointerToLine(NativeLine* linePtr) =>
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
		/// Directly converts from <see cref="SyntaxLine"/> to <see cref="NativeLine"/>.
		/// </summary>
		/// <param name="line"></param>
		public static implicit operator NativeLine(SyntaxLine line) => new(line);

	}

}
