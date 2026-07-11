using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Language.Lexing;

public interface ILexerRule
{
	// todo: implement this
	bool TryMatch(ISource source, SourceLocation position, out Token token);
}
