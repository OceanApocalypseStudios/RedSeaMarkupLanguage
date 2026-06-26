using System;
using System.ComponentModel;
using System.Text.Json;


namespace OceanApocalypseStudios.RSML.Host
{

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class HostInfoWire
	{

		public string? SystemName { get; set; }
		public int SystemVersion { get; set; }
		public string? DistroName { get; set; }
		public string? DistroFamily { get; set; }
		public string? ProcessorArchitecture { get; set; }

	}

	/// <summary>
	/// JSON &lt;-&gt; <see cref="HostInfo"/> converter that respects the
	/// <a href="https://github.com/OceanApocalypseStudios/schemas/blob/main/rsml_hostinfo_schema.json">official</a>
	/// schema.
	/// </summary>
	public static class HostInfoConverter
	{

		/// <summary>
		/// Converts JSON into <see cref="HostInfo"/>.
		/// </summary>
		/// <param name="document">The JSON document</param>
		/// <param name="options">Serialization options</param>
		/// <returns>The host's info</returns>
		public static HostInfo? FromJson(ReadOnlySpan<char> document, JsonSerializerOptions? options = null)
		{

			var wire = JsonSerializer.Deserialize<HostInfoWire>(document, options);

			if (wire is null)
				return null;

			if (wire.SystemName == "linux")
				return new(wire.DistroName, wire.DistroFamily, wire.ProcessorArchitecture, wire.SystemVersion);

			return new(wire.SystemName, wire.ProcessorArchitecture, wire.SystemVersion);

		}

		/// <inheritdoc cref="FromJson(ReadOnlySpan{Char}, JsonSerializerOptions?)"/>
		public static HostInfo? FromJson(string document, JsonSerializerOptions? options = null) => FromJson(document.AsSpan(), options);

		/// <inheritdoc cref="FromJson(ReadOnlySpan{Char}, JsonSerializerOptions?)"/>
		public static HostInfo? FromJson(JsonDocument document, JsonSerializerOptions? options = null)
		{

			var wire = JsonSerializer.Deserialize<HostInfoWire>(document, options);

			if (wire is null)
				return null;

			if (wire.SystemName == "linux")
				return new(wire.DistroName, wire.DistroFamily, wire.ProcessorArchitecture, wire.SystemVersion);

			return new(wire.SystemName, wire.ProcessorArchitecture, wire.SystemVersion);

		}

		/// <summary>
		/// Converts <see cref="HostInfo"/> into JSON.
		/// </summary>
		/// <param name="hostInfo">The host's info</param>
		/// <returns>The JSON string</returns>
		public static string ToJson(HostInfo hostInfo) =>
			$$"""
			{
				"SystemName": "{{hostInfo.SystemName ?? "null"}}",
				"SystemVersion": {{hostInfo.SystemVersion}},
				"DistroName": "{{hostInfo.DistroName ?? "null"}}",
				"DistroFamily": "{{hostInfo.DistroFamily ?? "null"}}",
				"ProcessorArchitecture": "{{hostInfo.ProcessorArchitecture ?? "null"}}"
			}
			""";

	}

}
