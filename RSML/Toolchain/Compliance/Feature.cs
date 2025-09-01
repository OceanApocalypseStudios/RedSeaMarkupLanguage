namespace OceanApocalypseStudios.RSML.Toolchain.Compliance
{

	/// <summary>
	/// A RSML feature, as stated in the Official Language Specification.
	/// </summary>
	public enum Feature
	{

		/// <summary>
		/// Logic path handling.
		/// </summary>
		LogicPaths,

		/// <summary>
		/// Special action handling.
		/// </summary>
		SpecialActions,

		/// <summary>
		/// Comment handling.
		/// </summary>
		Comments,

		/// <summary>
		/// The management of whitespace in RSML.
		/// </summary>
		WhitespaceManagement,

		/// <summary>
		/// The conversion from a generic overload such as Operator + Value to a more specific one,
		/// such as Operator + System Name + Version Number + Architecture + Value.
		/// </summary>
		OverloadConversion

	}

}
