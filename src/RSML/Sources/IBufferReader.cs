using System;


namespace OceanApocalypseStudios.RSML.Sources;

/// <summary>
/// Represents a sequential buffer reader whose data is composed of <see cref="Byte"/>s.
/// </summary>
/// <remarks>
/// > [!NOTE]
/// > Not implemented in v3.0.0-prerelease1.
/// </remarks>
public interface IBufferReader : ISource, IEquatable<IBufferReader?>
{
	// todo: planned for v3.0.0-prerelease2
}
