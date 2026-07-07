using System;
using System.Diagnostics.CodeAnalysis; // needed for .NET 10 despite VS showing it grayed out


namespace OceanApocalypseStudios.RSML.Sources;

/// <summary>
/// Specifies the location of an item in a <see cref="ISource"/> (such as a <see cref="Buffers.IReadOnlyBuffer{TItem}"/>).
/// </summary>
/// <param name="index">The 0-based index.</param>
/// <param name="line">The 0-based line number.</param>
/// <param name="column">The 0-based column number (the index relative to the start of the line).</param>
public readonly struct SourceLocation(int index, int line, int column) : IEquatable<SourceLocation>, IEquatable<int>, IFormattable,
																		 IComparable<SourceLocation>, IComparable<int>
{
	/// <summary>
	/// The 0-based line number, counting from the start of the source.
	/// </summary>
	public int Line => line;

	/// <summary>
	/// The 0-based column number, which is the index of the item relative to the start of the line it is in.
	/// </summary>
	public int Column => column;

	/// <summary>
	/// The absolute 0-based index of the item in the source.
	/// </summary>
	public int Index => index;

	/// <summary>
	/// Compares the index of the location to another index.
	/// </summary>
	/// <param name="other">The index to compare against.</param>
	public int CompareTo(int other) => Index.CompareTo(other);

	/// <inheritdoc/>
	public int CompareTo(SourceLocation other) => throw new NotImplementedException();

	/// <inheritdoc/>
#if NET10_0_OR_GREATER
	public override bool Equals([NotNullWhen(true)] object? obj) =>
#elif NETSTANDARD2_0
	public override bool Equals(object obj) =>
#endif
		obj switch
		{
			SourceLocation location => Equals(location),
			int index               => Index == index,
			_                       => false
		};

	/// <summary>
	/// Checks if two <see cref="SourceLocation"/>s are equal to each other.
	/// </summary>
	/// <param name="other">The other <see cref="SourceLocation"/>.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(SourceLocation other) => Index == other.Index && Line == other.Line && Column == other.Column;

	/// <summary>
	/// Checks if two indexes are equal to each other.
	/// </summary>
	/// <param name="other">The other location's index.</param>
	/// <returns>True if equals.</returns>
	public bool Equals(int other) => Index.Equals(other);

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = InternalUtils.HashCodeSeed * InternalUtils.HashCodeMultiplier + Index.GetHashCode();
			hashCode = hashCode * InternalUtils.HashCodeMultiplier + Line.GetHashCode();

			return hashCode * InternalUtils.HashCodeMultiplier + Column.GetHashCode();
		}
	}

	/// <summary>
	/// Returns a generic string representation of the current instance.
	/// </summary>
	/// <returns>The string representation.</returns>
	public override string ToString() => $"SourceLocation(Index={Index}, Line={Line}, Column={Column})";

	/// <summary>
	/// Given a format, tries to return a string that uses said format as a basis for the representation.
	/// If it fails, it defaults to <see cref="ToString()"/>.
	/// </summary>
	/// <param name="format">The format. Available formats are: CTOR (constructor-like string) and JSON (struct as JSON).</param>
	/// <param name="formatProvider">Unused. Don't bother assigning it anything.</param>
	/// <returns>The string representation.</returns>
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		format switch
		{
			"CTOR" or "I" or "INIT" or "NET" => $"new SourceLocation({Index}, {Line}, {Column})",
			"JSON"                           => $$"""{ "index": {{Index}}, "line": {{Line}}, "column": {{Column}} }""",
			_                                => ToString()
		};

	/// <summary>
	/// Checks if two <see cref="SourceLocation"/>s are equal to each other.
	/// </summary>
	/// <returns>True if equals.</returns>
	public static bool operator ==(SourceLocation left, SourceLocation right) => left.Equals(right);

	/// <summary>
	/// Checks if two <see cref="SourceLocation"/>s are different from each other.
	/// </summary>
	/// <returns>True if different.</returns>
	public static bool operator !=(SourceLocation left, SourceLocation right) => !left.Equals(right);

	/// <summary>
	/// Checks if <paramref name="left"/> is strictly less than <paramref name="right"/>.
	/// </summary>
	public static bool operator <(SourceLocation left, SourceLocation right) => left.Index < right.Index;

	/// <summary>
	/// Checks if <paramref name="left"/> is strictly greater than <paramref name="right"/>.
	/// </summary>
	public static bool operator >(SourceLocation left, SourceLocation right) => left.Index > right.Index;

	/// <summary>
	/// Checks if <paramref name="left"/> is greather than or equal to <paramref name="right"/>.
	/// </summary>
	public static bool operator >=(SourceLocation left, SourceLocation right) => left.Index >= right.Index;

	/// <summary>
	/// Checks if <paramref name="left"/> is less than or equal to <paramref name="right"/>.
	/// </summary>
	public static bool operator <=(SourceLocation left, SourceLocation right) => left.Index <= right.Index;
}
