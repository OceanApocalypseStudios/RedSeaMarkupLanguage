namespace OceanApocalypseStudios.RSML.Sdk;

/// <summary>
/// Configuration options for a <see cref="ToolchainExecutionPlan"/>, usually passed via a <see cref="ToolchainExecutionPlanBuilder"/>.
/// </summary>
public enum ToolchainConfiguration
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
	/// > This configuration is automatically enabled when no extensions are enabled, meaning that
	/// > if you pass this configuration manually when no extensions are active, you will cause RSML
	/// > to error out due to duplicated configuration, unless <see cref="SilentlyIgnoreDuplicatedConfigurations"/>
	/// > or <see cref="SilentlyIgnoreAllErrors"/> are enabled.
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

	/// <summary>
	/// Overrides the default behavior of freezing all toolchain components after the last injection.
	/// </summary>
	DoNotFreezeComponentsOnLastInjection,

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

