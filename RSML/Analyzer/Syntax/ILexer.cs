using System;
using System.Collections.Generic;

using RSML.Toolchain;


namespace RSML.Analyzer.Syntax
{

	/// <summary>
	/// A lexer for RSML that converts lines into collections of tokens.
	/// </summary>
	public interface ILexer : IToolchainComponent
	{

		/// <summary>
		/// Tokenizes a RSML line.
		/// </summary>
		/// <param name="line">The line to tokenize</param>
		/// <returns>An enumerable collection of tokens</returns>
		IEnumerable<SyntaxToken> TokenizeLine(string line);

		/// <summary>
		/// Forms a RSML document from an enumerable collection of tokens.
		/// </summary>
		/// <param name="tokens">The tokens</param>
		/// <returns>A RSML document</returns>
		string CreateDocumentFromTokens(IEnumerable<SyntaxToken> tokens);

		/// <summary>
		/// Tokenizes a component of a logic path.
		/// </summary>
		/// <param name="component">The component to tokenize</param>
		/// <param name="pos">The position at which the tokenization is being done</param>
		/// <returns>A single token or <c>null</c> if not recognized in the context of a logic path.</returns>
		SyntaxToken? TokenizeLogicPathComponent(ReadOnlySpan<char> component, ref int pos);

	}

}
