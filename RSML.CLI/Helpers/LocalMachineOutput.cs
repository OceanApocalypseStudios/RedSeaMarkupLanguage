using System;
using System.Text;
using System.Text.Json;

using Spectre.Console;

using LocalMachine = OceanApocalypseStudios.RSML.Machine.LocalMachine;


namespace OceanApocalypseStudios.RSML.CLI.Helpers
{

	internal static class LocalMachineOutput
	{

		private static string Quote(string? str) => str is null or "null" ? "null" : $"\"{str}\"";

		public static LocalMachine FromJson(string? json)
		{

			if (json is null)
				return new();

			using var document = JsonDocument.Parse(json);

			var system = document.RootElement.GetProperty("system");
			string? sysName = system.GetProperty("name").GetString();
			int sysVersion = system.GetProperty("version").GetInt32();

			var linuxDistro = document.RootElement.GetProperty("linuxDistro");
			string? distroName = linuxDistro.GetProperty("name").GetString();
			string? distroFamily = linuxDistro.GetProperty("family").GetString();

			string? procArch = document.RootElement.GetProperty("processor").GetProperty("architecture").GetString();

			if (sysName == "linux")
				return new(distroName, distroFamily, procArch, sysVersion);

			return new(sysName, procArch, sysVersion);

		}

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

	}

}
