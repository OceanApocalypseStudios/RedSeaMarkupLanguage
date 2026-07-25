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
	private readonly static ImmutableArray<string> keywords = ["return", "if", "requires"];

	/// <inheritdoc cref="ILexer.Lex"/>
	public static IEnumerable<Token> Lex(ISource source, ToolchainConfiguration configurations, DiagnosticCollector diagnostics) => source switch
	{
		IReadOnlyScanner stream => LexStream(stream, configurations, diagnostics),
		IReadOnlyBuffer buffer => LexBuffer(buffer, configurations, diagnostics),
		_ => []
	};

	/// <summary>
	/// Tokenizes a read-only span buffer passed to the lexer.
	/// </summary>
	/// <returns>The tokens.</returns>
	public static IEnumerable<Token> LexSpan(ReadOnlySpanBuffer buffer, ToolchainConfiguration configuration, DiagnosticCollector collector)
	{
		throw new NotImplementedException(); // todo
	}

	/// <summary>
	/// Tokenizes a buffer passed to the lexer.
	/// </summary>
	/// <remarks>
	/// > [!NOTE]
	/// > This method avoids boxing.
	/// > [!NOTE]
	/// > This method accepts ref structs if the target framework is
	/// > .NET 9.0 or higher. Otherwise, you might want to take a look at
	/// > <see cref="LexSpan(ReadOnlySpanBuffer, ToolchainConfiguration, DiagnosticCollector)"/>.
	/// </remarks>
	/// <returns>The tokens.</returns>
	public static IEnumerable<Token> LexBuffer<TBuffer>(TBuffer buffer, ToolchainConfiguration configurations, DiagnosticCollector diagnostics)
		where TBuffer : IReadOnlyBuffer
	{
		throw new NotImplementedException(); // todo: implement
	}

	/// <summary>
	/// Tokenizes a stream passed to the lexer.
	/// </summary>
	/// <remarks>
	/// > [!NOTE]
	/// > This method avoids boxing.
	/// </remarks>
	/// <returns>The tokens.</returns>
	public static IEnumerable<Token> LexStream<TStream>(TStream stream, ToolchainConfiguration configurations, DiagnosticCollector diagnostics)
		where TStream : IReadOnlyScanner
		=> throw new NotImplementedException(); // todo: implement
}
