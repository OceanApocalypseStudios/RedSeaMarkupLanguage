using Spectre.Console;

using RSML.Evaluation;
using RSML.Machine;


namespace RSML.CLI
{

	internal partial class Program
	{

		public static void LocalMachineInfo_Pretty() => LocalMachineOutput.AsPrettyText(new());

		public static string? LocalMachineInfo_NoPretty(string outputFormat)
		{

			LocalMachine machine = new();
			return outputFormat switch
			{

				"PlainText" => LocalMachineOutput.AsPlainText(machine),
				"JSON" => LocalMachineOutput.AsJson(machine),
				_ => null

			};

		}

		public static string? CompileRsml_NoPretty(string rsml, string language, string moduleName)
		{

			var result = new Evaluator(rsml).Evaluate();
			return language switch
			{

				"C#" => CompiledRsmlGenerator.GenerateCSharp(moduleName, result.WasMatchFound ? $"(\"{result.MatchValue!}\")" : "()"),
				"F#" => CompiledRsmlGenerator.GenerateFSharp(moduleName, result.WasMatchFound ? $"(\"{result.MatchValue!}\")" : "()"),
				"VB" => CompiledRsmlGenerator.GenerateVisualBasic(moduleName, result.WasMatchFound ? $"(\"{result.MatchValue!}\")" : "()"),
				_ => null

			};

		}

		public static int SpecificationSupport_NoPretty()
		{

			// for the nodes, use these characters: ! (partial) X (nope) V (yes)
			Tree tree = new($"[cyan]Language Specification Support[/] ({LanguageVersion})");

			var statements = tree.AddNode("[green](V)[/] Statement Syntax");
			_ = statements.AddNode("[green](V)[/] Each statement must be in a different line");

			_ = statements.AddNode("[green](V)[/] Full-line Comments with #");

			var logicPaths = statements.AddNode("[green](V)[/] Logic Paths");

			var overloads = logicPaths.AddNode("[green](V)[/] Overloads");
			_ = overloads.AddNode("[green](V)[/] Operator + Value");
			_ = overloads.AddNode("[green](V)[/] Operator + System Name + Value");
			_ = overloads.AddNode("[green](V)[/] Operator + System Name + Processor Architecture + Value");
			_ = overloads.AddNode("[green](V)[/] Operator + System Name + Major Version Number + Processor Architecture + Value");
			_ = overloads.AddNode("[green](V)[/] Operator + System Name + Comparator + Major Version Number + Processor Architecture + Value");

			var rules = logicPaths.AddNode("[green](V)[/] Overloading Rules");
			_ = rules.AddNode("[green](V)[/] Generic overloads are treated as more specific ones that maintain the same semantic value");
			_ = rules.AddNode("[green](V)[/] Comparators cannot be used to compare wildcards");

			var operators = logicPaths.AddNode("[green](V)[/] Operators");
			_ = operators.AddNode("[green](V)[/] Return Operator (->)");
			_ = operators.AddNode("[green](V)[/] Error-throw Operator (!>)");

			var specialActions = statements.AddNode("[green](V)[/] Special Actions with @");
			_ = specialActions.AddNode("[green](V)[/] Argument-less")
							  .AddNode("[green](V)[/] Argument-less are treated as Single-argument with empty argument");
			_ = specialActions.AddNode("[green](V)[/] Single-argument");

			var lexicalStructure = tree.AddNode("[yellow](!)[/] Lexical Structure");

			var charset = lexicalStructure.AddNode("[yellow](!)[/] Character Set");
			_ = charset.AddNode("[red](X)[/] UTF-8");
			_ = charset.AddNode("[green](V)[/] UTF-16");

			var tokens = lexicalStructure.AddNode("[green](V)[/] All token types supported");

			AnsiConsole.Write(tree);

			return 0;

		}

	}

}
