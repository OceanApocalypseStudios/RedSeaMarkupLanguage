using OceanApocalypseStudios.RSML.Sdk;
using OceanApocalypseStudios.RSML.Sdk.Extensibility;


namespace OceanApocalypseStudios.RSML.Language.Parsing;

/// <summary>
/// The base type that deals with parsing tokens and turning them into an organized tree.
/// </summary>
public abstract class Parser : IToolchainComponent
{
	// todo: add necessary content to IParser

	/// <inheritdoc/>
	public bool IsMutable { get; protected set; } = true;

	/// <inheritdoc/>
	public virtual void Freeze() => IsMutable = false;

	/// <inheritdoc/>
	public void TryInject<TExtension>() where TExtension : ILanguageExtension, new() => throw new System.NotImplementedException();

	/// <inheritdoc/>
	public void TryInject(ILanguageExtension injectable) => throw new System.NotImplementedException();

	/// <inheritdoc/>
	public virtual void Inject(ToolchainConfiguration configuration) => throw new System.NotImplementedException();
}
