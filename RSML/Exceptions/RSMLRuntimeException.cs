using System;


namespace RSML.Exceptions
{

	/// <summary>
	/// Exception that is thrown by the RSML document.
	/// </summary>
	public class RSMLRuntimeException : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="RSMLRuntimeException" /> class.
		/// </summary>
		public RSMLRuntimeException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RSMLRuntimeException" /> class
		/// with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public RSMLRuntimeException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RSMLRuntimeException" /> class
		/// with a specified error message and a reference to the error that caused this
		/// exception.
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		/// <param name="innerException">The exception that's the cause of the current exception</param>
		public RSMLRuntimeException(string? message, Exception? innerException) : base(message, innerException) { }

	}

}
