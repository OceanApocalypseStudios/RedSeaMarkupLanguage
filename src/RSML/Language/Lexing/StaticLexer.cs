using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using OceanApocalypseStudios.RSML.Diagnostics;
using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Language.Lexing;

/// <summary>
/// A static lexer for when you don't need to lex from a builder.
/// </summary>
public static class StaticLexer
{
	public readonly static ImmutableArray<string> Keywords = ["return", "if", "requires"];

	/// <summary>
	/// Tokenizes a read-only span buffer passed to the lexer.
	/// </summary>
	/// <returns>The tokens.</returns>
	public static ImmutableArray<Token> LexSpan(ReadOnlySpanBuffer buffer, ToolchainConfiguration configuration, DiagnosticCollector collector) => throw new NotImplementedException(); // todo

	/// <summary>
	/// Tokenizes a buffer passed to the lexer.
	/// </summary>
	/// <returns>The tokens.</returns>
	public static IEnumerable<Token> LexBuffer<TBuffer>(TBuffer buffer, ToolchainConfiguration configurations, DiagnosticCollector diagnostics)
		where TBuffer : IReadOnlyBuffer => throw new NotImplementedException(); // todo: implement

	/// <summary>
	/// Tokenizes a stream passed to the lexer.
	/// </summary>
	/// <returns>The tokens.</returns>
	public static IEnumerable<Token> LexStream<TStream>(TStream stream, ToolchainConfiguration configurations, DiagnosticCollector diagnostics)
		where TStream : IReadOnlyScanner
		=> throw new NotImplementedException(); // todo: implement
}
