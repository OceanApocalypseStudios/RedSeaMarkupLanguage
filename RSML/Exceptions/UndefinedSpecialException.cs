using System;


namespace RSML.Exceptions
{

	/// <summary>
	/// Exception that is thrown when a special action is used but is undefined.
	/// </summary>
	public class UndefinedSpecialException : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="UndefinedSpecialException" /> class.
		/// </summary>
		public UndefinedSpecialException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UndefinedSpecialException" /> class
		/// with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public UndefinedSpecialException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UndefinedSpecialException" /> class
		/// with a specified error message and a reference to the error that caused this
		/// exception.
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		/// <param name="innerException">The exception that's the cause of the current exception</param>
		public UndefinedSpecialException(string? message, Exception? innerException) : base(message, innerException) { }

	}

}
