namespace OceanApocalypseStudios.RSML.Sdk.Extensibility.Hooks;

public interface ILexerHook : ILanguageExtension
{
	void Register(LexerRuleRegistry rules);
}
