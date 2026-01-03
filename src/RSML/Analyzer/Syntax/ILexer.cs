using OceanApocalypseStudios.RSML.Toolchain;


namespace OceanApocalypseStudios.RSML.Analyzer.Syntax
{

	/// <summary>
	/// A lexer for RSML that converts lines into collections of tokens.
	/// </summary>
	public interface ILexer : IToolchainComponent
	{

		/// <summary>
		/// Forms a RSML document from a collection of tokens.
		/// </summary>
		/// <param name="line">The tokens</param>
		/// <param name="context">The context for the tokens</param>
		/// <returns>A RSML document</returns>
		static abstract string CreateDocumentFromTokens(in SyntaxLine line, DualTextBuffer context);

		/// <summary>
		/// Tokenizes a RSML line.
		/// </summary>
		/// <param name="buffer">The line to tokenize, as a buffer</param>
		/// <returns>A collection of tokens</returns>
		static abstract SyntaxLine TokenizeLine(DualTextBuffer buffer);

		/// <summary>
		/// Tokenizes a component of a logic path.
		/// </summary>
		/// <param name="buffer">The buffer, where the next data is the component to tokenize</param>
		/// <returns>A single token or <c>null</c> if not recognized in the context of a logic path.</returns>
		static abstract SyntaxToken? TokenizeLogicPathComponent(DualTextBuffer buffer);

	}

}
