using System.Collections.Generic;

using OceanApocalypseStudios.RSML.Toolchain.Abstractions.Diagnostics;
using OceanApocalypseStudios.RSML.Toolchain.Abstractions.Sources;

namespace OceanApocalypseStudios.RSML.Language.Lexing;

/// <summary>
/// An implementation of a RSML lexer backed by a read-only or read-and-write stream.
/// </summary>
/// <param name="stream">A stream. Can be read-only (<see cref="IReadOnlyScanner"/>) or read and write (<see cref="IScanner"/>).</param>
/// <param name="diagnostics">A collector for all emitted diagnostics.</param>
public class StreamLexer(IReadOnlyScanner stream, DiagnosticCollector diagnostics) : Lexer
{
	/// <inheritdoc/>
	public override IEnumerable<Token> Lex() => StaticLexer.LexStream(stream, Configuration, diagnostics);
}
