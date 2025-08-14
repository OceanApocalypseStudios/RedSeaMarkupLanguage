using System.Text;
using Spectre.Console;
using RSML.Machine;


namespace RSML.CLI
{

	internal static class LocalMachineOutput
	{

		public static string AsJson(LocalMachine machine)
		{

			string systemVersion = machine.SystemVersion == -1 ? "null" : machine.SystemVersion.ToString();

			return $$"""
				{
					"system": {
						"name": {{machine.SystemName ?? "null"}},
						"version" : {{systemVersion}}
					},
					"linuxDistro": {
						"name": {{machine.DistroName ?? "null"}},
						"family": {{machine.DistroFamily ?? "null"}}
					},
					"processor": {
						"architecture": {{machine.ProcessorArchitecture ?? "null"}}
					}
				}
				""";

		}

		public static string AsPlainText(LocalMachine machine)
		{

			string systemVersion = machine.SystemName == "windows" ? machine.SystemVersion switch
			{

				6 => "Vista",
				7 or 8 or 10 or 11 => machine.SystemVersion.ToString(),
				9 => "8.1",
				_ => "Unknown"

			} : machine.SystemVersion.ToString();

			if (machine.SystemVersion == -1)
				systemVersion = "Unknown";

			return new StringBuilder()
				.AppendLine($"System Name: {machine.SystemName ?? "Unknown"}")
				.AppendLine($"System Version: {systemVersion}")
				.AppendLine()
				.AppendLine($"Distro Name: {machine.DistroName ?? "Unknown"}")
				.AppendLine($"Distro Family: {machine.DistroFamily ?? "Unknown"}")
				.AppendLine()
				.AppendLine($"Processor Architecture: {machine.ProcessorArchitecture ?? "Unknown"}")
				.ToString();

		}

		public static void AsPrettyText(LocalMachine machine)
		{

			string systemVersion = machine.SystemName == "windows" ? machine.SystemVersion switch
			{

				6 => "Vista",
				7 or 8 or 10 or 11 => machine.SystemVersion.ToString(),
				9 => "8.1",
				_ => "Unknown"

			} : machine.SystemVersion.ToString();

			if (machine.SystemVersion == -1)
				systemVersion = "Unknown";

			AnsiConsole.Write(
				new Panel(
					new Rows(
						new Panel(
							new Columns(
								new Panel(
									new Rows(
										new Markup(
											"[yellow]Operating System[/]"
										),
										new Markup(
											$"[white]Name:[/] [grey]{machine.SystemName ?? "Unknown"}[/]"
										),
										new Markup(
											$"[white]Version:[/] [grey]{systemVersion}[/]"
										)
									)
								).Expand(),
								new Panel(
									new Rows(
										new Markup(
											"[green]Linux Distribution[/] [grey](if applicable)[/]",
											machine.SystemName == "linux" ? null : new(null, null, Decoration.Strikethrough)
										),
										new Markup(
											$"[white]Family:[/] [grey]{machine.DistroFamily ?? "Unknown"}[/]",
											machine.SystemName == "linux" ? null : new(null, null, Decoration.Strikethrough)
										),
										new Markup(
											$"[white]Name:[/] [grey]{machine.DistroName ?? "Unknown"}[/]",
											machine.SystemName == "linux" ? null : new(null, null, Decoration.Strikethrough)
										)
									)
								).Expand()
							)
						).Expand(),
						new Panel(
							new Rows(
								new Markup(
									"[cyan]Processor[/]"
								),
								new Markup(
									$"[white]Architecture:[/] [grey]{machine.ProcessorArchitecture ?? "Unknown"}[/]"
								)
							)
						).Expand()
					)
				).Expand()
			);

		}

	}

}
