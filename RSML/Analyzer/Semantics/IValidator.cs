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
		/// Valid architecture identifiers. Case-insensitive.
		/// </summary>
		static abstract ImmutableArray<ReadOnlyMemory<char>> ValidArchitectures { get; }

		/// <summary>
		/// Valid version number comparators.
		/// </summary>
		static abstract ImmutableArray<ReadOnlyMemory<char>> ValidComparators { get; }

		/// <summary>
		/// Valid special action names. Case-sensitive.
		/// </summary>
		static abstract ImmutableArray<ReadOnlyMemory<char>> ValidSpecialActionNames { get; }

		/// <summary>
		/// Valid system names. Case-insensitive.
		/// </summary>
		static abstract ImmutableArray<ReadOnlyMemory<char>> ValidSystems { get; }

		/// <summary>
		/// Validates a single line of RSML from its tokens.
		/// </summary>
		/// <param name="line">A collection of tokens</param>
		/// <param name="context">The buffer</param>
		static abstract void ValidateLine(SyntaxLine line, DualTextBuffer context);

	}

}
