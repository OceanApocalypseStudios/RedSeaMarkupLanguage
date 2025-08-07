using System;

using RSML.Language;


namespace RSML.Trimming
{

	/// <summary>
	/// A trimmer that does not trim anything from the document.
	/// </summary>
	public class NoTrimmer : ITrimmer
	{

		/// <summary>
		/// Creates a new instance of a no-trimmer.
		/// </summary>
		public NoTrimmer() { }

		/// <summary>
		/// Returns the content as is.
		/// </summary>
		/// <param name="document">The document</param>
		/// <returns>The input document, untouched</returns>
		public string Trim(ReadOnlySpan<char> document) => document.ToString();

	}

}
