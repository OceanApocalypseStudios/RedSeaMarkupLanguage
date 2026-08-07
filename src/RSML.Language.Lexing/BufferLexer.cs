using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using OceanApocalypse.RSML.Language.Lexing.Diagnostics;
using OceanApocalypse.RSML.Language.Lexing.Tokens;
using OceanApocalypse.RSML.Toolchain.Abstractions;
using OceanApocalypse.RSML.Toolchain.Abstractions.Diagnostics;
using OceanApocalypse.RSML.Toolchain.Abstractions.Sources;

namespace OceanApocalypse.RSML.Language.Lexing;

/// <summary>
/// An implementation of a RSML lexer backed by a read-only or read-and-write buffer.
/// </summary>
/// <param name="buffer">A buffer. Can be read-only (<see cref="IBuffer"/>) or read and write (<see cref="IBuffer"/>).</param>
/// <param name="diagnostics">A collector for all emitted diagnostics.</param>
public class BufferLexer(IBuffer buffer, DiagnosticCollector diagnostics) : Lexer
{
	private static readonly ImmutableArray<string> keywords = [
		// keywords
		"as", "end", "if", "let", "region", "requires", "return", "struct", "type",
		// modifiers
		"fn", "mut", "previous",
		// reserved keywords - not yet implemented but blocked from being used as identifiers
		"class"
	];
	private int cursor;

	/// <remarks>
	/// :::note[Diagnostic output]
	/// This method does not add diagnostics to the collector
	/// (<see cref="DiagnosticCollector"/>): it only returns them when it
	/// proves necessary.
	/// :::
	/// </remarks>
	/// <inheritdoc/>
	public override Result<Token> GetNextToken()
	{
		SkipWhitespaceAndComments();

		if (cursor >= buffer.Length)
			return Result.Success(new Token(TokenKind.Eof, null, SourceSpan.Empty));

		var startLoc = buffer.GetSourceLocation(cursor);

		char c = buffer[cursor];

		if (c == '"')
			return ScanStringLiteral(startLoc);

		if (Char.IsAsciiDigit(c))
			return ScanNumber(startLoc);

		if (Char.IsAsciiLetter(c) || c == '_')
			return ScanIdentifierOrKeyword(startLoc);

		// todo: add the remaining possible paths
		return Result.Failure<Token>(new(LexerErrorCodes.FailedToLexToken, "Tried all possible token logic paths, but none was true.", Severity.Error));
	}

	/// <inheritdoc/>
	public override IEnumerable<Token> Lex()
	{
		int maxFailedRunsLimit = 10;
		int failedRuns = 0;

		while (failedRuns < maxFailedRunsLimit)
		{
			var token = GetNextToken();

			if (token.IsError)
			{
				diagnostics.Add(token.Error);
				failedRuns++;
				continue;
			}

			if (token.Value.Kind == TokenKind.Eof)
				yield break;

			else
				yield return token.Value;
		}
	}

	private Result<Token> ScanNumber(SourceLocation startLoc)
	{
		bool dot = false;

		while (cursor < buffer.Length && (Char.IsAsciiDigit(buffer[cursor]) || buffer[cursor] == '_' || buffer[cursor] == '.'))
		{
			if (buffer[cursor] == '.')
			{
				if (dot)
					return Result.Success(new Token(TokenKind.Number, null, new(startLoc, buffer.GetSourceLocation(cursor - 1))));

				else
					dot = true;
			}

			cursor++;
		}

		return Result.Success(new Token(TokenKind.Number, null, new(startLoc, buffer.GetSourceLocation(cursor))));
	}

	private Result<Token> ScanStringLiteral(SourceLocation startLoc)
	{
		cursor++;
		bool escaping = false;

		while (cursor < buffer.Length)
		{
			if (buffer[cursor].IsNewline())
			{
				return Result.Failure<Token>(new(
					LexerErrorCodes.UnterminatedStringLiteral,
					new SourceSpan(startLoc, new(cursor, startLoc.Line, cursor - startLoc.Index + startLoc.Column)),
					"A string literal must begin and end in the same line.",
					Severity.Error
				));
			}

			if (buffer[cursor] == '"' && !escaping)
				break;

			if (buffer[cursor] == '\\')
				escaping = !escaping;

			cursor++;
		}

		if (cursor < buffer.Length)
			cursor++; // skip end quote if anything beyond it

		return Result.Success(new Token(TokenKind.StringLiteral, null, new(startLoc, buffer.GetSourceLocation(cursor))));
	}

	private Result<Token> ScanIdentifierOrKeyword(SourceLocation startLoc)
	{
		while (cursor < buffer.Length && (Char.IsAsciiLetterOrDigit(buffer[cursor]) || buffer[cursor] == '_'))
			cursor++;

		SourceSpan span = new(startLoc, buffer.GetSourceLocation(cursor));

		return keywords.Contains(buffer[span])
			? Result.Success(new Token(GetKeywordTokenKind(buffer[span]), null, span)) // is keyword
			: Result.Success(new Token(TokenKind.Identifier, null, span)); // is identifier
	}

	private static TokenKind GetKeywordTokenKind(scoped ReadOnlySpan<char> keyword) => keyword switch
	{
		// keywords
		"as" => TokenKind.As,
		"end" => TokenKind.End,
		"if" => TokenKind.If,
		"let" => TokenKind.Let,
		"region" => TokenKind.Region,
		"requires" => TokenKind.Requires,
		"return" => TokenKind.Return,
		"struct" => TokenKind.Struct,
		"type" => TokenKind.Type,

		// modifiers
		"fn" => TokenKind.FunctionModifier,
		"mut" => TokenKind.MutableModifier,
		"previous" => TokenKind.PreviousModifier,

		_ => TokenKind.Unknown,
	};

	private void SkipWhitespaceAndComments()
	{
		while (cursor < buffer.Length)
		{
			char c = buffer[cursor];

			if (Char.IsWhiteSpace(c))
			{
				cursor += buffer.CountUntilNotWhitespace(cursor);
			}
			else if (c == '#')
			{
				while (!buffer[cursor].IsNewline())
					cursor++;
			}
			else
			{
				break;
			}
		}
	}
}
