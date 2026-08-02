using System.Collections.Generic;

using OceanApocalypseStudios.RSML.Language.Lexing.Tokens;
using OceanApocalypseStudios.RSML.Toolchain.Abstractions.Diagnostics;
using OceanApocalypseStudios.RSML.Toolchain.Abstractions.Sources;

namespace OceanApocalypseStudios.RSML.Language.Lexing;

/// <summary>
/// An implementation of a RSML lexer backed by a read-only or read-and-write stream.
/// </summary>
/// <param name="stream">A stream. Can be read-only (<see cref="IScanner"/>) or read and write (<see cref="IScanner"/>).</param>
/// <param name="diagnostics">A collector for all emitted diagnostics.</param>
public class StreamLexer(IScanner stream, DiagnosticCollector diagnostics) : Lexer
{
	/// <inheritdoc/>
	public override Result<Token> GetNextToken() => throw new System.NotImplementedException();

	/// <inheritdoc/>
	public override IEnumerable<Token> Lex() => throw new System.NotImplementedException();
}
