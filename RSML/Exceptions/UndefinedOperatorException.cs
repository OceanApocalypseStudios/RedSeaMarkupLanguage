using System;


namespace RSML.Exceptions
{

	/// <summary>
	/// Exception that's thrown when a function that requires all 3 operators defined
	/// is called, without all 3 being defined.
	/// </summary>
	public class UndefinedOperatorException : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="UndefinedOperatorException" /> class.
		/// </summary>
		public UndefinedOperatorException() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UndefinedOperatorException" />
		/// with a custom error message.
		/// </summary>
		/// <param name="message">The custom error message</param>
		public UndefinedOperatorException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UndefinedOperatorException" />
		/// with a custom error message and a reference to the exception
		/// that caused this error.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public UndefinedOperatorException(string? message, Exception? innerException) : base(message, innerException) { }

	}

}
