using OceanApocalypseStudios.RSML.Sources;

namespace OceanApocalypseStudios.RSML.Language.Lexing;


public record struct Token(int TokenKind, object? Value, SourceSpan Span);
