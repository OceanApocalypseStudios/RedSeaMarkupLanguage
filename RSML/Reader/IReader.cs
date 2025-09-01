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
		/// The current index in the read buffer.
		/// Could be off by a few units or dozens, in certain scenarios.
		/// </summary>
		int CurrentBufferIndex { get; }

		/// <summary>
		/// Tries to tokenize the next line in the buffer.
		/// </summary>
		/// <param name="tokens">The output tokens</param>
		/// <returns><c>false</c> if the end of the buffer has been reached</returns>
		bool TryTokenizeNextLine(out IEnumerable<SyntaxToken> tokens);

	}

}
