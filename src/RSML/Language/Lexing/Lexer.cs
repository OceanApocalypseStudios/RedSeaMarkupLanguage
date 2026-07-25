using System;
using System.Collections.Generic;

namespace OceanApocalypseStudios.RSML.Language.Lexing;

/// <summary>
/// The base class for implementations of RSML lexers and tokenizers.
/// </summary>
public abstract class Lexer : ILexer
{
	/// <inheritdoc/>
	public virtual ToolchainConfiguration Configuration { get; protected set; }

	/// <inheritdoc/>
	public virtual void Inject(ToolchainConfiguration configuration) => Configuration |= configuration;

	/// <inheritdoc/>
	public abstract IEnumerable<Token> Lex();

	/// <inheritdoc/>
	public virtual void Dispose() => GC.SuppressFinalize(this);
}
