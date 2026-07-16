namespace OceanApocalypseStudios.RSML.Language.Lexing;

/// <summary>
/// An implementation of a lexer as a by-ref struct, ensuring allocations are reduced to a minimum.
/// </summary>
public ref struct StatelessLexer : ILexer
{
	/// <inheritdoc/>
	public ToolchainConfiguration Configuration { get; private set; }

	/// <inheritdoc/>
	public void Inject(ToolchainConfiguration configuration) => Configuration |= configuration;
}
