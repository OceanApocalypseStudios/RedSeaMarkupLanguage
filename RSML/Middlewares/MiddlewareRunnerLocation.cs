namespace RSML.Middlewares
{

	/// <summary>
	/// The location at which to run the middleware.
	/// </summary>
	public enum MiddlewareRunnerLocation
	{

		/// <summary>
		/// Before the tokens being normalized.
		/// Use if you'd like to work with raw tokens.
		/// </summary>
		BeforeNormalization,

		/// <summary>
		/// Before the tokens being validated.
		/// Use if you'd like to work with normalized but potentially invalid tokens.
		/// </summary>
		BeforeValidation,

		/// <summary>
		/// Before the final EOL has been removed from the tokens.
		/// Use if you'd like to work with normalized and validated tokens with a EOL token in the end.
		/// </summary>
		BeforeEolRemoval,

		/// <summary>
		/// Before the length is checked.
		/// Same as AfterEolRemoval.
		/// </summary>
		BeforeLengthCheck,

		/// <summary>
		/// If the length is zero.
		/// </summary>
		ZeroLength,

		/// <summary>
		/// If the length is 2.
		/// </summary>
		TwoLength,

		/// <summary>
		/// Runs if the length is 3 AND before the special action is called.
		/// </summary>
		ThreeLengthBeforeSpecialCall,

		/// <summary>
		/// Runs if the length is 5 and before the logic path handling.
		/// </summary>
		FiveLengthBeforeHandling,

		/// <summary>
		/// Runs if the length is 5 and after the logic path handling.
		/// </summary>
		FiveLengthAfterHandling,

		/// <summary>
		/// Runs if the length is 6 and before the logic path handling.
		/// </summary>
		SixLengthBeforeHandling,

		/// <summary>
		/// Runs if the length is 6 and after the logic path handling.
		/// </summary>
		SixLengthAfterHandling,

		/// <summary>
		/// Runs if the length is not 0, 2, 3, 5 or 6 AND before the comment check.
		/// </summary>
		AnyLengthBeforeCommentCheck

	}

}
