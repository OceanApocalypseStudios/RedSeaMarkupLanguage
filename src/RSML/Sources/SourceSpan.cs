using System;
using System.Diagnostics.CodeAnalysis; // needed despite VS showing it grayed out

using OceanApocalypseStudios.RSML.Sources.Buffers;


namespace OceanApocalypseStudios.RSML.Sources;

/// <summary>
/// Represents a span taken from a source.
/// </summary>
public readonly partial struct SourceSpan(SourceLocation start, SourceLocation end) : IFormattable
{
	/// <summary>
	/// The start of the span.
	/// </summary>
	public readonly SourceLocation Start => start;

	/// <summary>
	/// The end of the span.
	/// </summary>
	public readonly SourceLocation End => end;

	/// <summary>
	/// The length of the span.
	/// </summary>
	public readonly int Length => End.Index - Start.Index;

	/// <summary>
	/// The span is located in a single line.
	/// </summary>
	public bool IsSingleLine => Start.Line == End.Line;

	/// <inheritdoc/>
	#if NET10_0_OR_GREATER
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		#elif NETSTANDARD2_0
	public override bool Equals(object obj) =>
		#endif
		obj is SourceSpan span && Equals(span);

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
			case IReadOnlyBuffer<char> charBuffer:
				Span<char> destination = stackalloc char[Length];
				charBuffer.Slice(Start.Index, destination);

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
			int hashCode = InternalUtils.HashCodeSeed * InternalUtils.HashCodeMultiplier + Start.GetHashCode();

			return hashCode * InternalUtils.HashCodeMultiplier + End.GetHashCode();
		}
	}
}
