using System.Collections.Generic;

using RSML.Analyzer.Syntax;
using RSML.Toolchain;


namespace RSML.Reader
{

	/// <summary>
	/// A RSML reader that reads from a buffer and tokenizes the lines.
	/// </summary>
	public interface IReader : IToolchainComponent
	{

		/// <summary>
		/// Tries to tokenize the next line in the buffer.
		/// </summary>
		/// <param name="lexer">The lexer to use for such task</param>
		/// <param name="tokens">The output tokens</param>
		/// <returns><c>false</c> if the end of the buffer has been reached</returns>
		bool TryTokenizeNextLine(ILexer lexer, out IEnumerable<SyntaxToken> tokens);

	}

}
