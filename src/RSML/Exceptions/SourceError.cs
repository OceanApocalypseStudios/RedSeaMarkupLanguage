using System;

using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Exceptions
{

	/// <summary>
	/// An error that has happened in the RSML toolchain and comes from a source.
	/// </summary>
	/// <param name="span">The span the error relates to.</param>
	/// <param name="message">A brief error message detailing why it has happened.</param>
	/// <param name="severity">The error's severity.</param>
	public struct SourceError(SourceSpan span, string message, Severity severity) : IError, IEquatable<SourceError>
	{

		/// <summary>
		/// The span the error relates to.
		/// </summary>
		public readonly SourceSpan Span => span;

		/// <summary>
		/// A brief error message detailing why it has happened.
		/// </summary>
		public readonly string Message => message;

		/// <summary>
		/// The error's severity.
		/// </summary>
		public readonly Severity Severity => severity;

		/// <inheritdoc/>
		public override readonly bool Equals(object? obj) => obj is SourceError error && Equals(error);
		
		/// <inheritdoc/>
		public readonly bool Equals(SourceError other) => Message == other.Message && Severity == other.Severity && Span.Equals(other.Span);

		/// <summary>
		/// Checks if two <see cref="SourceError"/>s are equal to each other.
		/// </summary>
		/// <returns>True if equals.</returns>
		public static bool operator ==(SourceError left, SourceError right) => left.Equals(right);

		/// <summary>
		/// Checks if two <see cref="SourceError"/>s are different from each other.
		/// </summary>
		/// <returns>True if different.</returns>
		public static bool operator !=(SourceError left, SourceError right) => !left.Equals(right);

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{

			unchecked
			{

				int hashCode = InternalUtils.HashCodeSeed * InternalUtils.HashCodeMultiplier + Span.GetHashCode();
				hashCode = hashCode * InternalUtils.HashCodeMultiplier + Message.GetHashCode();
				return hashCode * InternalUtils.HashCodeMultiplier + Severity.GetHashCode();

			}

		}

		/// <inheritdoc/>
		public readonly bool Equals(IError? other) => other is SourceError error && Equals(error);

		/// <summary>
		/// Returns a generic string representation of the current instance.
		/// </summary>
		/// <returns>The string representation.</returns>
		public override readonly string ToString() => $"SourceError(Span={span}, Message={message}, Severity={severity})";

		/// <summary>
		/// Given a format, tries to return a string that uses said format as a basis for the representation.
		/// If it fails, it defaults to <see cref="ToString()"/>.
		/// </summary>
		/// <param name="format">The format. Available formats are: CTOR (constructor-like string), LOG (output-ready format) and JSON (struct as JSON).</param>
		/// <param name="formatProvider">Unused. Don't bother assigning it anything.</param>
		/// <returns>The string representation.</returns>
		public readonly string ToString(string? format, IFormatProvider? formatProvider)
		{

			switch (format)
			{

				case "CTOR":
				case "I":
				case "INIT":
				case "NET":
					return $"new SourceError({Span.ToString("ctor", null)}, \"{Message}\", {Severity})";

				case "LOG":
					if (span.IsSingleLine)
						return $"ERROR: {message} (line {span.Start.Line + 1}, column range {span.Start.Column + 1}..{span.End.Column + 1})";

					return $"ERROR: {message} (line range {span.Start.Line + 1}..{span.End.Line + 1}, column range {span.Start.Column + 1}..{span.End.Column + 1})"


				case "JSON":
					return
						$$"""
						{
							"span": {{span.ToString("JSON", null)}},
							"message": "{{message}}",
							"severity": "{{severity}}"
						}
						""";

				default:
					return ToString();

			}

		}

	}

}
