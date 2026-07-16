using OceanApocalypseStudios.RSML.Language.Lexing;
using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Extensibility;

/// <summary>
/// Specifies a rule for the lexer to apply.
/// </summary>
public interface ILexerRule
{
	/// <summary>
	/// Tries to match a token via the lexer.
	/// </summary>
	/// <param name="source">The source being read.</param>
	/// <param name="position">The cursor position.</param>
	/// <param name="token">The output token, if valid.</param>
	/// <returns>True if the token was a match, False if not.</returns>
	bool TryMatch(ISource source, SourceLocation position, out Token token);
}
