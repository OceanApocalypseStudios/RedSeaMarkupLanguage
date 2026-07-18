using OceanApocalypseStudios.RSML.Sources;

namespace OceanApocalypseStudios.RSML.Language.Lexing;


/// <summary>
/// Represents a RSML token.
/// </summary>
/// <param name="TokenKind">An integer that identifies the type of token.</param>
/// <param name="Value">The token's value.</param>
/// <param name="Span">The span where the token occurs.</param>
public record struct Token(int TokenKind, object? Value, SourceSpan Span);
