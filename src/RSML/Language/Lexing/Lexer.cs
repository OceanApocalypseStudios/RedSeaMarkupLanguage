using OceanApocalypseStudios.RSML.Sdk;
using OceanApocalypseStudios.RSML.Sdk.Extensibility;


namespace OceanApocalypseStudios.RSML.Language.Lexing;

public abstract class Lexer : IToolchainComponent
{
	// todo: add necessary content to ILexer

	/// <inheritdoc/>
	public bool IsMutable { get; protected set; } = true;

	/// <inheritdoc/>
	public virtual void Freeze() => IsMutable = false;

	/// <inheritdoc/>
	public void Inject<TExtension>() where TExtension : ILanguageExtension, new() => throw new System.NotImplementedException();

	/// <inheritdoc/>
	public void Inject(ILanguageExtension injectable) => throw new System.NotImplementedException();

	/// <inheritdoc/>
	public virtual void Inject(ToolchainConfiguration configuration) => throw new System.NotImplementedException();
}
