using System;

using RSML.Language;


namespace RSML.Trimming
{

	/// <summary>
	/// Base interface for all RSML trimmers.
	/// </summary>
	public interface ITrimmer
	{

		/// <summary>
		/// Given a language standard, trims all unnecessary content from a RSML document,
		/// in order to make it more lightweight.
		/// </summary>
		/// <param name="document">The document</param>
		/// <param name="languageStandard">The language standard</param>
		/// <returns>A trimmed document</returns>
		public string Trim(ReadOnlySpan<char> document, in LanguageStandard languageStandard);

	}

}
