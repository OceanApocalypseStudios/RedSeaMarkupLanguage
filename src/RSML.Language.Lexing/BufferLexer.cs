using System.Collections.Generic;

using OceanApocalypseStudios.RSML.Toolchain.Abstractions.Diagnostics;
using OceanApocalypseStudios.RSML.Toolchain.Abstractions.Sources;

namespace OceanApocalypseStudios.RSML.Language.Lexing;

/// <summary>
/// An implementation of a RSML lexer backed by a read-only or read-and-write buffer.
/// </summary>
/// <param name="buffer">A buffer. Can be read-only (<see cref="IReadOnlyBuffer"/>) or read and write (<see cref="IBuffer"/>).</param>
/// <param name="diagnostics">A collector for all emitted diagnostics.</param>
public class BufferLexer(IReadOnlyBuffer buffer, DiagnosticCollector diagnostics) : Lexer
{
	/// <inheritdoc/>
	public override IEnumerable<Token> Lex() => StaticLexer.LexBuffer(buffer, Configuration, diagnostics);
}
