using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using System.CommandLine;

using RSML.Parser;
using RSML.Exceptions;


namespace RSML.CLI
{

	internal class Program
	{

		/// <summary>
		/// The color randomizer to use and discard.
		/// </summary>
		static readonly Random? randomizer = new();

		/// <summary>
		/// The CLI ASCII art title.
		/// </summary>
		const string ASCII_ART = @"    _      ______  _____ ___  ___ _         _____  _      _____       _    
 /\| |/\   | ___ \/  ___||  \/  || |       /  __ \| |    |_   _|   /\| |/\ 
 \ ` ' /   | |_/ /\ `--. | .  . || |       | /  \/| |      | |     \ ` ' / 
|_     _|  |    /  `--. \| |\/| || |       | |    | |      | |    |_     _|
 / , . \   | |\ \ /\__/ /| |  | || |____ _ | \__/\| |____ _| |_    / , . \ 
 \/|_|\/   \_| \_|\____/ \_|  |_/\_____/(_) \____/\_____/ \___/    \/|_|\/ 
                                                                           
																																";

		static async Task Main(string[] args)
		{

			RootCommand rootCommand = new("The CLI for the Red Sea Markup Language");

			if (!args.Contains("--no-pretty")) // don't pretty print
			{

				PrettyPrintAsciiArtAndCopyright();
				Console.WriteLine();

			}

			var noPrettyOption = new Option<bool>(
				aliases: ["--no-pretty"],
				description: "Don't output ASCII art, copyright messages or colored output",
				getDefaultValue: () => false
			);

			rootCommand.AddGlobalOption(noPrettyOption);

			//// VERSION COMMAND ////
			// not really a command - it kind of a trick
			Command versionCommand = new("--rsml-version", "Get the current version of RSML");

			versionCommand.SetHandler(void () =>
			{

				Assembly rsmlAssembly = typeof(RSDocument).Assembly; // this will get the assembly where RSDocument is, so fuck yeah
				try
				{

					Console.WriteLine($"You're currently running RSML v{(rsmlAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "UNKNOWN").Split("+")[0]}.\nGoodbye!");

				}
				catch (IndexOutOfRangeException)
				{

					Console.WriteLine("[ERROR] Could not get RSML's version.");
					Environment.Exit(2);

				}

				Environment.Exit(0);

			});

			versionCommand.AddAlias("-V");
			versionCommand.AddAlias("--formatted-version");
			versionCommand.AddAlias("-v");

			//// EVALUATE COMMAND ////
			Command evaluateCommand = new("evaluate", "Evaluate a Red Sea Markup Language document");

			var printOnlyPrimaryOption = new Option<bool>(
				aliases: ["--primary-only", "-p"],
				description: "Output only the primary results: useful if you need to pipe this result",
				getDefaultValue: () => false
			);
			var fallbackForNullOption = new Option<string?>(
				aliases: ["--antinull-fallback", "-f"],
				description: "Fallback for no-match scenarios",
				getDefaultValue: () => null
			);
			var fallbackForErrorOption = new Option<string?>(
				aliases: ["--antierror-fallback", "-F"],
				description: "Fallback for error scenarios",
				getDefaultValue: () => null
			);
			var customRidOption = new Option<string?>(
				aliases: ["--custom-rid", "-r", "--rid"],
				description: "Custom RID to check against",
				getDefaultValue: () => null
			);
			var expandAnyOption = new Option<bool>(
				aliases: ["--expand-any", "-x"],
				description: "Expands the any RID",
				getDefaultValue: () => false
			);

			evaluateCommand.AddOption(printOnlyPrimaryOption);
			evaluateCommand.AddOption(customRidOption);
			evaluateCommand.AddOption(expandAnyOption);
			evaluateCommand.AddOption(fallbackForErrorOption);
			evaluateCommand.AddOption(fallbackForNullOption);

			evaluateCommand.SetHandler(void (primaryOnly, nullFallback, errorFallback, customRid, expandAny) =>
			{

				string? currentInState = Console.In.ReadToEnd();

				if (currentInState is null)
				{

					Console.WriteLine((errorFallback ?? "") == "" ? "[ERROR] Could not parse piped input." : errorFallback!);
					Environment.Exit(1);

				}

				RSParser parser;

				if (primaryOnly)
				{

					parser = new(currentInState);
					parser.RegisterAction(OperatorType.Secondary, (_, _) => { });
					parser.RegisterAction(OperatorType.Tertiary, (_, _) => { });

				}
				else
				{

					parser = ReadyToGoParser.CreateNew(currentInState);

				}

				RSDocument document = new(parser);

				try
				{

					if (customRid is not null)
					{

						Console.WriteLine(
							(document.EvaluateDocument(customRid, expandAny)) ??
								((nullFallback is null)
									? "[WARNING] No match was found."
									: nullFallback));

					}
					else
					{

						Console.WriteLine(
							(document.EvaluateDocument(expandAny)) ??
								((nullFallback is null)
									? "[WARNING] No match was found."
									: nullFallback));

					}

				}
				catch (RSMLRuntimeException ex)
				{

					Console.WriteLine((errorFallback ?? "") == "" ? $"[ERROR] User-triggered error occured -> {ex.Message}" : errorFallback!);
					Environment.Exit(3);

				}

				Environment.Exit(0);

			}, printOnlyPrimaryOption, fallbackForNullOption, fallbackForErrorOption, customRidOption, expandAnyOption);

			evaluateCommand.AddAlias("eval");
			evaluateCommand.AddAlias("parse");

			//// REPOSITORY COMMAND ////
			Command repoCommand = new("repository", "Outputs the link to RSML's GitHub repository");

			repoCommand.SetHandler(void () =>
			{

				Console.WriteLine("Visit the repository at: https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage");
				Environment.Exit(0);

			});

			repoCommand.AddAlias("repo");
			repoCommand.AddAlias("github");
			repoCommand.AddAlias("about");
			repoCommand.AddAlias("online");

			//// GET-RID COMMAND ////
			Command ridCommand = new("get-rid", "Gets the Runtime Identifier for this system as specified by MSBuild");

			ridCommand.SetHandler(void () =>
			{

				Console.WriteLine($"The MSBuild RID for this system is: {RuntimeInformation.RuntimeIdentifier}.\nRemember RSML is compatible with Regex!");
				Environment.Exit(0);

			});

			ridCommand.AddAlias("get-runtime-id");
			ridCommand.AddAlias("get-runtime-identifier");
			ridCommand.AddAlias("rid-get");

			//// MFROAD-LIKE COMMAND ////
			Command mfroadCommand = new("emulate-mfroad", "Emulates a MFRoad-like syntax in RSML; not actually MFRoad - it's still RSML");

			mfroadCommand.SetHandler(void () =>
			{

				string? currentInState = Console.In.ReadToEnd();

				if (currentInState is null)
				{

					Console.WriteLine("[ERROR] Could not parse piped input.");
					Environment.Exit(1);

				}

				RSParser parser = ReadyToGoParser.CreateMFRoadLike(currentInState);
				RSDocument document = new(parser);

				try
				{

					Console.WriteLine(document.EvaluateDocument() ?? "[WARNING] No match was found.");

				}
				catch (RSMLRuntimeException ex)
				{

					Console.WriteLine($"[ERROR] User-triggered error occured -> {ex.Message}");
					Environment.Exit(3);

				}
				Environment.Exit(0);

			});

			mfroadCommand.AddAlias("mfroad-like");
			mfroadCommand.AddAlias("roadlike");

			//// ROOT COMMAND ////
			rootCommand.SetHandler((_) => { }, noPrettyOption);

			rootCommand.AddCommand(evaluateCommand);
			rootCommand.AddCommand(mfroadCommand);
			rootCommand.AddCommand(ridCommand);
			rootCommand.AddCommand(repoCommand);
			rootCommand.AddCommand(versionCommand);

#pragma warning disable IDE0058 // expression value is never used
			await rootCommand.InvokeAsync(args);
#pragma warning restore

		}

		/// <summary>
		/// Pretty prints the ASCII art and the copyright message.
		/// </summary>
		static void PrettyPrintAsciiArtAndCopyright()
		{

			Console.Write($"\u001b[38;5;{randomizer!.Next(18, 233)}m");
			Console.WriteLine(ASCII_ART);
			Console.WriteLine("Copyright(c) 2025 OceanApocalypseStudios");
			Console.Write("\u001b[0m");

		}

	}

}
