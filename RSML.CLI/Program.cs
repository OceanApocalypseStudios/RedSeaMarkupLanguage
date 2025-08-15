using System;
using System.CommandLine;
using System.CommandLine.Help;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using RSML.CLI.Helpers;

using Spectre.Console;


namespace RSML.CLI
{

	internal partial class Program
	{

		public const string LanguageVersion = "v2.0.0";

		public static string? fSharpLogo;
		public static string? cSharpLogo;
		public static string? visualBasicLogo;

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

			Command machineCmd = new("machine", "Handles machines");

			Command createMachineCmd = new("create", "Creates a new machine");
			Command getMachineDataCmd = new("get", "Gets the current machine");

			var outputFormatMOpt = new Option<string>("--output-format")
			{

				Description = "The format to output as.",
				DefaultValueFactory = _ => "PlainText"

			}.AcceptOnlyFromAmong("PlainText", "JSON");

			outputFormatMOpt.Aliases.Add("--format");
			outputFormatMOpt.Aliases.Add("-o");

			Option<string?> systemNameOpt = new("--system-name")
			{

				Description = "The name of the operating system.",
				DefaultValueFactory = _ => null

			};

			systemNameOpt.Aliases.Add("-S");

			Option<string?> linuxNameOpt = new("--distro-name")
			{

				Description = "The name of the Linux distribution.",
				DefaultValueFactory = _ => null

			};

			linuxNameOpt.Aliases.Add("-D");

			Option<string?> linuxFamilyOpt = new("--distro-family")
			{

				Description = "The family of the Linux distribution.",
				DefaultValueFactory = _ => null

			};

			linuxFamilyOpt.Aliases.Add("-F");

			Option<string?> procArchOpt = new("--processor-architecture")
			{

				Description = "The architecture of the processor.",
				DefaultValueFactory = _ => null

			};

			procArchOpt.Aliases.Add("-P");

			Option<int> sysVersionOpt = new("--system-version")
			{

				Description = "The operating system's version.",
				DefaultValueFactory = _ => -1

			};

			sysVersionOpt.Aliases.Add("-V");

			createMachineCmd.Options.Add(disableAnsiOpt);
			createMachineCmd.Options.Add(systemNameOpt);
			createMachineCmd.Options.Add(sysVersionOpt);
			createMachineCmd.Options.Add(linuxNameOpt);
			createMachineCmd.Options.Add(linuxFamilyOpt);
			createMachineCmd.Options.Add(procArchOpt);
			createMachineCmd.Options.Add(outputFormatMOpt);

			getMachineDataCmd.Options.Add(outputFormatMOpt);
			getMachineDataCmd.Options.Add(disableAnsiOpt);

			createMachineCmd.SetAction(result =>
				{

					string? distroName = result.GetValue(linuxNameOpt);
					string? sysName = distroName is not null ? "linux" : result.GetValue(systemNameOpt);

					string? distroFamily = (sysName?.Equals("linux", StringComparison.OrdinalIgnoreCase) ?? false)
											   ? result.GetValue(linuxFamilyOpt)
											   : null;

					string? processorArch = result.GetValue(procArchOpt);
					int sysVersion = result.GetValue(sysVersionOpt);

					return GetMachine(
						result,
						(sysName?.Equals("linux", StringComparison.OrdinalIgnoreCase) ?? false)
							? new(distroName, distroFamily, processorArch, sysVersion)
							: new(sysName, processorArch, sysVersion),
						result.GetValue(disableAnsiOpt), result.GetValue(outputFormatMOpt)
					);

				}
			);

			getMachineDataCmd.SetAction(result => GetMachine(result, new(), result.GetValue(disableAnsiOpt), result.GetValue(outputFormatMOpt)));

			machineCmd.SetAction(_ =>
				{

					Console.WriteLine("Use one of machine command's subcommands.");

					return 1;

				}
			);

			machineCmd.Add(createMachineCmd);
			machineCmd.Add(getMachineDataCmd);
			rootCommand.Add(machineCmd);

			#endregion

			#region Generate Command

			Command generateCmd = new("generate", "Generate \"compiled\" RSML for C#, F# or Visual Basic");

			var languageOpt = new Option<string>("--language")
			{

				Description = "The language to generate for.",
				DefaultValueFactory = _ => "C#"

			}.AcceptOnlyFromAmong("C#", "F#", "VB");

			languageOpt.Aliases.Add("--dotnet-lang");
			languageOpt.Aliases.Add("-l");

			Option<string> moduleNameOpt = new("--module-name")
			{

				Description = "The name of the static class (C#) or module (VB/F#) that will contain the generated code.",
				DefaultValueFactory = _ => "GeneratedRsml"

			};

			moduleNameOpt.Aliases.Add("--class-name");
			moduleNameOpt.Aliases.Add("-M");

			generateCmd.Options.Add(languageOpt);
			generateCmd.Options.Add(moduleNameOpt);
			generateCmd.Options.Add(disableAnsiOpt);

			generateCmd.SetAction(result =>
				{

					bool disableAnsi = result.GetValue(disableAnsiOpt);
					string? language = result.GetValue(languageOpt);

					string compilerOutput = CompileRsml_NoPretty(
												Console.In.ReadToEnd(), language ?? "InvalidValue", result.GetValue(moduleNameOpt) ?? "GeneratedRsml"
											) ??
											"//Failed to generate compiled RSML!";

					if (language is not null && !disableAnsi)
					{

						int colWidth = (Console.BufferWidth - 8) / 2;

						switch (language)
						{

							case "C#":
								if (cSharpLogo is null)
								{
									using AsciiImage img = new(Path.Join(AppContext.BaseDirectory, "InternalAssets", "csharp-logo.png"));

									cSharpLogo = img.GetRenderable(60, 50);

								}

								var grid1 = new Grid()
											.AddColumns(new GridColumn().Width(colWidth), new GridColumn().Width(colWidth))
											.AddRow(
												new Markup(cSharpLogo).Centered(),
												new Text(compilerOutput)
											)
											.Expand();

								AnsiConsole.Write(grid1);

								return 0;

							case "F#":
								if (fSharpLogo is null)
								{

									using AsciiImage img = new(Path.Join(AppContext.BaseDirectory, "InternalAssets", "fsharp-logo.png"));

									fSharpLogo = img.GetRenderable(60, 50);

								}

								var grid2 = new Grid()
											.AddColumns(new GridColumn().Width(colWidth), new GridColumn().Width(colWidth))
											.AddRow(
												new Markup(fSharpLogo).Centered(),
												new Text(compilerOutput)
											)
											.Expand();

								AnsiConsole.Write(grid2);

								return 0;

							case "VB":
								if (visualBasicLogo is null)
								{

									using AsciiImage img = new(Path.Join(AppContext.BaseDirectory, "InternalAssets", "vbnet-logo.png"));

									visualBasicLogo = img.GetRenderable(60, 50);

								}

								var grid3 = new Grid()
											.AddColumns(new GridColumn().Width(colWidth), new GridColumn().Width(colWidth))
											.AddRow(
												new Markup(visualBasicLogo).Centered(),
												new Text(compilerOutput)
											)
											.Expand();

								AnsiConsole.Write(grid3);

								return 0;

						}

					}

					Console.WriteLine(compilerOutput);

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
