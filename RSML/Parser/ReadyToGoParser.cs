using System;
using System.IO;

using RSML.Exceptions;


namespace RSML.Parser
{

	/// <summary>
	/// Ready-to-go parsers follow the RSML standards and are recommended for begginers
	/// and for better compatibility.
	/// </summary>
	public static class ReadyToGoParser
	{

		/// <summary>
		/// Creates a new ready-to-go RSML parser following its standards.
		/// </summary>
		/// <param name="rsmlContent">The RSML document, as a string</param>
		/// <returns>The RSML parser</returns>
		public static RSParser CreateNew(string rsmlContent)
		{

			RSParser parser = new(rsmlContent);

			// actions
			parser.RegisterAction(OperatorType.Secondary, (_, argument) => Console.WriteLine(argument));
			parser.RegisterAction(OperatorType.Tertiary, (_, argument) => throw new RSMLRuntimeException(argument));

			return parser;

		}

		/// <summary>
		/// Creates a new ready-to-go parser from a file.
		/// </summary>
		/// <param name="filepath">The path to the file</param>
		/// <returns>A RSML parser</returns>
		public static RSParser? CreateNewFromFilepath(string filepath)
		{

			if (!File.Exists(filepath)) return null;

			RSParser parser = new(File.ReadAllText(filepath));

			// actions
			parser.RegisterAction(OperatorType.Secondary, (_, argument) => Console.WriteLine(argument));
			parser.RegisterAction(OperatorType.Tertiary, (_, argument) => throw new RSMLRuntimeException(argument));

			return parser;

		}

		/// <summary>
		/// Creates a new ready-to-go RSML MFRoad-like parser following its standards.
		/// Note that this parser does not accurately represent MFRoad - it's simply
		/// RSML with MFRoad operators and actions.
		/// </summary>
		/// <param name="rsmlContent">The RSML document, as a string</param>
		/// <returns>The RSML parser</returns>
		public static RSParser CreateMFRoadLike(string rsmlContent)
		{

			RSParser parser = new(rsmlContent);

			// actions
			parser.RegisterAction(OperatorType.Secondary, (_, argument) => Console.WriteLine(argument));
			parser.RegisterAction(OperatorType.Tertiary, (_, argument) => throw new RSMLRuntimeException(argument));

			// operators
			parser.DefineOperator(OperatorType.Primary, "???"); // primary
			parser.DefineOperator(OperatorType.Secondary, "<<"); // outputs
			parser.DefineOperator(OperatorType.Tertiary, "!!!"); // throws error

			return parser;

		}

	}

}
