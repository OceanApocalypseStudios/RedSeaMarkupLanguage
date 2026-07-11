namespace OceanApocalypseStudios.RSML.Sdk;

/// <summary>
/// Configuration options for a <see cref="PipeLine"/>, usually passed via a <see cref="PipeLineBuilder"/>.
/// </summary>
public enum PipeLineConfiguration
{
	/// <summary>
	/// Optimizes the toolchain pipeline by disabling extension processing.
	/// </summary>
	/// <remarks>
	/// > [!WARNING]
	/// > This completely disables extensions, but does not warn you if there are active extensions,
	/// > meaning sometimes you might be wondering why your extension is not working when, in reality,
	/// you've enabled this configuration.
	/// > [!IMPORTANT]
	/// > This configuration is automatically enabled when no extensions are activated, meaning that
	/// > if you pass this configuration manually when no extensions are active, you will cause RSML
	/// > to error out due to duplicated configuration, unless <see cref="SilentlyIgnoreDuplicatedConfigurations"/>
	/// > is enabled.
	/// </remarks>
	DisableExtensionProcessing,

	/// <summary>
	/// Only allows OceanApocalypseStudios extensions, leading to an error if any non-OAS extension is active.
	/// </summary>
	/// <remarks>
	/// > [!NOTE]
	/// > When used alongside <see cref="SilentlyIgnoreAllErrors"/>, the non-OAS extensions will be disabled, but
	/// > no errors will be thrown.
	/// </remarks>
	AllowOnlyOASExtensions,

	SilentlyIgnoreBrokenExtensions,
	SilentlyIgnoreDuplicatedExtensions,
	SilentlyIgnoreDuplicatedConfigurations,

	/// <summary>
	/// Ignores all errors thrown during pipeline creation and pipeline execution.
	/// </summary>
	/// <remarks>
	/// > [!CAUTION]
	/// > This option is only needed in beyond extremely rare occasions.
	/// > It emulates RSML v1.x.x behavior.
	/// </remarks>
	SilentlyIgnoreAllErrors
}

