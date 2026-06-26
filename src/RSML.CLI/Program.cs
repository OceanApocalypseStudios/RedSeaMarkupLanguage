using System;
using System.CommandLine;
using System.CommandLine.Help;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using OceanApocalypseStudios.RSML.CLI.Helpers;
using OceanApocalypseStudios.RSML.Host;

using Spectre.Console;


namespace OceanApocalypseStudios.RSML.CLI
{

	internal partial class Program
	{

		// todo: change this later
		public const string LanguageVersion = "v2.1.0-dev";

		public static string? cSharpLogo;

		public static string? fSharpLogo;

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

			Option<FileInfo?> filepathOpt = new("--filepath")
			{

				Description = "The file to load RSML from, instead of the stdin.",
				DefaultValueFactory = _ => null

			};

			filepathOpt.Aliases.Add("-f");

			#endregion

			#region RootCommand Setup

			RootCommand rootCommand = new("Red Sea Markup Language CLI");

			var helpVersionOpt = rootCommand.Options.FirstOrDefault(o => o is HelpOption);

			helpVersionOpt?.Action = new AsciiHelp((HelpAction)helpVersionOpt.Action!);

			var defaultVersionOpt = rootCommand.Options.FirstOrDefault(o => o is VersionOption || o.Name == "--version" || o.Aliases.Contains("--version"));

			if (defaultVersionOpt is not null)
				_ = rootCommand.Options.Remove(defaultVersionOpt);

			rootCommand.Options.Add(specSupportOpt);
			rootCommand.Options.Add(disableAnsiOpt);
			rootCommand.Options.Add(versionOpt);

			#endregion

			#region Host Command

			Command hostCmd = new("host", "Handles hosts");

			Command createHostCmd = new("create", "Creates a new host");
			Command getHostCmd = new("get", "Gets the current host");

			var hostOutputFormatOpt = new Option<string>("--output-format")
			{

				Description = "The format to output as.",
				DefaultValueFactory = _ => "PlainText"

			}.AcceptOnlyFromAmong(
				"PlainText",
				"JSON",
				"Dotnet",
				"CSharp"
			);

			hostOutputFormatOpt.Aliases.Add("--format");
			hostOutputFormatOpt.Aliases.Add("-o");

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

			createHostCmd.Options.Add(disableAnsiOpt);
			createHostCmd.Options.Add(systemNameOpt);
			createHostCmd.Options.Add(sysVersionOpt);
			createHostCmd.Options.Add(linuxNameOpt);
			createHostCmd.Options.Add(linuxFamilyOpt);
			createHostCmd.Options.Add(procArchOpt);
			createHostCmd.Options.Add(hostOutputFormatOpt);

			getHostCmd.Options.Add(hostOutputFormatOpt);
			getHostCmd.Options.Add(disableAnsiOpt);

			createHostCmd.SetAction(result =>
				{

					string? sysName = result.GetValue(systemNameOpt);
					string? distroName = sysName?.Equals("linux", StringComparison.OrdinalIgnoreCase) ?? false
											   ? result.GetValue(linuxNameOpt)
											   : null;
					string? distroFamily = sysName?.Equals("linux", StringComparison.OrdinalIgnoreCase) ?? false
											   ? result.GetValue(linuxFamilyOpt)
											   : null;

					string? processorArch = result.GetValue(procArchOpt);
					int sysVersion = result.GetValue(sysVersionOpt);

					return GetMachine(
						sysName?.Equals("linux", StringComparison.OrdinalIgnoreCase) ?? false
							? new(
								distroName,
								distroFamily,
								processorArch,
								sysVersion
							)
							: new(sysName, processorArch, sysVersion),
						result.GetValue(disableAnsiOpt),
						result.GetValue(hostOutputFormatOpt)
					);

				}
			);

			getHostCmd.SetAction(result => GetMachine(new(), result.GetValue(disableAnsiOpt), result.GetValue(hostOutputFormatOpt)));

			hostCmd.SetAction(_ =>
				{

					Console.WriteLine("Use one of host command's subcommands.");

					return 1;

				}
			);

			hostCmd.Add(createHostCmd);
			hostCmd.Add(getHostCmd);
			rootCommand.Add(hostCmd);

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

					string compilerOutput = CompileRsml_NoPretty(Console.In.ReadToEnd(), language ?? "InvalidValue", result.GetValue(moduleNameOpt) ?? "GeneratedRsml") ??
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

			#region Tokenize Command

			Command tokenizeCmd = new("tokenize", "Tokenizes a RSML document");

			tokenizeCmd.Options.Add(filepathOpt);

			tokenizeCmd.SetAction(result =>
				{

					string? filepath = result.GetValue(filepathOpt)?.FullName;

					string data = filepath is null
									  ? Console.In.ReadToEnd()
									  : File.ReadAllText(filepath);

					Console.WriteLine(Tokenize_NoPretty(data));

					return 0;

				}
			);

			rootCommand.Add(tokenizeCmd);

			#endregion

			#region Evaluate Command

			Command evaluateCmd = new("evaluate", "Evaluates a RSML document");

			Option<string?> hostOpt = new("--host")
			{

				Description = "The host, in JSON, to evaluate from.",
				DefaultValueFactory = _ => null

			};

			hostOpt.Aliases.Add("-m");

			evaluateCmd.Options.Add(hostOpt);
			evaluateCmd.Options.Add(filepathOpt);
			evaluateCmd.Options.Add(disableAnsiOpt);

			evaluateCmd.SetAction(result =>
				{

					bool disableAnsi = result.GetValue(disableAnsiOpt);
					string? filepath = result.GetValue(filepathOpt)?.FullName;

					string data = filepath is null
									  ? Console.In.ReadToEnd()
									  : File.ReadAllText(filepath);

					HostInfo hostInfo;

					try
					{

						hostInfo = HostOutput.FromJson(result.GetValue(hostOpt));

					}
					catch (Exception ex)
					{

						if (disableAnsi)
							Console.WriteLine($"JSON Error: {ex.Message}");
						else
							AnsiConsole.Markup($"[red]JSON Error on HostInfo load[/] {ex.Message}");

						return 2; // json error

					}

					if (disableAnsi)
						Evaluate_NoPretty(data, hostInfo);
					else
						Evaluate_Pretty(data, hostInfo);

					return 0;

				}
			);

			rootCommand.Add(evaluateCmd);

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
