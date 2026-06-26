using System;
using System.Text;

using OceanApocalypseStudios.RSML.Host;

using Spectre.Console;


namespace OceanApocalypseStudios.RSML.CLI.Helpers
{

	internal static class HostOutput
	{

		public static string AsDotnet(HostInfo hostInfo) =>
			hostInfo.IsLinux
				? $"new HostInfo(\"{hostInfo.SystemName}\", \"{hostInfo.ProcessorArchitecture}\", {hostInfo.SystemVersion})"
				: $"new HostInfo(\"{hostInfo.DistroName}\", \"{hostInfo.DistroFamily}\", \"{hostInfo.ProcessorArchitecture}\", {hostInfo.SystemVersion})";

		public static string AsJson(HostInfo hostInfo) => HostInfoConverter.ToJson(hostInfo);

		public static string AsPlainText(HostInfo hostInfo)
		{

			string systemVersion = hostInfo.SystemName?.Equals("windows", StringComparison.OrdinalIgnoreCase) ?? false
									   ? hostInfo.SystemVersion switch
									   {

										   6                  => "Vista",
										   7 or 8 or 10 or 11 => hostInfo.StringifiedSystemVersion!,
										   9                  => "8.1",
										   _                  => "Unknown"

									   }
									   : hostInfo.StringifiedSystemVersion ?? "Unknown";

			if (hostInfo.SystemVersion == -1)
				systemVersion = "Unknown";

			return new StringBuilder()
				   .AppendLine($"System Name: {(hostInfo.SystemName ?? "Unknown").Capitalize()}")
				   .AppendLine($"System Version: {systemVersion}")
				   .AppendLine()
				   .AppendLine($"Distro Name: {(hostInfo.DistroName ?? "Unknown").Capitalize()}")
				   .AppendLine($"Distro Family: {(hostInfo.DistroFamily ?? "Unknown").Capitalize()}")
				   .AppendLine()
				   .AppendLine($"Processor Architecture: {hostInfo.ProcessorArchitecture ?? "Unknown"}")
				   .ToString();

		}

		public static void AsPrettyText(HostInfo hostInfo)
		{

			string systemVersion = hostInfo.SystemName?.Equals("windows", StringComparison.OrdinalIgnoreCase) ?? false
									   ? hostInfo.SystemVersion switch
									   {

										   6                  => "Vista",
										   7 or 8 or 10 or 11 => hostInfo.StringifiedSystemVersion!,
										   9                  => "8.1",
										   _                  => "Unknown"

									   }
									   : hostInfo.StringifiedSystemVersion ?? "Unknown";

			if (hostInfo.SystemVersion == -1)
				systemVersion = "Unknown";

			AnsiConsole.Write(
				new Panel(
					new Rows(
						new Panel(
							new Columns(
								new Panel(
									new Rows(
										new Markup("[yellow]Operating System[/]"),
										new Markup($"[white]Name:[/] [grey]{(hostInfo.SystemName ?? "Unknown").Capitalize()}[/]"),
										new Markup($"[white]Version:[/] [grey]{systemVersion}[/]")
									)
								).Expand(),
								new Panel(
									new Rows(
										new Markup(
											"[green]Linux Distribution[/] [grey](if applicable)[/]",
											hostInfo.IsLinux
												? null
												: new(null, null, Decoration.Strikethrough)
										),
										new Markup(
											$"[white]Family:[/] [grey]{(hostInfo.DistroFamily ?? "Unknown").Capitalize()}[/]",
											hostInfo.IsLinux
												? null
												: new(null, null, Decoration.Strikethrough)
										),
										new Markup(
											$"[white]Name:[/] [grey]{(hostInfo.DistroName ?? "Unknown").Capitalize()}[/]",
											hostInfo.IsLinux
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
								new Markup($"[white]Architecture:[/] [grey]{hostInfo.ProcessorArchitecture ?? "Unknown"}[/]")
							)
						).Expand()
					)
				).Expand()
			);

		}

		public static HostInfo FromJson(string? json) => json is null ? new() : (HostInfoConverter.FromJson(json) ?? new());

	}

}
