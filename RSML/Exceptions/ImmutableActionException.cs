using System;


namespace RSML.Exceptions
{

	/// <summary>
	/// Exception that is thrown when a user attempts to redefine the primary action,
	/// which is immutable.
	/// </summary>
	public class ImmutableActionException : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="ImmutableActionException" /> class.
		/// </summary>
		public ImmutableActionException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ImmutableActionException" /> class
		/// with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public ImmutableActionException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ImmutableActionException" /> class
		/// with a specified error message and a reference to the error that caused this
		/// exception.
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		/// <param name="innerException">The exception that's the cause of the current exception</param>
		public ImmutableActionException(string? message, Exception? innerException) : base(message, innerException) { }

	}

}
