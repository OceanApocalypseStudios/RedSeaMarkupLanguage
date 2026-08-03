using System;
using System.Diagnostics.CodeAnalysis;

using OceanApocalypse.RSML.Toolchain.Abstractions.Sources;


namespace OceanApocalypse.RSML.Toolchain.Abstractions.Diagnostics;

/// <summary>
/// A diagnostic reported by RSML's API.
/// </summary>
public readonly struct Diagnostic : IFormattable, IEquatable<Diagnostic>
{
	/// <summary>
	/// The span the error relates to.
	/// </summary>
	public SourceSpan Span { get; }

	/// <summary>
	/// The error's code. Contains information about the category of the error.
	/// </summary>
	public string Code { get; }

	/// <summary>
	/// Checks whether the error is internal (API error results, for example).
	/// </summary>
	public bool IsInternal => Code[1] == 'I';

	/// <summary>
	/// A brief error message detailing why it has happened.
	/// </summary>
	public string Message { get; }

	/// <summary>
	/// The error's severity.
	/// </summary>
	public Severity Severity { get; }

	/// <summary>Creates a new diagnostic with a basic error code.</summary>
	/// <param name="code">The error code.</param>
	public Diagnostic(string code)
	{
		ArgumentNullException.ThrowIfNullOrWhiteSpace(code);
		ThrowIfInvalidErrorCode(code);

		Code = code;
		Span = SourceSpan.Empty;
		Message = "";
		Severity = Severity.None;
	}

	/// <summary>Creates a new diagnostic.</summary>
	/// <param name="code">The error code.</param>
	/// <param name="message">A brief error message detailing why it has happened.</param>
	public Diagnostic(string code, string message)
	{
		ArgumentNullException.ThrowIfNullOrWhiteSpace(code);
		ThrowIfInvalidErrorCode(code);

		Code = code;
		Span = SourceSpan.Empty;
		Message = message;
		Severity = Severity.None;
	}

	/// <summary>Creates a new diagnostic.</summary>
	/// <param name="code">The error code.</param>
	/// <param name="severity">The error's severity.</param>
	public Diagnostic(string code, Severity severity)
	{
		ArgumentNullException.ThrowIfNullOrWhiteSpace(code);
		ThrowIfInvalidErrorCode(code);

		Code = code;
		Span = SourceSpan.Empty;
		Message = "";
		Severity = severity;
	}

	/// <summary>Creates a new diagnostic.</summary>
	/// <param name="code">The error code.</param>
	/// <param name="message">A brief error message detailing why it has happened.</param>
	/// <param name="severity">The error's severity.</param>
	public Diagnostic(string code, string message, Severity severity)
	{
		ArgumentNullException.ThrowIfNullOrWhiteSpace(code);
		ThrowIfInvalidErrorCode(code);

		Code = code;
		Span = SourceSpan.Empty;
		Message = message;
		Severity = severity;
	}

	/// <summary>Creates a new diagnostic.</summary>
	/// <param name="code">The error code.</param>
	/// <param name="span">The span the error relates to.</param>
	/// <param name="message">A brief error message detailing why it has happened.</param>
	/// <param name="severity">The error's severity.</param>
	public Diagnostic(string code, SourceSpan span, string message, Severity severity)
	{
		ArgumentNullException.ThrowIfNullOrWhiteSpace(code);
		ThrowIfInvalidErrorCode(code);

		Code = code;
		Span = span;
		Message = message;
		Severity = severity;
	}

	/// <inheritdoc/>
	public override bool Equals(
		[NotNullWhen(true)]
		object? obj
	) => obj is Diagnostic error && Equals(error);

	/// <inheritdoc/>
	public bool Equals(Diagnostic other) => Message == other.Message && Code == other.Code && Severity == other.Severity && Span.Equals(other.Span);

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
	public override int GetHashCode() => unchecked(HashCode.Combine(Span, Code, Message, Severity));

	/// <summary>
	/// Returns a generic string representation of the current instance.
	/// </summary>
	/// <returns>The string representation.</returns>
	public override string ToString() => $"Diagnostic(Code={Code}, Span={Span}, Message={Message}, Severity={Severity})";

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
				return $"new Diagnostic(\"{Code}\", {Span.ToString("ctor", null)}, \"{Message}\", {Severity})";

			case "LOG":
				string prefix = Severity switch
				{
					Severity.Message => "INFO ",
					Severity.Warning => "WARNING ",
					Severity.Error => "ERROR ",
					Severity.Critical => "CRITICAL ",
					_ => ""
				};

				if (Span.IsSingleLine)
					return $"[{prefix}{Code}] @ L{Span.Start.Line + 1},C({Span.Start.Column + 1}..{Span.End.Column + 1}) : {Message}";

				return $"[{prefix}{Code}] @ L({Span.Start.Line + 1}..{Span.End.Line + 1}),C({Span.Start.Column + 1}..{Span.End.Column + 1}) : {Message}";

			case "JSON":
				return
					$$"""
					  {
					    "errorCode": "{{Code}}",
					  	"span": {{Span.ToString("JSON", null)}},
					  	"message": "{{Message}}",
					  	"severity": "{{Severity}}"
					  }
					  """;

			default:
				return ToString();
		}
	}

	private static void ThrowIfInvalidErrorCode(string code, string? paramName = null)
	{
		if (code.Length != 6 || code[0] != 'R' || code[1] is not 'I' and not 'L' and not 'S' and not 'P' and not '-' || Char.IsAsciiDigit(code[2]) || Char.IsAsciiDigit(code[3]) || Char.IsAsciiDigit(code[4]) || Char.IsAsciiDigit(code[5]))
			throw new ArgumentException("The error code is not in the correct format. Correct format in Regex is: R(-|I|L|P|S)\\d\\d\\d\\d", paramName ?? nameof(code));
	}
}
