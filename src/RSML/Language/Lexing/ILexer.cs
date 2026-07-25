using System.Collections.Generic;


namespace OceanApocalypseStudios.RSML.Language.Lexing;

/// <summary>
/// Represents a lexer for RSML.
/// </summary>
/// <remarks>
/// > [!TIP]
/// > If you want to add content on top of a lexer, without overriding
/// > the extra functionality it adds, you might want to take a look at
/// > <see cref="Lexer"/>.
/// </remarks>
public interface ILexer : IToolchainComponent
{
	/// <summary>
	/// Tokenizes a source passed to the lexer.
	/// </summary>
	/// <returns>The tokens.</returns>
	IEnumerable<Token> Lex();
}
