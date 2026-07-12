using System;


namespace OceanApocalypseStudios.RSML.Sdk.Exceptions;

/// <summary>
/// An exception caused by a mal-function of the <see cref="Sdk"/>.
/// </summary>
public class SdkException : Exception
{
	/// <summary>
	/// Creates an empty <see cref="SdkException"/>.
	/// </summary>
	public SdkException() : base() { }

	/// <summary>
	/// Creates a <see cref="SdkException"/> with a custom error message.
	/// </summary>
	/// <param name="message">The error message.</param>
	public SdkException(string message) : base(message) { }

	/// <summary>
	/// Creates a <see cref="SdkException"/> with a custom error message
	/// and a reference to the <see cref="Exception"/> that caused this issue.
	/// </summary>
	/// <param name="message">The error message.</param>
	/// <param name="innerException">The exception that caused the issue.</param>
	public SdkException(string? message, Exception innerException) : base(message, innerException) { }
}
