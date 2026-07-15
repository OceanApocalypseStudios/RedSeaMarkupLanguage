namespace OceanApocalypseStudios.RSML.Diagnostics;

/// <summary>
/// The category of an error, which changes its error code.
/// </summary>
public enum ErrorCategory
{
	/// <summary>
	/// Any general error code that is not restricted to any specific category.
	/// </summary>
	General,

	/// <summary>
	/// An internal error code.
	/// </summary>
	/// <remarks>
	/// These are usually never thrown unless you implement custom lexers, parsers and whatnot.
	/// </remarks>
	Internal,

	/// <summary>
	/// A lexer error code.
	/// </summary>
	/// <seealso cref="Language.Lexing.Lexer"/>
	Lexer,

	/// <summary>
	/// A parser error code.
	/// </summary>
	/// <seealso cref="Language.Parsing.Parser"/>
	Parser,

	/// <summary>
	/// A style error code.
	/// </summary>
	Style
}

