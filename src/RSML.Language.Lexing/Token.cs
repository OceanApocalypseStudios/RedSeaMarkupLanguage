using OceanApocalypseStudios.RSML.Toolchain.Abstractions.Sources;

namespace OceanApocalypseStudios.RSML.Language.Lexing;


/// <summary>
/// Represents a RSML token.
/// </summary>
/// <param name="TokenKind">An integer that identifies the type of token.</param>
/// <param name="Value">The token's value.</param>
/// <param name="Span">The span where the token occurs.</param>
public record struct Token(int TokenKind, object? Value, SourceSpan Span)
{
	/// <summary>
	/// Empty token. Used when something goes wrong.
	/// </summary>
	public readonly static Token Empty = new(-1, null, SourceSpan.Empty);
}
