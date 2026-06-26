using OceanApocalypseStudios.RSML.Toolchain;


namespace OceanApocalypseStudios.RSML.Analyzer.Syntax
{

	/// <summary>
	/// A lexer for RSML that converts lines into collections of tokens.
	/// </summary>
	public interface ILexer : IToolchainComponent
	{

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
