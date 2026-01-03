using System;
using System.Text;
using System.Text.Json;

using Spectre.Console;

using LocalMachine = OceanApocalypseStudios.RSML.Machine.LocalMachine;


namespace OceanApocalypseStudios.RSML.CLI.Helpers
{

	internal static class LocalMachineOutput
	{

		public static string AsJson(LocalMachine machine)
		{

			string systemVersion = machine.StringifiedSystemVersion ?? "null";

			return $$"""
					 {
					 	"system": {
					 		"name": {{Quote(machine.SystemName ?? "null")}},
					 		"version" : {{systemVersion}}
					 	},
					 	"linuxDistro": {
					 		"name": {{Quote(machine.DistroName ?? "null")}},
					 		"family": {{Quote(machine.DistroFamily ?? "null")}}
					 	},
					 	"processor": {
					 		"architecture": {{Quote(machine.ProcessorArchitecture ?? "null")}}
					 	}
					 }
					 """;

		}

		public static string AsPlainText(LocalMachine machine)
		{

			string systemVersion = machine.SystemName?.Equals("windows", StringComparison.OrdinalIgnoreCase) ?? false
									   ? machine.SystemVersion switch
									   {

										   6                  => "Vista",
										   7 or 8 or 10 or 11 => machine.StringifiedSystemVersion!,
										   9                  => "8.1",
										   _                  => "Unknown"

									   }
									   : machine.StringifiedSystemVersion ?? "Unknown";

			if (machine.SystemVersion == -1)
				systemVersion = "Unknown";

			return new StringBuilder()
				   .AppendLine($"System Name: {(machine.SystemName ?? "Unknown").Capitalize()}")
				   .AppendLine($"System Version: {systemVersion}")
				   .AppendLine()
				   .AppendLine($"Distro Name: {(machine.DistroName ?? "Unknown").Capitalize()}")
				   .AppendLine($"Distro Family: {(machine.DistroFamily ?? "Unknown").Capitalize()}")
				   .AppendLine()
				   .AppendLine($"Processor Architecture: {machine.ProcessorArchitecture ?? "Unknown"}")
				   .ToString();

		}

		public static void AsPrettyText(LocalMachine machine)
		{

			string systemVersion = machine.SystemName?.Equals("windows", StringComparison.OrdinalIgnoreCase) ?? false
									   ? machine.SystemVersion switch
									   {

										   6                  => "Vista",
										   7 or 8 or 10 or 11 => machine.StringifiedSystemVersion!,
										   9                  => "8.1",
										   _                  => "Unknown"

									   }
									   : machine.StringifiedSystemVersion ?? "Unknown";

			if (machine.SystemVersion == -1)
				systemVersion = "Unknown";

			AnsiConsole.Write(
				new Panel(
					new Rows(
						new Panel(
							new Columns(
								new Panel(
									new Rows(
										new Markup("[yellow]Operating System[/]"),
										new Markup($"[white]Name:[/] [grey]{(machine.SystemName ?? "Unknown").Capitalize()}[/]"),
										new Markup($"[white]Version:[/] [grey]{systemVersion}[/]")
									)
								).Expand(),
								new Panel(
									new Rows(
										new Markup(
											"[green]Linux Distribution[/] [grey](if applicable)[/]",
											machine.SystemName?.Equals("linux", StringComparison.OrdinalIgnoreCase) ?? false
												? null
												: new(null, null, Decoration.Strikethrough)
										),
										new Markup(
											$"[white]Family:[/] [grey]{(machine.DistroFamily ?? "Unknown").Capitalize()}[/]",
											machine.SystemName?.Equals("linux", StringComparison.OrdinalIgnoreCase) ?? false
												? null
												: new(null, null, Decoration.Strikethrough)
										),
										new Markup(
											$"[white]Name:[/] [grey]{(machine.DistroName ?? "Unknown").Capitalize()}[/]",
											machine.SystemName?.Equals("linux", StringComparison.OrdinalIgnoreCase) ?? false
												? null
												: new(null, null, Decoration.Strikethrough)
										)
									)
								).Expand()
							)
						).Expand(),
						new Panel(
							new Rows(
								new Markup("[cyan]Processor[/]"),
								new Markup($"[white]Architecture:[/] [grey]{machine.ProcessorArchitecture ?? "Unknown"}[/]")
							)
						).Expand()
					)
				).Expand()
			);

		}

		public static LocalMachine FromJson(string? json)
		{

			if (json is null)
				return new();

			using var document = JsonDocument.Parse(json);

			string? systemName = null!;
			int systemVersion = -1;

			string? distroName = null!;
			string? distroFamily = null!;

			string? processorArchitecture = null!;

			if (document.RootElement.TryGetProperty("system", out var system))
			{

				if (system.TryGetProperty("name", out var systemNameProperty))
					systemName = systemNameProperty.GetString();

				if (system.TryGetProperty("version", out var systemVersionProperty) && systemVersionProperty.TryGetInt32(out systemVersion)) { }

			}

			if (document.RootElement.TryGetProperty("linuxDistro", out var linuxDistro))
			{

				if (linuxDistro.TryGetProperty("name", out var distroNameProperty))
					distroName = distroNameProperty.GetString();

				if (linuxDistro.TryGetProperty("family", out var distroFamilyProperty))
					distroFamily = distroFamilyProperty.GetString();

			}

			if (document.RootElement.TryGetProperty("processor", out var processor))
			{

				if (processor.TryGetProperty("architecture", out var processorArchitectureProperty))
					processorArchitecture = processorArchitectureProperty.GetString();

			}

			if (systemName is not null && systemName.Equals("linux", StringComparison.OrdinalIgnoreCase))
				return new(distroName, distroFamily, processorArchitecture, systemVersion);

			return new(systemName, processorArchitecture, systemVersion);

		}

		private static string Quote(string? str) => str is null or "null" ? "null" : $"\"{str}\"";

	}

}
