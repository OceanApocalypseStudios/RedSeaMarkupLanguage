using OceanApocalypseStudios.RSML.Sdk.Extensibility;


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
	/// Injects an extension into the toolchain component, modifying it if
	/// it's mutable (see <see cref="IsMutable"/>). The item is of type <typeparamref name="TExtension"/>
	/// and is created via the default parameter-less constructor.
	/// </summary>
	/// <typeparam name="TExtension">The type of the extension.</typeparam>
	void Inject<TExtension>()
		where TExtension : ILanguageExtension, new();

	/// <summary>
	/// Injects an extension into the toolchain component, modifying it if
	/// it's mutable (see <see cref="IsMutable"/>).
	/// </summary>
	/// <param name="injectable">The item to inject.</param>
	void Inject(ILanguageExtension injectable);

	/// <summary>
	/// Injects a configuration into the toolchain component, modifying it
	/// if it's mutable (see <see cref="IsMutable"/>).
	/// </summary>
	/// <param name="configuration">The configuration to inject.</param>
	void Inject(ToolchainConfiguration configuration);
}
