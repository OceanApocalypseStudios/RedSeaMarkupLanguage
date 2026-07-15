using System.Collections.Generic;

using OceanApocalypseStudios.RSML.Sdk;
using OceanApocalypseStudios.RSML.Sdk.Extensibility;
using OceanApocalypseStudios.RSML.Sdk.Extensibility.Hooks;


namespace OceanApocalypseStudios.RSML.Language.Lexing;

/// <summary>
/// The base class for implementations of RSML lexers and tokenizers.
/// </summary>
public abstract class Lexer : IToolchainComponent
{
	ToolchainConfiguration configuration;
	List<ILexerHook> hooks;

	/// <inheritdoc/>
	public bool IsMutable { get; protected set; } = true;

	/// <inheritdoc/>
	public virtual void Freeze() => IsMutable = false;

	/// <inheritdoc/>
	public virtual void Inject<TExtension>() where TExtension : ILanguageExtension, new()
	{
		if (typeof(TExtension) is not ILexerHook)
		{
			
		}
	}

	/// <inheritdoc/>
	public virtual void Inject(ILanguageExtension injectable) => throw new System.NotImplementedException();

	/// <inheritdoc/>
	public virtual void Inject(ToolchainConfiguration configuration)
	{

	}
}
