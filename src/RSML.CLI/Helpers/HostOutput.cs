using System;
using System.Text;
using System.Text.Json;

using Spectre.Console;


namespace OceanApocalypseStudios.RSML.CLI.Helpers
{

	internal static class HostOutput
	{

		public static string AsDotnet(Host host) =>
			host.IsLinux
				? $"new Host(\"{host.SystemName}\", \"{host.ProcessorArchitecture}\", {host.SystemVersion})"
				: $"new Host(\"{host.DistroName}\", \"{host.DistroFamily}\", \"{host.ProcessorArchitecture}\", {host.SystemVersion})";

		public static string AsJson(Host host)
		{

			string systemVersion = host.StringifiedSystemVersion ?? "null";

			return $$"""
					 {
					 	"system": {
					 		"name": {{Quote(host.SystemName ?? "null")}},
					 		"version" : {{systemVersion}}
					 	},
					 	"linuxDistro": {
					 		"name": {{Quote(host.DistroName ?? "null")}},
					 		"family": {{Quote(host.DistroFamily ?? "null")}}
					 	},
					 	"processor": {
					 		"architecture": {{Quote(host.ProcessorArchitecture ?? "null")}}
					 	}
					 }
					 """;

		}

		public static string AsPlainText(Host host)
		{

			string systemVersion = host.SystemName?.Equals("windows", StringComparison.OrdinalIgnoreCase) ?? false
									   ? host.SystemVersion switch
									   {

										   6                  => "Vista",
										   7 or 8 or 10 or 11 => host.StringifiedSystemVersion!,
										   9                  => "8.1",
										   _                  => "Unknown"

									   }
									   : host.StringifiedSystemVersion ?? "Unknown";

			if (host.SystemVersion == -1)
				systemVersion = "Unknown";

			return new StringBuilder()
				   .AppendLine($"System Name: {(host.SystemName ?? "Unknown").Capitalize()}")
				   .AppendLine($"System Version: {systemVersion}")
				   .AppendLine()
				   .AppendLine($"Distro Name: {(host.DistroName ?? "Unknown").Capitalize()}")
				   .AppendLine($"Distro Family: {(host.DistroFamily ?? "Unknown").Capitalize()}")
				   .AppendLine()
				   .AppendLine($"Processor Architecture: {host.ProcessorArchitecture ?? "Unknown"}")
				   .ToString();

		}

		public static void AsPrettyText(Host host)
		{

			string systemVersion = host.SystemName?.Equals("windows", StringComparison.OrdinalIgnoreCase) ?? false
									   ? host.SystemVersion switch
									   {

										   6                  => "Vista",
										   7 or 8 or 10 or 11 => host.StringifiedSystemVersion!,
										   9                  => "8.1",
										   _                  => "Unknown"

									   }
									   : host.StringifiedSystemVersion ?? "Unknown";

			if (host.SystemVersion == -1)
				systemVersion = "Unknown";

			AnsiConsole.Write(
				new Panel(
					new Rows(
						new Panel(
							new Columns(
								new Panel(
									new Rows(
										new Markup("[yellow]Operating System[/]"),
										new Markup($"[white]Name:[/] [grey]{(host.SystemName ?? "Unknown").Capitalize()}[/]"),
										new Markup($"[white]Version:[/] [grey]{systemVersion}[/]")
									)
								).Expand(),
								new Panel(
									new Rows(
										new Markup(
											"[green]Linux Distribution[/] [grey](if applicable)[/]",
											host.IsLinux
												? null
												: new(null, null, Decoration.Strikethrough)
										),
										new Markup(
											$"[white]Family:[/] [grey]{(host.DistroFamily ?? "Unknown").Capitalize()}[/]",
											host.IsLinux
												? null
												: new(null, null, Decoration.Strikethrough)
										),
										new Markup(
											$"[white]Name:[/] [grey]{(host.DistroName ?? "Unknown").Capitalize()}[/]",
											host.IsLinux
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
								new Markup($"[white]Architecture:[/] [grey]{host.ProcessorArchitecture ?? "Unknown"}[/]")
							)
						).Expand()
					)
				).Expand()
			);

		}

		public static Host FromJson(string? json)
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
			{

				return new(
					distroName,
					distroFamily,
					processorArchitecture,
					systemVersion
				);

			}

			return new(systemName, processorArchitecture, systemVersion);

		}

		private static string Quote(string? str) =>
			str is null or "null"
				? "null"
				: $"\"{str}\"";

	}

}
