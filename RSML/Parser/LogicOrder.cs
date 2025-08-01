namespace RSML.Parser
{

	/// <summary>
	/// The logic order for evaluation.
	/// </summary>
	public enum LogicOrder
	{

		/// <summary>
		/// Starts evaluating from the very start.
		/// </summary>
		ReadAllFromStart,

		/// <summary>
		/// Starts evaluating from the second line.
		/// </summary>
		SkipFirstFromStart,

		/// <summary>
		/// Starts evaluating from one line above the end and continues until the start.
		/// </summary>
		SkipFirstFromEnd,

		/// <summary>
		/// Starts evaluating from the very end and continues until the start.
		/// </summary>
		ReadAllFromEnd

	}

}
