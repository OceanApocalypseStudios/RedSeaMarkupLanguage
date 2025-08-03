using System;

using RSML.Language;


namespace RSML.Tokenization
{

	/// <summary>
	/// RSML tokenizer.
	/// </summary>
	public interface ITokenizer
	{

		/// <summary>
		/// Tokenizes a RSML line.
		/// </summary>
		/// <param name="line">The line to tokenize</param>
		/// <param name="languageStandard">The language standard</param>
		/// <returns>A span of tokens</returns>
		public ReadOnlySpan<RsToken> TokenizeLine(ReadOnlySpan<char> line, in LanguageStandard languageStandard);

	}

}
