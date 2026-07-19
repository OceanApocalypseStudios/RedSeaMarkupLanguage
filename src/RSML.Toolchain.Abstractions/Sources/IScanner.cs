using System;


namespace OceanApocalypse.RSML.Toolchain.Abstractions.Sources;

/// <summary>
/// Represents a sequential scanner.
/// </summary>
public interface IScanner : IDisposable, IEquatable<IScanner?>
{
	// todo: planned for v3.0.0-prerelease2

	/// <summary>
	/// Whether the source is completely empty.
	/// </summary>
	bool IsEmpty { get; }

	/// <summary>
	/// Whether the source can be mutated.
	/// </summary>
	bool IsReadOnly { get; }
}
