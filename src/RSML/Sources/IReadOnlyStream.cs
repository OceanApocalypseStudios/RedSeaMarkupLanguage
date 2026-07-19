using System;


namespace OceanApocalypseStudios.RSML.Sources;

/// <summary>
/// Represents a read-only sequential stream.
/// </summary>
public interface IReadOnlyStream : ISource, IEquatable<IReadOnlyStream?>
{
	// todo: planned for v3.0.0-prerelease2

	/// <summary>
	/// The current index of the cursor.
	/// </summary>
	int CursorIndex { get; }
}
