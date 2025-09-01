using System;


namespace OceanApocalypseStudios.RSML.Exceptions
{

	/// <summary>
	/// Exception that's thrown when an action that's undefined is used.
	/// </summary>
	public class UndefinedActionException : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="UndefinedActionException" /> class.
		/// </summary>
		public UndefinedActionException() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UndefinedActionException" />
		/// with a custom error message.
		/// </summary>
		/// <param name="message">The custom error message</param>
		public UndefinedActionException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UndefinedActionException" />
		/// with a custom error message and a reference to the exception
		/// that caused this error.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public UndefinedActionException(string? message, Exception? innerException) : base(message, innerException) { }

	}

}
