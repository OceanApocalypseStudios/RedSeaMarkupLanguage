namespace RSML.Toolchain.Compliance
{

	/// <summary>
	/// Level of compliance of a RSML toolchain component.
	/// </summary>
	public enum ComplianceLevel
	{

		/// <summary>
		/// Not implemented at all.
		/// </summary>
		None,

		/// <summary>
		/// Barely supported.
		/// </summary>
		Minimal,

		/// <summary>
		/// Partial support, but with big gaps.
		/// </summary>
		Limited,

		/// <summary>
		/// Mostly supports, but with certain quirks.
		/// </summary>
		Moderate,

		/// <summary>
		/// Solid support, covers most use cases.
		/// </summary>
		Sufficient,

		/// <summary>
		/// Robust and reliable support.
		/// </summary>
		Strong,

		/// <summary>
		/// Near-complete, minor edge cases.
		/// </summary>
		Advanced,

		/// <summary>
		/// Fully compliant.
		/// </summary>
		Full

	}

}
