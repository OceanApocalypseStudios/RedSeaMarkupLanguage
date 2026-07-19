using System.Collections.Generic;

using OceanApocalypseStudios.RSML.Diagnostics;
using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Language.Lexing;

/// <summary>
/// An implementation of a RSML lexer backed by a read-only or read-and-write stream.
/// </summary>
/// <param name="stream">A stream. Can be read-only (<see cref="IReadOnlyStream"/>) or read and write (<see cref="IStream"/>).</param>
/// <param name="diagnostics">A collector for all emitted diagnostics.</param>
public class StreamLexer(IReadOnlyStream stream, DiagnosticCollector diagnostics) : Lexer
{
	/// <inheritdoc/>
	public override IEnumerable<Token> Lex() => StaticLexer.LexStream(stream, Configuration, diagnostics);
}
