using System;


namespace OceanApocalypseStudios.RSML.Sources;

/// <summary>
/// A data source for RSML's toolchain members.
/// </summary>
public interface ISource : IDisposable
{
	/// <summary>
	/// The length of the source.
	/// </summary>
	int Length { get; }

	/// <summary>
	/// Whether the source is completely empty.
	/// </summary>
	bool IsEmpty { get; }

	/// <summary>
	/// Converts an index into a location.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The location.</returns>
	SourceLocation GetSourceLocation(int index);
}
