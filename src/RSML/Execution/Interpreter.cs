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
	public abstract void Dispose();

	/// <inheritdoc/>
	public void Freeze() => IsMutable = false;

	/// <inheritdoc/>
	public void Inject(ToolchainConfiguration configuration) => throw new System.NotImplementedException();
}
