using System;
using System.CommandLine;
using System.CommandLine.Help;
using System.Linq;
using System.Threading.Tasks;

using Spectre.Console;


namespace RSML.CLI
{

	internal partial class Program
	{

		public const string LanguageVersion = "v2.0.0";

		private static async Task<int> Main(string[] args)
		{

			#region Global Options

			Option<bool> versionOpt = new("--version")
			{

				Description = "The RSML language version the CLI is made for",
				DefaultValueFactory = _ => false

			};

			Option<bool> specSupportOpt = new("--specification-support")
			{

				Description = "CLI support for the Language Specification of the current version",
				DefaultValueFactory = _ => false

			};

			specSupportOpt.Aliases.Add("--spec-support");
			specSupportOpt.Aliases.Add("-S");

			Option<bool> disableAnsiOpt = new("--disable-ansi")
			{

				Description = "Disables colored output and advanced rendering",
				DefaultValueFactory = _ => false

			};

			disableAnsiOpt.Aliases.Add("--no-colors");

			#endregion

			#region RootCommand Setup

			RootCommand rootCommand = new("Red Sea Markup Language CLI");

			var helpVersionOpt = rootCommand.Options.FirstOrDefault(o => o is HelpOption);

			if (helpVersionOpt is not null)
				helpVersionOpt.Action = new AsciiHelp((HelpAction)helpVersionOpt.Action!);

			var defaultVersionOpt =
				rootCommand.Options.FirstOrDefault(o => o is VersionOption || o.Name == "--version" || o.Aliases.Contains("--version"));

			if (defaultVersionOpt is not null)
				_ = rootCommand.Options.Remove(defaultVersionOpt);

			rootCommand.Options.Add(specSupportOpt);
			rootCommand.Options.Add(disableAnsiOpt);
			rootCommand.Options.Add(versionOpt);

			#endregion

			#region Machine Command

			Command machineCmd = new("machine", "Gets information about the current machine");

			var outputFormatMOpt = new Option<string>("--output-format")
			{

				Description = "The format to output as.",
				DefaultValueFactory = _ => "PlainText",

			}.AcceptOnlyFromAmong("PlainText", "JSON");
			outputFormatMOpt.Aliases.Add("--format");
			outputFormatMOpt.Aliases.Add("-o");

			machineCmd.Options.Add(outputFormatMOpt);
			machineCmd.Options.Add(disableAnsiOpt);

			machineCmd.SetAction(result =>
			{

				var format = result.GetValue(outputFormatMOpt);

				if (result.GetValue(disableAnsiOpt) || format == "JSON")
				{

					var x = LocalMachineInfo_NoPretty(
								format ?? "InvalidValue"
							);

					if (x is not null)
						Console.WriteLine(x);

					return x is not null ? 0 : 1;

				}

				if (format == "PlainText")
					LocalMachineInfo_Pretty(); // eh eh
				else
					return 1;

				return 0;

			});

			rootCommand.Add(machineCmd);

			#endregion

			#region Generate Command

			Command generateCmd = new("generate", "Generate \"compiled\" RSML for C#, F# or Visual Basic");

			var languageOpt = new Option<string>("--language")
			{

				Description = "The language to generate for.",
				DefaultValueFactory = _ => "C#",

			}.AcceptOnlyFromAmong("C#", "F#", "VB");
			languageOpt.Aliases.Add("--dotnet-lang");
			languageOpt.Aliases.Add("-L");

			Option<string> moduleNameOpt = new("--module-name")
			{

				Description = "The name of the static class (C#) or module (VB/F#) that will contain the generated code.",
				DefaultValueFactory = _ => "GeneratedRsml",

			};
			moduleNameOpt.Aliases.Add("--class-name");
			moduleNameOpt.Aliases.Add("-M");

			generateCmd.Options.Add(languageOpt);
			generateCmd.Options.Add(moduleNameOpt);
			generateCmd.Options.Add(disableAnsiOpt);

			generateCmd.SetAction(result =>
				{

					Console.WriteLine(CompileRsml_NoPretty(Console.In.ReadToEnd(), result.GetValue(languageOpt) ?? "InvalidValue", result.GetValue(moduleNameOpt) ?? "GeneratedRsml"));
					return 0;

				}
			);

			rootCommand.Add(generateCmd);

			#endregion

			rootCommand.SetAction(result =>
				{

					bool disableAnsi = result.GetValue(disableAnsiOpt);

					#region --version

					if (result.GetValue(versionOpt)) // --version is greedy
					{

						if (!disableAnsi)
						{

							AnsiConsole.Markup($"[red]Red[/] [cyan]Sea[/] [white]Markup Language[/] [yellow]{LanguageVersion}[/]");

							return 0;

						}

						Console.WriteLine($"Red Sea Markup Language {LanguageVersion}");

						return 0;

					}

					#endregion

					#region --specification-support

					if (result.GetValue(specSupportOpt))
					{

						if (!disableAnsi)
							return SpecificationSupport_NoPretty();

						Console.Write($"The {specSupportOpt.Name} option cannot be used alongside {disableAnsiOpt.Name}.");

						return 1;

					}

					#endregion

					#region Default Output

					if (disableAnsi)
						Console.WriteLine("Red Sea Markup Language CLI");
					else
						AnsiConsole.Markup("[red]Red[/] [cyan]Sea[/] [white]Markup Language[/] CLI");

					#endregion

					return 0;

				}
			);

			var result = rootCommand.Parse(args);
			int retCode = await result.InvokeAsync();

			return retCode;

		}

	}

}
