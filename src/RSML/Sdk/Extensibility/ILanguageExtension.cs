namespace OceanApocalypseStudios.RSML.Sdk.Extensibility;

/// <summary>
/// Represents an extension of RSML's language API.
/// </summary>
public interface ILanguageExtension
{
	/// <summary>
	/// A required <see cref="System.String"/> with the name of the extension.
	/// </summary>
	/// <remarks>
	/// If your extension has a name that is common, it's best to use a recognizable pattern
	/// such as <c>domain.organization.product</c> or <c>domain.name.product</c> or even <c>name.product.id</c>.
	/// > [!WARNING]
	/// > The string must <strong>not</strong> contain any characters that are not allowed in the language.
	/// </remarks>
	string Name { get; }

	/// <summary>
	/// An optional <see cref="System.String"/> with a brief description of the extension.
	/// Ignored by RSML; only serves the purpose of documenting the extension.
	/// </summary>
	string? Description { get; }

	/// <summary>
	/// A required non-negative <see cref="System.Int32"/> which is the major version of the extension.
	/// </summary>
	int Major { get; }
}
