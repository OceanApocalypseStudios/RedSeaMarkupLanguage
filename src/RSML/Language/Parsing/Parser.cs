namespace OceanApocalypseStudios.RSML.Language.Parsing;

/// <summary>
/// The base type that deals with parsing tokens and turning them into an organized tree.
/// </summary>
public abstract class Parser : IParser
{
	/// <inheritdoc/>
	public ToolchainConfiguration Configuration { get; protected set; }

	/// <inheritdoc/>
	public virtual void Inject(ToolchainConfiguration configuration) => throw new System.NotImplementedException();
}
