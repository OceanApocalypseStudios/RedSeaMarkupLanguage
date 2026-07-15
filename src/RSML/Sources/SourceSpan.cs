using System;


namespace OceanApocalypseStudios.RSML.Sources;

/// <summary>
/// Represents a span taken from a source.
/// </summary>
public readonly partial struct SourceSpan : IFormattable
{
	/// <summary>
	/// An empty span.
	/// </summary>
	public readonly static SourceSpan Empty = new(new(0, 0, 0), new(0, 0, 0));

	/// <summary>
	/// The start of the span.
	/// </summary>
	public readonly SourceLocation Start { get; }

	/// <summary>
	/// The end of the span.
	/// </summary>
	public readonly SourceLocation End { get; }

	/// <summary>
	/// The length of the span.
	/// </summary>
	public readonly int Length => End.Index - Start.Index;

	/// <summary>
	/// The span is located in a single line.
	/// </summary>
	public bool IsSingleLine => Start.Line == End.Line;

	/// <summary>
	/// Initializes a new span given a starting and an end indexes.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="end">The end index.</param>
	/// <exception cref="ArgumentException">The starting index is greater or equal to the end index.</exception>
	public SourceSpan(SourceLocation start, SourceLocation end)
	{
		if (start.Index > end.Index)
			throw new ArgumentException("The starting index must be less than the end index.");

		Start = start;
		End = end;
	}

	/// <inheritdoc/>
	public override bool Equals(
#if NET8_0_OR_GREATER
		[NotNullWhen(true)]
		object? obj
#else
		object obj
#endif
	) => obj is SourceSpan span && Equals(span);

	/// <summary>
	/// Checks whether two <see cref="SourceSpan"/>s are equals.
	/// </summary>
	/// <param name="other">The span to check against</param>
	/// <returns>True if equals</returns>
	public bool Equals(SourceSpan? other) => other is SourceSpan span && Start.Equals(span.Start) && End.Equals(span.End);

	/// <summary>
	/// Checks whether two <see cref="SourceSpan"/>s are equals.
	/// </summary>
	/// <returns>True if equals</returns>
	public static bool operator ==(SourceSpan left, SourceSpan right) => left.Equals(right);

	/// <summary>
	/// Checks whether two <see cref="SourceSpan"/>s are different from each other.
	/// </summary>
	/// <returns>True if different</returns>
	public static bool operator !=(SourceSpan left, SourceSpan right) => left.Equals(right);

	/// <summary>
	/// Returns a generic string representation of the current instance.
	/// </summary>
	/// <returns>The string representation.</returns>
	public override string ToString() => $"SourceSpan(Start={Start}, End={End})";

	/// <summary>
	/// Given a source, tries to return a string that uses said source as a basis for the representation.
	/// If it fails, it defaults to <see cref="ToString()"/>.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <returns>The string representation.</returns>
	public string ToString(ISource source)
	{
		switch (source)
		{
			case IBuffer<char> charBuffer:
				Span<char> destination = stackalloc char[Length];
				charBuffer.TrySlice(Start.Index, destination);

				return destination.ToString();

			default:
				return ToString();
		}
	}

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
			"CTOR" or "I" or "INIT" or "NET" => $"new SourceSpan({Start.ToString("ctor", null)}, {End.ToString("ctor", null)})",
			"JSON"                           => $$"""{ "start": {{Start.ToString("JSON", null)}}, "end": {{End.ToString("JSON", null)}} }""",
			_                                => ToString()
		};

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = Constants.HashCodeSeed * Constants.HashCodeMultiplier + Start.GetHashCode();

			return hashCode * Constants.HashCodeMultiplier + End.GetHashCode();
		}
	}
}
