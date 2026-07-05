using System;


namespace OceanApocalypseStudios.RSML.Exceptions;

/// <summary>
/// Represents an error in the RSML toolchain.
/// </summary>
public interface IError : IEquatable<IError>, IFormattable;
