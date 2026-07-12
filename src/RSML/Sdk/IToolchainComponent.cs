namespace OceanApocalypseStudios.RSML.Sdk;

/// <summary>
/// A component of the RSML toolchain.
/// </summary>
/// <seealso cref="Language.Lexing.Lexer"/>
/// <seealso cref="Language.Parsing.Parser"/>
/// <seealso cref="Execution.Interpreter"/>
public interface IToolchainComponent
{
	/// <summary>
	/// Whether the toolchain component is mutable. If not, it might mean
	/// the component has been compiled.
	/// </summary>
	bool IsMutable { get; }

	/// <summary>
	/// Freezes the toolchain component and doesn't allow further mutations.
	/// </summary>
	void Freeze();

	/// <summary>
	/// Injects an item into the toolchain component, modifying it if
	/// it's mutable (see <see cref="IsMutable"/>). The item is of type <typeparamref name="TInjectable"/>
	/// and is created via the default parameter-less constructor.
	/// </summary>
	/// <typeparam name="TInjectable">The type of the injectable item.</typeparam>
	void Inject<TInjectable>()
		where TInjectable : IInjectable, new();

	/// <summary>
	/// Injects an item into the toolchain component, modifying it if
	/// it's mutable (see <see cref="IsMutable"/>).
	/// </summary>
	/// <param name="injectable">The item to inject.</param>
	void Inject(IInjectable injectable);

	/// <summary>
	/// Injects a configuration into the toolchain component, modifying it
	/// if it's mutable (see <see cref="IsMutable"/>).
	/// </summary>
	/// <param name="configuration">The configuration to inject.</param>
	void Inject(ToolchainConfiguration configuration);
}
