using System;

using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Diagnostics;

/// <summary>
/// A diagnostic reported by RSML's API.
/// </summary>
/// <param name="code">The error code.</param>
/// <param name="span">The span the error relates to.</param>
/// <param name="message">A brief error message detailing why it has happened.</param>
/// <param name="severity">The error's severity.</param>
public readonly struct Diagnostic(ErrorCode code, SourceSpan span, string message, Severity severity) : IFormattable, IEquatable<Diagnostic>
{
	/// <summary>
	/// The span the error relates to.
	/// </summary>
	public SourceSpan Span => span;

	/// <summary>
	/// The error's code. Contains information about the category of the error.
	/// </summary>
	public ErrorCode Code => code;

	/// <summary>
	/// The error's category.
	/// </summary>
	public ErrorCategory Category => Code.Category;

	/// <summary>
	/// Checks whether the error is internal (API error results, for example).
	/// </summary>
	public bool IsInternal => Code.Category == ErrorCategory.Internal;

	/// <summary>
	/// A brief error message detailing why it has happened.
	/// </summary>
	public string Message => message;

	/// <summary>
	/// The error's severity.
	/// </summary>
	public Severity Severity => severity;

	/// <inheritdoc/>
	public override bool Equals(
#if NET8_0_OR_GREATER
		[NotNullWhen(true)]
		object? obj
#else
		object obj
#endif
	) => obj is Diagnostic error && Equals(error);

	/// <inheritdoc/>
	public bool Equals(Diagnostic other) => Message == other.Message && Severity == other.Severity && Span.Equals(other.Span);

	/// <summary>
	/// Checks if two <see cref="Diagnostic"/>s are equal to each other.
	/// </summary>
	/// <returns>True if equals.</returns>
	public static bool operator ==(Diagnostic left, Diagnostic right) => left.Equals(right);

	/// <summary>
	/// Checks if two <see cref="Diagnostic"/>s are different from each other.
	/// </summary>
	/// <returns>True if different.</returns>
	public static bool operator !=(Diagnostic left, Diagnostic right) => !left.Equals(right);

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = Constants.HashCodeSeed * Constants.HashCodeMultiplier + Span.GetHashCode();
			hashCode = hashCode * Constants.HashCodeMultiplier + Message.GetHashCode();

			return hashCode * Constants.HashCodeMultiplier + Severity.GetHashCode();
		}
	}

	/// <summary>
	/// Returns a generic string representation of the current instance.
	/// </summary>
	/// <returns>The string representation.</returns>
	public override string ToString() => $"Diagnostic(Span={span}, Message={message}, Severity={severity})";

	/// <summary>
	/// Given a format, tries to return a string that uses said format as a basis for the representation.
	/// If it fails, it defaults to <see cref="ToString()"/>.
	/// </summary>
	/// <param name="format">The format. Available formats are: CTOR (constructor-like string), LOG (output-ready format) and JSON (struct as JSON).</param>
	/// <param name="formatProvider">Unused. Don't bother assigning it anything.</param>
	/// <returns>The string representation.</returns>
	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		switch (format)
		{
			case "CTOR":
			case "I":
			case "INIT":
			case "NET":
				return $"new Diagnostic({Span.ToString("ctor", null)}, \"{Message}\", {Severity})";

			case "LOG":
				string prefix = Severity switch
				{
					Severity.Message => "INFO ",
					Severity.Warning => "WARNING ",
					Severity.Error => "ERROR ",
					Severity.Critical => "CRITICAL ",
					_ => ""
				};

				if (span.IsSingleLine)
					return $"[{prefix}{Code}] @ L{span.Start.Line + 1},C({span.Start.Column + 1}..{span.End.Column + 1}) : {message}";

				return $"[{prefix}{Code}] @ L({span.Start.Line + 1}..{span.End.Line + 1}),C({span.Start.Column + 1}..{span.End.Column + 1}) : {message}";

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
