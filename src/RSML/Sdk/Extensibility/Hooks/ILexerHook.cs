using OceanApocalypseStudios.RSML.Sdk.Extensibility.Registries;


namespace OceanApocalypseStudios.RSML.Sdk.Extensibility.Hooks;

/// <summary>
/// Represents an extension that modifies the <see cref="Language.Parsing.Parser"/>.
/// </summary>
public interface ILexerHook : ILanguageExtension
{
	void Register(LexerRuleRegistry rules);
}
