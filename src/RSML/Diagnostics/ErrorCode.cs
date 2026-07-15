namespace OceanApocalypseStudios.RSML.Diagnostics;

/// <summary>
/// Represents a code specific to an error.
/// </summary>
/// <param name="Category">The category of the error.</param>
/// <param name="Code">The error code.</param>
public record struct ErrorCode(ErrorCategory Category, int Code)
{
	/// <summary>
	/// The prefix for all error codes related to RSML.
	/// </summary>
	public const char LanguagePrefix = 'R';

	/// <summary>
	/// The specific prefix for each error category.
	/// </summary>
	public readonly char CategoryPrefix => Category switch
	{
		ErrorCategory.Internal => 'I',
		ErrorCategory.Lexer => 'L',
		ErrorCategory.Parser => 'P',
		ErrorCategory.Style => 'S',
		_ => 'E'
	};

	/// <summary>
	/// Returns the formatted error code.
	/// </summary>
	/// <returns>The formatted error code.</returns>
	public readonly override string ToString() => $"{LanguagePrefix}{CategoryPrefix}{Code:0000}";
}
