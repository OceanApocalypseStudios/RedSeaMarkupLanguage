using System;
using System.Diagnostics;

using RSML.CLI.Helpers;
using RSML.Evaluation;
using RSML.Exceptions;
using RSML.Machine;

using Spectre.Console;


namespace RSML.CLI
{

	internal partial class Program
	{

		public static void Evaluate_NoPretty(string data, LocalMachine machine, bool cacheActionResults = true) =>
			Console.WriteLine(new Evaluator(data).Evaluate(machine, !cacheActionResults).MatchValue);

		public static void Evaluate_Pretty(string data, LocalMachine machine, bool cacheActionResults = true)
		{

			Evaluator evaluator = new(data);

			var start = Stopwatch.GetTimestamp();
			var result = evaluator.Evaluate(machine, !cacheActionResults);

			if (data.Length <= 10000) // 10000 characters, not lines brother
			{

				for (int i = 0; i < 10; i++)
					_ = evaluator.Evaluate(machine, !cacheActionResults);
				// average of several calls to avoid overhead of Stopwatch

			}

			var end = Stopwatch.GetTimestamp();

			var timeTaken = (end - start) * 1000 / Stopwatch.Frequency;

			if (data.Length <= 10000)
				timeTaken /= 10; // iterations (or calls)

			AnsiConsole.Write(
				new Columns(
					new Panel(
						new Rows(
							new Markup("[yellow]Evaluation Result[/]").Centered(),
							new Markup(
								result.WasMatchFound ? "[green]Match found![/]" : "[red]No matches found.[/]", new(null, null, Decoration.Italic)
							),
							new Markup(result.WasMatchFound ? result.MatchValue! : "", new(Color.White))
						).Expand()
					).Expand(),
					new Panel(
						new Rows(
							new Markup("[green]Statistics[/]").Centered(),
							new Markup($"[white]Time elapsed [/][red](crude estimate)[/][white]:[/] [gray]{timeTaken} ms[/]"),
							new Markup($"[white]Special actions loaded:[/] [gray]{evaluator.SpecialActions.Count + 1}[/]"),
							new Markup($"[white]Middlewares loaded:[/] [gray]{evaluator.LoadedMiddlewaresCount}[/]"),
							new Markup($"[white]Special action caching:[/] [gray]{cacheActionResults}[/]"),
							new Markup($"[white]Amount of characters in document:[/] [gray]{data.Length}[/]")
						)
					).Expand()
				).Expand()
			);

		}

		public static int GetMachine(LocalMachine machine, bool disableAnsi, string? format)
		{

			if (disableAnsi || format == "JSON")
			{

				string? x = LocalMachineInfo_NoPretty(machine, format ?? "InvalidValue");

				if (x is not null)
					Console.WriteLine(x);

				return x is not null ? 0 : 1;

			}

			if (format == "PlainText")
				LocalMachineInfo_Pretty(machine); // eh eh
			else
				return 1;

			return 0;

		}

		public static void LocalMachineInfo_Pretty(LocalMachine machine) => LocalMachineOutput.AsPrettyText(machine);

		public static string? LocalMachineInfo_NoPretty(LocalMachine machine, string outputFormat) =>
			outputFormat switch
			{

				"PlainText" => LocalMachineOutput.AsPlainText(machine),
				"JSON"      => LocalMachineOutput.AsJson(machine),
				_           => null

			};

		public static string? CompileRsml_NoPretty(string rsml, string language, string moduleName)
		{

			EvaluationResult result;

			try
			{

				result = new Evaluator(rsml).Evaluate();

			}
			catch (UserRaisedException)
			{

				return null;

			}
			catch (InvalidRsmlSyntax)
			{

				return null;

			}

			return language switch
			{

				"C#" => CompiledRsmlGenerator.GenerateCSharp(moduleName, result.WasMatchFound ? $"(\"{result.MatchValue!}\")" : "()"),
				"F#" => CompiledRsmlGenerator.GenerateFSharp(moduleName, result.WasMatchFound ? $"(\"{result.MatchValue!}\")" : "()"),
				"VB" => CompiledRsmlGenerator.GenerateVisualBasic(moduleName, result.WasMatchFound ? $"(\"{result.MatchValue!}\")" : "()"),
				_    => null

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

			_ = lexicalStructure.AddNode("[green](V)[/] All token types supported");

			AnsiConsole.Write(tree);

			return 0;

		}

	}

}
