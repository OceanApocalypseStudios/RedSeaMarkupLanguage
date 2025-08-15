using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

using RSML.Evaluation;


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

		private Evaluator? complexEvaluator;
		private Evaluator? largeEvaluator;
		private Evaluator? mediumEvaluator;
		private Evaluator? smallEvaluator;

		[GlobalSetup]
		public void Setup()
		{

			smallEvaluator = CreateConfiguredParser("-> windows \"value\"\n@SpecialAction arg\n# Comment");
			mediumEvaluator = CreateConfiguredParser(GenerateContent(100));
			largeEvaluator = CreateConfiguredParser(GenerateContent(10000));
			complexEvaluator = CreateConfiguredParser(GenerateComplexContent(500));

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
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_SmallContent() => smallEvaluator!.Evaluate(new("ubuntu", null, null));

		[Benchmark]
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_MediumContent() => mediumEvaluator!.Evaluate(new("ubuntu", null, null));

		[Benchmark]
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_LargeContent() => largeEvaluator!.Evaluate(new("ubuntu", null, null));

		[Benchmark]
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_ComplexContent() => complexEvaluator!.Evaluate(new("ubuntu", null, null));

		[Benchmark]
		[BenchmarkCategory("ContentPropertyAccess")]
		public void ContentProperty_SmallContent() => _ = smallEvaluator!.Content;

		[Benchmark]
		[BenchmarkCategory("ContentPropertyAccess")]
		public void ContentProperty_LargeContent() => _ = largeEvaluator!.Content;

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
