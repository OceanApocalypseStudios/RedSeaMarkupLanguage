using System.Runtime.InteropServices;


namespace RSML.Parser
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
		/// Initializes a new set of evaluation properties.
		/// </summary>
		/// <param name="rid">The RID to check against, in the form of a string</param>
		public EvaluationProperties(string rid) { RuntimeIdentifier = rid; }

		/// <summary>
		/// Initializes a new set of evaluation properties.
		/// </summary>
		/// <param name="rid">The RID to check again, in the form of a string</param>
		/// <param name="expandAny">If set to <c>true</c>, expands <c>any</c> into <c>.+</c></param>
		public EvaluationProperties(string rid, bool expandAny)
		{

			RuntimeIdentifier = rid;
			ExpandAnyIntoRegularExpression = expandAny;

		}

		/// <summary>
		/// Creates a set of properties from the machine's .NET RID.
		/// </summary>
		/// <param name="expandAny">Whether to expand <c>any</c> into <c>.+</c></param>
		/// <returns></returns>
		public static EvaluationProperties FromMachineRid(bool expandAny) => new(RuntimeInformation.RuntimeIdentifier, expandAny);

	}

}
