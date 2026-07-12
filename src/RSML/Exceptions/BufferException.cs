using System;
using System.IO;


namespace OceanApocalypseStudios.RSML.Exceptions;

/// <summary>
/// An exception that is thrown when an error occurs inside a buffer operation.
/// </summary>
public class BufferException : IOException
{
	/// <summary>
	/// Initializes a new <see cref="BufferException"/> with no message.
	/// </summary>
	public BufferException() : base() { }

	/// <summary>
	/// Initializes a new <see cref="BufferException"/> with a <paramref name="message"/>.
	/// </summary>
	public BufferException(string message) : base(message) { }

	/// <summary>
	/// Initializes a new <see cref="BufferException"/> with a <paramref name="message"/> and a reference
	/// to the exception that caused this error (<paramref name="innerException"/>).
	/// </summary>
	public BufferException(string? message, Exception? innerException) : base(message, innerException) { }
}
