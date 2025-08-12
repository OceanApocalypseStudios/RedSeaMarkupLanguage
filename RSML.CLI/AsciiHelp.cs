using System;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;

using Spectre.Console;


namespace RSML.CLI
{

	public class AsciiHelp(
		HelpAction defaultHelp
	) : SynchronousCommandLineAction
	{

		/// <inheritdoc />
		public override int Invoke(ParseResult parseResult)
		{

			if (!parseResult.GetValue<bool>("--disable-ansi"))
				AnsiConsole.Write(new FigletText(parseResult.RootCommandResult.Command.Description!));

			int result = defaultHelp.Invoke(parseResult);

			return result;

		}

	}

}
