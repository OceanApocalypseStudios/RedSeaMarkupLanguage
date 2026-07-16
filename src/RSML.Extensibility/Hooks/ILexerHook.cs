using OceanApocalypseStudios.RSML.Extensibility.Registries;


namespace OceanApocalypseStudios.RSML.Extensibility.Hooks;

/// <summary>
/// Represents an extension that modifies the <see cref="Language.Parsing.Parser"/>.
/// </summary>
public interface ILexerHook : ILanguageExtension
{
	void Register(LexerRuleRegistry rules);
}
