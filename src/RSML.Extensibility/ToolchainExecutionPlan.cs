using OceanApocalypseStudios.RSML.Execution;
using OceanApocalypseStudios.RSML.Language.Lexing;
using OceanApocalypseStudios.RSML.Language.Parsing;
using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Extensibility;

public sealed class ToolchainExecutionPlan(ISource source, Lexer lexer, Parser parser, Interpreter interpreter, ILanguageExtension[] extensions, ToolchainConfiguration[] configurations)
{
	// todo: implement this
	private ISource Source { get; } = source;
	private Lexer Lexer { get; } = lexer;
	private Parser Parser { get; } = parser;
	private Interpreter Interpreter { get; } = interpreter;
	private ILanguageExtension[] Extensions { get; } = extensions;
	private ToolchainConfiguration[] Configurations { get; } = configurations;
}
