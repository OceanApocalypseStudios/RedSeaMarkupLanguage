using System.Collections.Generic;


namespace OceanApocalypseStudios.RSML.Toolchain.Compliance
{

	/// <summary>
	/// Compliance with the specification.
	/// </summary>
	public struct SpecificationCompliance
	{

		/// <summary>
		/// The level of compliance per RSML feature.
		/// </summary>
		public Dictionary<Feature, ComplianceLevel> CompliancePerFeature { get; } = [ ];

		/// <summary>
		/// The version of the RSML Language Specification a toolchain component implements.
		/// </summary>
		public string? SpecificationVersion { get; set; } = null;

		/// <summary>
		/// Creates a new instance of a <see cref="SpecificationCompliance" /> struct.
		/// </summary>
		/// <param name="specificationVersion">The version of the RSML Language Specification a toolchain component implements</param>
		public SpecificationCompliance(string? specificationVersion) { SpecificationVersion = specificationVersion; }

		/// <summary>
		/// Creates a new instance of a <see cref="SpecificationCompliance" /> struct with no support for any RSML feature.
		/// </summary>
		public static SpecificationCompliance CreateNoSupport() =>
			new(null)
			{

				CompliancePerFeature =
				{

					[Feature.LogicPaths] = ComplianceLevel.None,
					[Feature.SpecialActions] = ComplianceLevel.None,
					[Feature.Comments] = ComplianceLevel.None,
					[Feature.WhitespaceManagement] = ComplianceLevel.None,
					[Feature.OverloadConversion] = ComplianceLevel.None

				}

			};

		/// <summary>
		/// Creates a new instance of a <see cref="SpecificationCompliance" /> struct with full support for every single RSML
		/// feature.
		/// </summary>
		public static SpecificationCompliance CreateFull(string specificationVersion) =>
			new(specificationVersion)
			{

				CompliancePerFeature =
				{

					[Feature.LogicPaths] = ComplianceLevel.Full,
					[Feature.SpecialActions] = ComplianceLevel.Full,
					[Feature.Comments] = ComplianceLevel.Full,
					[Feature.WhitespaceManagement] = ComplianceLevel.Full,
					[Feature.OverloadConversion] = ComplianceLevel.Full

				}

			};

	}

}
