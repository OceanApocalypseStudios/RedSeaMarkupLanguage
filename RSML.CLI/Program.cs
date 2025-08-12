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
