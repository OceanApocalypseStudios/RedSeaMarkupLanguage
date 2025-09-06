using OceanApocalypseStudios.RSML.Toolchain.Compliance;


namespace OceanApocalypseStudios.RSML.Toolchain
{

	/// <summary>
	/// A component of the RSML toolchain. Can be custom-made or officially maintained.
	/// </summary>
	public interface IToolchainComponent
	{

		/// <summary>
		/// The level of compliance, per feature, this toolchain component has.
		/// </summary>
		public static abstract SpecificationCompliance SpecificationCompliance { get; }

	}

}
