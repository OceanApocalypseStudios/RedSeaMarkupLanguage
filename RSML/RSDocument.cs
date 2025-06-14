using System.IO;

using RSML.Parser;


namespace RSML
{

	/// <summary>
	/// Represents a Red Sea Markup Language document.
	/// </summary>
	public sealed partial class RSDocument
	{

		/// <summary>
		/// The parser that'll be used throughout this document.
		/// </summary>
		readonly RSParser parser;

		/// <summary>
		/// Creates a new document from a string containing
		/// RSML.
		/// </summary>
		/// <param name="rsml">The string</param>
		public RSDocument(string rsml)
		{

			parser = new(rsml);

		}

		/// <summary>
		/// Creates a new document from a StringReader
		/// containing RSML.
		/// </summary>
		/// <param name="reader">The StringReader</param>
		public RSDocument(StringReader reader)
		{

			parser = new(reader);

		}

		/// <summary>
		/// Creates a new document from a already initialized parser.
		/// </summary>
		/// <param name="parser">The parser</param>
		public RSDocument(RSParser parser)
		{

			this.parser = parser;

		}

		/// <summary>
		/// Creates a new document from a filepath.
		/// </summary>
		/// <param name="filepath">The path to the file</param>
		/// <returns>A RSDocument</returns>
		/// <exception cref="FileNotFoundException">The file could not be located</exception>
		public static RSDocument NewFromFile(string filepath)
		{

			return !File.Exists(filepath) ?
				throw new FileNotFoundException("Could not find such file.")
				: new(File.ReadAllText(filepath));

		}

		/// <summary>
		/// Parses and evaluates the document.
		/// </summary>
		/// <returns>Null if no priamry matches or a string matching the return value of the only primary match</returns>
		public string? EvaluateDocument() => parser.EvaluateRSML();

		/// <summary>
		/// Parses and evaluates the document.
		/// </summary>
		/// <param name="lineSeparation">The custom line separation character to use</param>
		/// <returns>Null if no priamry matches or a string matching the return value of the only primary match</returns>
		public string? EvaluateDocument(string lineSeparation) => parser.EvaluateRSML(lineSeparation);

		/// <summary>
		/// Parses and evaluates the document.
		/// </summary>
		/// <param name="expandAny">Whether to expand any or not</param>
		/// <param name="lineSeparation">The custom line separation character to use</param>
		/// <returns>Null if no priamry matches or a string matching the return value of the only primary match</returns>
		public string? EvaluateDocument(bool expandAny, string? lineSeparation = null) => parser.EvaluateRSML(expandAny, lineSeparation);

		/// <summary>
		/// Parses and evaluates the document.
		/// </summary>
		/// <param name="customRid">The custom RID to check against</param>
		/// <param name="lineSeparation">The custom line separation character to use</param>
		/// <returns>Null if no priamry matches or a string matching the return value of the only primary match</returns>
		public string? EvaluateDocument(string customRid, string? lineSeparation = null) => parser.EvaluateRSMLWithCustomRid(customRid, lineSeparation);

		/// <summary>
		/// Parses and evaluates the document.
		/// </summary>
		/// <param name="customRid">The custom RID to check against</param>
		/// <param name="expandAny">Whether to expand any or not</param>
		/// <param name="lineSeparation">The custom line separation character to use</param>
		/// <returns>Null if no priamry matches or a string matching the return value of the only primary match</returns>
		public string? EvaluateDocument(string customRid, bool expandAny, string? lineSeparation = null) => parser.EvaluateRSMLWithCustomRid(customRid, expandAny, lineSeparation);

		/// <summary>
		/// Returns this document's contents.
		/// </summary>
		/// <returns>The RSML data</returns>
		public override string ToString() => parser.ToString();

	}

}
