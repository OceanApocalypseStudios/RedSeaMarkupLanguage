using System;
using System.Collections.Generic;

using OceanApocalypseStudios.RSML.Language.Lexing.Tokens;
using OceanApocalypseStudios.RSML.Toolchain.Abstractions;
using OceanApocalypseStudios.RSML.Toolchain.Abstractions.Diagnostics;

namespace OceanApocalypseStudios.RSML.Language.Lexing;

/// <summary>
/// The base class for implementations of RSML lexers and tokenizers.
/// </summary>
public abstract class Lexer : ILexer
{
	private bool isDisposed;

	/// <inheritdoc/>
	public virtual ToolchainConfigurations Configuration { get; protected set; }

	/// <inheritdoc/>
	public virtual void Inject(ToolchainConfigurations configuration) => Configuration |= configuration;

	/// <inheritdoc/>
	public abstract IEnumerable<Token> Lex();

	/// <inheritdoc/>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Disposes of both managed and unmanaged resources.
	/// </summary>
	/// <param name="disposing">When set to <c>false</c>, disposes of unmanaged resources only.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (isDisposed)
			return;

		// dispose of managed stuff if disposing is true

		isDisposed = true;
	}

	/// <inheritdoc/>
	public abstract Result<Token> GetNextToken();
}
