using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Toolchain;


namespace OceanApocalypseStudios.RSML.Analyzer.Semantics
{

	/// <summary>
	/// A semantic normalizer for RSML.
	/// </summary>
	public interface INormalizer : IToolchainComponent
	{

		/// <summary>
		/// Normalizes a line of tokens.
		/// </summary>
		/// <param name="line">The line to normalize</param>
		/// <param name="tokenCount">The amount of tokens output</param>
		public static abstract void NormalizeLine(ref SyntaxLine line, out int tokenCount);

	}

}
