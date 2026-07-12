using OceanApocalypseStudios.RSML.Sdk;


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
	public virtual void Inject<TInjectable>() where TInjectable : IInjectable, new() => throw new System.NotImplementedException();

	/// <inheritdoc/>
	public virtual void Inject(IInjectable injectable) => throw new System.NotImplementedException();

	/// <inheritdoc/>
	public virtual void Inject(ToolchainConfiguration configuration) => throw new System.NotImplementedException();
}
