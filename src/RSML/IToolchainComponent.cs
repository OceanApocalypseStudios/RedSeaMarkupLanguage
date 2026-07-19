using System;

namespace OceanApocalypseStudios.RSML;

/// <summary>
/// A component of the RSML toolchain.
/// </summary>
/// <seealso cref="Language.Lexing.Lexer"/>
/// <seealso cref="Language.Parsing.Parser"/>
/// <seealso cref="Execution.Interpreter"/>
public interface IToolchainComponent : IDisposable
{
	/// <summary>
	/// Configurations for the toolchain component.
	/// </summary>
	ToolchainConfiguration Configuration { get; }

	/// <summary>
	/// Injects a configuration into the toolchain component, modifying it.
	/// </summary>
	/// <param name="configuration">The configuration to inject.</param>
	void Inject(ToolchainConfiguration configuration);
}
