using System.Collections.Generic;

using OceanApocalypseStudios.RSML.Sdk;
using OceanApocalypseStudios.RSML.Sdk.Extensibility;
using OceanApocalypseStudios.RSML.Sdk.Extensibility.Hooks;
using OceanApocalypseStudios.RSML.Sdk.Extensibility.Registries;


namespace OceanApocalypseStudios.RSML.Language.Lexing;

/// <summary>
/// The base class for implementations of RSML lexers and tokenizers.
/// </summary>
public abstract class Lexer : IToolchainComponent
{
	/// <inheritdoc/>
	public virtual ToolchainConfiguration Configuration { get; protected set; }
	readonly List<ILexerHook> hooks = [];
	readonly LexerRuleRegistry registry = new();

	/// <inheritdoc/>
	public bool IsMutable { get; protected set; } = true;

	/// <inheritdoc/>
	public virtual void Freeze() => IsMutable = false;

	/// <inheritdoc/>
	public virtual bool TryInject(ILanguageExtension injectable)
	{
		if (injectable is not ILexerHook hook || hooks.Contains(hook))
			return false;

		hook.Register(registry);
		hooks.Add(hook);
		return true;
	}

	/// <inheritdoc/>
	public virtual void Inject(ToolchainConfiguration configuration) => Configuration |= configuration;
}
