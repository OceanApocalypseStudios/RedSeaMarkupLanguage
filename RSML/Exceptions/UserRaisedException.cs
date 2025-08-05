using System;


namespace RSML.Exceptions
{

	/// <summary>
	/// Exception that's thrown when the user uses an "exception-raising" action
	/// in RSML.
	/// </summary>
	public class UserRaisedException : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="UserRaisedException" /> class.
		/// </summary>
		protected UserRaisedException() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UserRaisedException" />
		/// with a custom error message.
		/// </summary>
		/// <param name="message">The custom error message</param>
		protected UserRaisedException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UserRaisedException" />
		/// with a custom error message and a reference to the exception
		/// that caused this error.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		protected UserRaisedException(string? message, Exception? innerException) : base(message, innerException) { }

	}

}
