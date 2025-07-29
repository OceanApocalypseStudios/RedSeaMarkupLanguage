namespace RSML.Core.Parser
{

	/// <summary>
	/// Properties for evaluation.
	/// </summary>
	public readonly struct EvaluationProperties
	{

		/// <summary>
		/// The Runtime Identifier to use.
		/// </summary>
		public string RuntimeIdentifier { get; init; }

		/// <summary>
		/// If set to <c>true</c>, expands <c>any</c> into <c>.+</c>.
		/// </summary>
		public bool ExpandAnyIntoRegularExpression { get; init; } = false;

		/// <summary>
		/// How strict evaluation should be.
		/// </summary>
		public StrictnessLevel StrictnessLevel { get; init; } = StrictnessLevel.Basic;

		/// <summary>
		/// Initializes a new set of evaluation properties.
		/// </summary>
		/// <param name="rid">The RID to check against, in the form of a string</param>
		public EvaluationProperties(string rid)
		{

			RuntimeIdentifier = rid;

		}

		/// <summary>
		/// Initializes a new set of evaluation properties.
		/// </summary>
		/// <param name="rid">The RID to check again, in the form of a string</param>
		/// <param name="expandAny">If set to <c>true</c>, expands <c>any</c> into <c>.+</c></param>
		/// <param name="strictness">How strict the evaluation process should be</param>
		public EvaluationProperties(string rid, bool expandAny, StrictnessLevel strictness)
		{

			RuntimeIdentifier = rid;
			ExpandAnyIntoRegularExpression = expandAny;
			StrictnessLevel = strictness;

		}

	}

}
