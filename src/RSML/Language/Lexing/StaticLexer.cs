using System.Collections.Generic;

using OceanApocalypseStudios.RSML.Diagnostics;
using OceanApocalypseStudios.RSML.Sources;

namespace OceanApocalypseStudios.RSML.Language.Lexing;

/// <summary>
/// A static lexer for when you don't need to apply specific configurations
/// or use extra features provided in <see cref="ILexer"/>.
/// </summary>
public static class StaticLexer
{
	/// <inheritdoc cref="ILexer.Lex"/>
	public static Result<IEnumerable<Token>> Lex(ISource source, ToolchainConfiguration configurations, DiagnosticCollector diagnostics) => source switch
	{
		IReadOnlyStream stream => LexStream(stream, configurations, diagnostics),
		IReadOnlyBuffer buffer => LexBuffer(buffer, configurations, diagnostics),
		_ => Result<IEnumerable<Token>>.Fail(new(new(ErrorCategory.Internal, 3), SourceSpan.Empty, "Source is not one of the allowed types.", Severity.None))
	};

	internal static Result<IEnumerable<Token>> LexBuffer(IReadOnlyBuffer buffer, ToolchainConfiguration configurations, DiagnosticCollector diagnostics) =>
		throw new System.NotImplementedException(); // todo: implement

	internal static Result<IEnumerable<Token>> LexStream(IReadOnlyStream stream, ToolchainConfiguration configurations, DiagnosticCollector diagnostics) =>
		throw new System.NotImplementedException(); // todo: implement
}
