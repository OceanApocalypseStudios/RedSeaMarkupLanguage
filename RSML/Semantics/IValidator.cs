using System;
using System.Collections.Immutable;

using RSML.ComponentLayout;
using RSML.Tokenization;


namespace RSML.Semantics
{

	/// <summary>
	/// A semantics validator for RSML.
	/// </summary>
	public interface IValidator : IRsToolchainComponent
	{

		/// <summary>
		/// Valid version number comparators.
		/// </summary>
		ImmutableHashSet<string> ValidComparators { get; }

		/// <summary>
		/// Valid architecture identifiers. Case-insensitive.
		/// </summary>
		ImmutableHashSet<string> ValidArchitectures { get; }

		/// <summary>
		/// Valid system names. Case-insensitive.
		/// </summary>
		ImmutableHashSet<string> ValidSystems { get; }

		/// <summary>
		/// Validates a RSML buffer from its tokens.
		/// </summary>
		/// <param name="bufferTokens">The buffer's tokens</param>
		void ValidateBuffer(ReadOnlySpan<RsToken> bufferTokens);

		/// <summary>
		/// Validates a single line of RSML from its tokens.
		/// </summary>
		/// <param name="tokens">The line's tokens</param>
		void ValidateLine(ReadOnlySpan<RsToken> tokens);

	}

}
