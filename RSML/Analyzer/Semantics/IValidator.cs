using System;
using System.Collections.Immutable;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Toolchain;


namespace OceanApocalypseStudios.RSML.Analyzer.Semantics
{

	/// <summary>
	/// A semantics validator for RSML.
	/// </summary>
	public interface IValidator : IToolchainComponent
	{

		/// <summary>
		/// Valid version number comparators.
		/// </summary>
		static abstract ImmutableHashSet<string> ValidComparators { get; }

		/// <summary>
		/// Valid architecture identifiers. Case-insensitive.
		/// </summary>
		static abstract ImmutableHashSet<string> ValidArchitectures { get; }

		/// <summary>
		/// Valid system names. Case-insensitive.
		/// </summary>
		static abstract ImmutableHashSet<string> ValidSystems { get; }

		/// <summary>
		/// Valid special action names. Case-sensitive.
		/// </summary>
		static abstract ImmutableHashSet<string> ValidSpecialActionNames { get; }

		/// <summary>
		/// Validates a RSML buffer from its tokens.
		/// </summary>
		/// <param name="bufferTokens">The buffer's tokens</param>
		static abstract void ValidateBuffer(ReadOnlySpan<SyntaxToken> bufferTokens);

		/// <summary>
		/// Validates a single line of RSML from its tokens.
		/// </summary>
		/// <param name="tokens">The line's tokens</param>
		static abstract void ValidateLine(ReadOnlySpan<SyntaxToken> tokens);

	}

}
