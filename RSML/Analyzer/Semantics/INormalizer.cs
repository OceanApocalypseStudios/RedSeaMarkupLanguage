using System.Collections.Generic;

using RSML.Analyzer.Syntax;
using RSML.Toolchain;


namespace RSML.Analyzer.Semantics
{

	/// <summary>
	/// A semantic normalizer for RSML.
	/// </summary>
	public interface INormalizer : IToolchainComponent
	{

		/// <summary>
		/// Normalizes tokens.
		/// </summary>
		/// <param name="tokens">The tokens to semantically normalize</param>
		/// <param name="length">The amount of tokens output</param>
		/// <returns>Normalized tokens</returns>
		public static abstract IEnumerable<SyntaxToken> NormalizeLine(IEnumerable<SyntaxToken> tokens, out int length);

	}

}
