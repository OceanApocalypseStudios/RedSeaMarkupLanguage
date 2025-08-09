namespace RSML.ComponentLayout
{

	/// <summary>
	/// A component of the RSML toolchain. Can be custom-made or officially maintained.
	/// </summary>
	public interface IRsToolchainComponent
	{

		/// <summary>
		/// The officially released version of RSML this component of the RSML toolchain is
		/// standardized for.
		/// If only partially standardized or no version is accurate, use <c>null</c>.
		/// </summary>
		string? StandardizedVersion { get; }

	}

}
