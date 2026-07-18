using OceanApocalypseStudios.RSML.Diagnostics;

namespace OceanApocalypseStudios.RSML.Language.Lexing;

public interface ILexer : IToolchainComponent
{
	Result<Token> ScanWord()
}
