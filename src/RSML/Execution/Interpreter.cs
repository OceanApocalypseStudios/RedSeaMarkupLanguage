using OceanApocalypseStudios.RSML.Sdk;
using OceanApocalypseStudios.RSML.Sdk.Extensibility;


namespace OceanApocalypseStudios.RSML.Execution;

/// <summary>
/// The base type that deals with evaluating, interpreting and executing RSML.
/// </summary>
public abstract class Interpreter : IToolchainComponent
{
	// todo: add necessary content to IInterpreter

	/// <inheritdoc/>
	public bool IsMutable { get; protected set; } = true;

	/// <inheritdoc/>
	public ToolchainConfiguration Configuration { get; protected set; }

	/// <inheritdoc/>
	public void Freeze() => IsMutable = false;

	/// <inheritdoc/>
	public void TryInject<TExtension>() where TExtension : ILanguageExtension, new() => throw new System.NotImplementedException();

	/// <inheritdoc/>
	public void TryInject(ILanguageExtension extension) => throw new System.NotImplementedException();

	/// <inheritdoc/>
	public void Inject(ToolchainConfiguration configuration) => throw new System.NotImplementedException();
	bool IToolchainComponent.TryInject(ILanguageExtension injectable) => throw new System.NotImplementedException();
}
