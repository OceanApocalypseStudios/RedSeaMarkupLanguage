using System;


namespace OceanApocalypseStudios.RSML.Sources;

/// <summary>
/// Represents a read-only sequential scanner.
/// </summary>
public interface IReadOnlyScanner : IDisposable, IEquatable<IReadOnlyScanner?>
{
	// todo: planned for v3.0.0-prerelease2

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
}
