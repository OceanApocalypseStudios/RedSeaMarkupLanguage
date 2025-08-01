using System;


namespace RSML.Exceptions
{

	/// <summary>
	/// Exception that's thrown when an invalid RSML syntax is used.
	/// </summary>
	public class InvalidRSMLSyntax : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidRSMLSyntax" /> class.
		/// </summary>
		public InvalidRSMLSyntax() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidRSMLSyntax" />
		/// with a custom error message.
		/// </summary>
		/// <param name="message">The custom error message</param>
		public InvalidRSMLSyntax(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidRSMLSyntax" />
		/// with a custom error message and a reference to the exception
		/// that caused this error.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public InvalidRSMLSyntax(string? message, Exception? innerException) : base(message, innerException) { }

	}

}
