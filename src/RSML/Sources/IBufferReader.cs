using System;


namespace OceanApocalypseStudios.RSML.Sources;

/// <summary>
/// Represents a sequential buffer reader whose data is composed of <see cref="Byte"/>s.
/// </summary>
public interface IBufferReader : ISource, IEquatable<IBufferReader?>
{
	// todo: planned for v3.0.0-prerelease2
}
