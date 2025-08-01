namespace RSML.Parser
{

	/// <summary>
	/// The strictness level of the evaluation.
	/// </summary>
	public enum StrictnessLevel : byte
	{

		/// <summary>
		/// Basic strictness. Tries as hard as possible not to throw any
		/// exceptions or add anything to the error list.
		/// </summary>
		Basic = 25,

		/// <summary>
		/// Cancels the evaluation whenever an error appears, as minimal as it
		/// might be.
		/// </summary>
		Strict = 100

	}

}
