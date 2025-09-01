using System;

using OceanApocalypseStudios.RSML.Actions;


namespace OceanApocalypseStudios.RSML.Exceptions
{

	/// <summary>
	/// Exception that's thrown when an action returns an error type that's not
	/// <see cref="SpecialActionBehavior.Error" />.
	/// </summary>
	public class ActionErrorException : UserRaisedException
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionErrorException" /> class.
		/// </summary>
		public ActionErrorException() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionErrorException" />
		/// with a custom error message.
		/// </summary>
		/// <param name="message">The custom error message</param>
		public ActionErrorException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionErrorException" />
		/// with a custom error message and a reference to the exception
		/// that caused this error.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public ActionErrorException(string? message, Exception? innerException) : base(message, innerException) { }

	}

}
