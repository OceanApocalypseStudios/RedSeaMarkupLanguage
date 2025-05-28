using System.IO;


namespace RSML
{

	public sealed partial class RSDocument
	{

		/// <summary>
		/// Saves RSML into a file.
		/// </summary>
		/// <param name="filepath">The path to the file</param>
		/// <param name="rsmlContents">The data to save</param>
		public static void SaveRSMLToFile(string filepath, string rsmlContents) => File.WriteAllText(filepath, rsmlContents);

		/// <summary>
		/// Loads a RSML file into a string.
		/// </summary>
		/// <param name="filepath">The path to the file</param>
		/// <returns>The RSML inside the file</returns>
		public static string LoadRSMLFromFile(string filepath) => File.ReadAllText(filepath);

		/// <summary>
		/// Loads a RSML file into a document.
		/// </summary>
		/// <param name="filepath">The path to the file</param>
		/// <returns>The document containing ready-to-parse RSML</returns>
		public static RSDocument LoadRSMLFromFileIntoDocument(string filepath) => new(File.ReadAllText(filepath));

	}

}
