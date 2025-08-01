namespace RSML.Parser
{

	/// <summary>
	/// The result of the evaluation of an RSML document.
	/// </summary>
	public readonly ref struct EvaluationResult
	{

		/// <summary>
		/// The match's value or <c>null</c> if none was found.
		/// </summary>
		public string? MatchValue { get; private init; } = null;

		/// <summary>
		/// Whether a match was found (<c>true</c>) or not.
		/// </summary>
		public bool WasMatchFound => MatchValue is not null;

		/// <summary>
		/// Initializes a new evaluation result with no found matches.
		/// </summary>
		public EvaluationResult() { }

		/// <summary>
		/// Initializes a new evaluation result with a found match.
		/// </summary>
		/// <param name="matchValue">The found match's value</param>
		public EvaluationResult(string matchValue)
		{

			MatchValue = matchValue;

		}

	}

}
