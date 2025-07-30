using System;


namespace RSML.Core.Exceptions
{

	/// <summary>
	/// Exception that's thrown when an action returns an error type of
	/// <see cref="Actions.SpecialActionBehavior.Error" />.
	/// </summary>
	public class ActionStandardErrorException : ActionErrorException
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionStandardErrorException" /> class.
		/// </summary>
		public ActionStandardErrorException() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionStandardErrorException" />
		/// with a custom error message.
		/// </summary>
		/// <param name="message">The custom error message</param>
		public ActionStandardErrorException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionStandardErrorException" />
		/// with a custom error message and a reference to the exception
		/// that caused this error.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public ActionStandardErrorException(string? message, Exception? innerException) : base(message, innerException) { }

	}

}
