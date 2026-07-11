using System;


namespace OceanApocalypseStudios.RSML.Sources;

/// <summary>
/// A data source for RSML's toolchain members.
/// </summary>
public interface ISource : IToolchainComponent, IDisposable
{
	/// <summary>
	/// The current index of the cursor.
	/// </summary>
	int CursorIndex { get; }

	/// <summary>
	/// Whether the source is completely empty.
	/// </summary>
	bool IsEmpty { get; }

	/// <summary>
	/// Whether the source can be mutated.
	/// </summary>
	bool IsReadOnly { get; }

	/// <summary>
	/// The length of the source.
	/// </summary>
	int Length { get; }
}
