using System;


namespace RSML.Exceptions
{

	/// <summary>
	/// Exception that is thrown when a secondary or tertiary action is used but is undefined.
	/// </summary>
	public class UndefinedActionException : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="UndefinedActionException" /> class.
		/// </summary>
		public UndefinedActionException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UndefinedActionException" /> class
		/// with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public UndefinedActionException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UndefinedActionException" /> class
		/// with a specified error message and a reference to the error that caused this
		/// exception.
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		/// <param name="innerException">The exception that's the cause of the current exception</param>
		public UndefinedActionException(string? message, Exception? innerException) : base(message, innerException) { }

	}

}
