using System;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;

using Spectre.Console;


namespace RSML.CLI
{

	internal class Program
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

			RootCommand rootCommand = new("Red Sea Markup Language's official CLI");

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

					if (result.GetValue(specSupportOpt))
					{

						if (disableAnsi)
						{

							Console.Write($"The {specSupportOpt.Name} option cannot be used alongside {disableAnsiOpt.Name}.");

							return 1;

						}

						// for the nodes, use these characters: · (partial) ⨯ (nope) ✓ (yesssss)

						NonInteractibleTree tree = new($"[cyan]Language Specification Support[/] ({LanguageVersion})");

						var syntax = tree.AddNode("[green](✓)[/] Syntax");
						_ = syntax.AddNode("[green](✓)[/] Full-line Comments with #");

						var logicPaths = syntax.AddNode("[green](✓)[/] Logic Paths");

						var overloads = logicPaths.AddNode("[green](✓)[/] Overloads");
						_ = overloads.AddNode("[green](✓)[/] Operator + Value");
						_ = overloads.AddNode("[green](✓)[/] Operator + System Name + Value");
						_ = overloads.AddNode("[green](✓)[/] Operator + System Name + Processor Architecture + Value");
						_ = overloads.AddNode("[green](✓)[/] Operator + System Name + Major Version Number + Processor Architecture + Value");

						_ = overloads.AddNode(
							"[green](✓)[/] Operator + System Name + Comparator + Major Version Number + Processor Architecture + Value"
						);

						var operators = logicPaths.AddNode("[green](✓)[/] Operators");
						_ = operators.AddNode("[green](✓)[/] Return Operator (->)");
						_ = operators.AddNode("[green](✓)[/] Return Operator (!>)");

						var specialActions = syntax.AddNode("[green](✓)[/] Special Actions with @");
						_ = specialActions.AddNode("[green](✓)[/] Argument-less");
						_ = specialActions.AddNode("[green](✓)[/] Single argument");

						_ = tree.AddNode("[green](✓)[/] Character Set").AddNode("[green](✓)[/] UTF-16");

						AnsiConsole.Write(tree);

						return 0;

					}

					if (result.GetValue(versionOpt))
					{

						if (!disableAnsi)
						{

							AnsiConsole.Markup($"[red]Red[/] [cyan]Sea[/] [white]Markup Language[/] [yellow]{LanguageVersion}[/]");

							return 0;

						}

						Console.WriteLine($"Red Sea Markup Language {LanguageVersion}");

						return 0;

					}

					if (disableAnsi)
						Console.WriteLine("Red Sea Markup Language CLI");
					else
						AnsiConsole.Markup($"[red]Red[/] [cyan]Sea[/] [white]Markup Language[/] CLI");

					return 0;

				}
			);

			var result = rootCommand.Parse(args);
			int retCode = await result.InvokeAsync();

			return retCode;

		}

	}

}
