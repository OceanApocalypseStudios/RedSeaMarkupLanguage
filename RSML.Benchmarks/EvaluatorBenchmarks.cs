using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

using RSML.Evaluation;
using RSML.Machine;


namespace RSML.Benchmarks
{

	[MemoryDiagnoser]
	[SimpleJob(RuntimeMoniker.Net80)]
	[WarmupCount(2)]
	[IterationCount(3)]
	[HideColumns("Job", "StdDev")]
	[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
	[SuppressMessage("Performance", "CA1822:Mark members as static")]
	public class EvaluatorBenchmarks
	{

		private readonly LocalMachine ubuntu = new("ubuntu", null, null);

		private Evaluator? primitiveEvaluatorLogic;
		private Evaluator? primitiveEvaluatorAction;
		private Evaluator? primitiveEvaluatorComment;
		private Evaluator? primitiveEvaluatorCommentWs;
		private Evaluator? primitiveEvaluatorNewlines;

		private Evaluator? complexEvaluator1;
		private Evaluator? complexEvaluator2;
		private Evaluator? complexEvaluator3;

		private Evaluator? smallEvaluator;
		private Evaluator? mediumEvaluator;
		private Evaluator? largeEvaluator;

		[GlobalSetup]
		public void Setup()
		{

			var datasetPath = Path.Join(Environment.CurrentDirectory, "..", "..", "..", "..", "Dataset");

			primitiveEvaluatorComment = CreateConfiguredParser("# comment");
			primitiveEvaluatorCommentWs = CreateConfiguredParser("                       # comment");
			primitiveEvaluatorNewlines = CreateConfiguredParser("\n\n\n\n\n\n\n\n");
			primitiveEvaluatorLogic = CreateConfiguredParser("-> windows 10 x64 \"Some random value\"");
			primitiveEvaluatorAction = CreateConfiguredParser("@SpecialAction\n@SpecialAction\n@EndAll");

			smallEvaluator = CreateConfiguredParser("-> windows \"value\"\n@SpecialAction arg\n# Comment");
			mediumEvaluator = CreateConfiguredParser(File.ReadAllText(Path.Join(datasetPath, "medium_content.rsea")));
			largeEvaluator = CreateConfiguredParser(File.ReadAllText(Path.Join(datasetPath, "large_content.rsea")));

			complexEvaluator1 = CreateConfiguredParser(File.ReadAllText(Path.Join(datasetPath, "complex_content_1.rsea")));
			complexEvaluator2 = CreateConfiguredParser(File.ReadAllText(Path.Join(datasetPath, "complex_content_2.rsea")));
			complexEvaluator3 = CreateConfiguredParser(File.ReadAllText(Path.Join(datasetPath, "complex_content_3.rsea")));

		}

		internal static string GenerateContent(int lines)
		{

			var sb = new StringBuilder();

			for (int i = 0; i < lines; i++)
			{

				_ = sb.AppendLine(
					i % 5 == 0
						? $"-> windows \"value{i}\""
						: $"# Comment {i}"
				);

			}

			return sb.ToString();

		}

		internal static string GenerateComplexContent(int lines)
		{

			var sb = new StringBuilder();
			var rand = new Random();

			for (int i = 0; i < lines; i++)
			{

				_ = rand.Next(0, 6) switch
				{

					0 => sb.AppendLine($"-> windows \"value{i}\""),
					1 => sb.AppendLine($"@SpecialAction arg{i}"),
					2 => sb.AppendLine($"!> windows x64 \"value{i}\""),
					3 => sb.AppendLine($"-> windows {i} x64 \"value{i}\""),
					4 => sb.AppendLine($"# Comment {i}"),
					_ => sb.AppendLine($"     # Comment {i}")

				};

			}

			return sb.ToString();

		}

		internal static Evaluator CreateConfiguredParser(string content)
		{

			var evaluator = new Evaluator(content);
			evaluator.RegisterSpecialAction("SpecialAction", (_, _) => 0);

			return evaluator;

		}

		[Benchmark]
		[BenchmarkCategory("Primitives")]
		public void Evaluate_PrimitiveLogic() => primitiveEvaluatorLogic!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Primitives")]
		public void Evaluate_PrimitiveAction() => primitiveEvaluatorAction!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Primitives")]
		public void Evaluate_PrimitiveComment() => primitiveEvaluatorComment!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Primitives")]
		public void Evaluate_PrimitiveCommentWhitespace() => primitiveEvaluatorCommentWs!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Primitives")]
		public void Evaluate_PrimitiveNewlines() => primitiveEvaluatorNewlines!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_SmallContent() => smallEvaluator!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_MediumContent() => mediumEvaluator!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_LargeContent() => largeEvaluator!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_ComplexContent_1() => complexEvaluator1!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_ComplexContent_2() => complexEvaluator2!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_ComplexContent_3() => complexEvaluator3!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("IsComment")]
		public void IsComment_True_Medium() =>
			Evaluator.IsComment(
				"# This is not a big comment, but also not really a small one. Either ways, this will let us test the method and benchmark it somewhat accurately."
			);

		[Benchmark]
		[BenchmarkCategory("IsComment")]
		public void IsComment_True_Small() => Evaluator.IsComment("# small");

		[Benchmark]
		[BenchmarkCategory("IsComment")]
		public void IsComment_False_Medium() =>
			Evaluator.IsComment(
				"             * One day, I hope this string is no longer  spaced in  a weird way, but it'll let us test the IsComment() method of the     RsParser class located    in the namespace known as RSML.Parser.RsParser. Interesting,  right?"
			);

		[Benchmark]
		[BenchmarkCategory("IsComment")]
		public void IsComment_False_Small() => Evaluator.IsComment("not a comment");

	}

}
