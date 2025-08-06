using System;


namespace RSML.Tokenization
{

	/// <summary>
	/// RSML tokenizer.
	/// </summary>
	public interface ILexer
	{

		/// <summary>
		/// Tokenizes a RSML line.
		/// </summary>
		/// <param name="line">The line to tokenize</param>
		/// <returns>An array of tokens</returns>
		public RsToken[] TokenizeLine(ReadOnlySpan<char> line);

	}

}
