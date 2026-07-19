using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

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
		IReadOnlyStream stream => LexStream(stream, configurations, diagnostics),
		IReadOnlyBuffer buffer => LexBuffer(buffer, configurations, diagnostics),
		_ => []
	};

	/// <summary>
	/// Tokenizes a read-only span buffer passed to the lexer.
	/// </summary>
	/// <returns>The tokens.</returns>
	public static IEnumerable<Token> LexSpan(ReadOnlySpanBuffer buffer, ToolchainConfiguration configuration, DiagnosticCollector collector)
	{
#if NET9_0_OR_GREATER
		return LexBuffer(buffer, configuration, collector);
#else
		throw new System.NotImplementedException(); // todo
#endif
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
#if NET9_0_OR_GREATER
		, allows ref struct
#endif
	{
		int lineNumber = 0;
		bool inString = false;
		int lastQuote = 0;

		while (lineNumber < buffer.LineCount)
		{
			ReadOnlySpan<char> line = buffer.GetLineAsSpan(lineNumber);

			for (int i = 0; i < line.Length; i++)
			{
				// todo: check for comment (greedy)

				if (i == 0 && inString)
				{
					var errorSpan = buffer.GetSourceSpan(lastQuote, i);
					diagnostics.Add(new(new(ErrorCategory.Lexer, 1), errorSpan.IsSuccessful ? errorSpan.Value : SourceSpan.Empty, "String must start and end in the same line.", Severity.Error));
					yield break; // stop lexer execution
				}

				if (line[i] == '\"' && !inString)
				{
					inString = true;
					lastQuote = i;
					continue;
				}

				if (line[i] == '\"' && inString)
				{
					if (line[i - 1] == '\\')
						continue; // escape character

					inString = false;
					lastQuote = i;
					continue;
				}

				if (Char.IsDigit(line[i]))
				{
					// todo: handle integers and floats
				}
			}
		}

		yield return new Token(0, null, new(new(buffer.Length, buffer.LineCount + 1, 0), new(buffer.Length, buffer.LineCount + 1, 0)));
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
		where TStream : IReadOnlyStream
#if NET9_0_OR_GREATER
		, allows ref struct
#endif
		=> throw new System.NotImplementedException(); // todo: implement
}
