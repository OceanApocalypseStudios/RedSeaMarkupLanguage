using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

using OceanApocalypseStudios.RSML.Evaluation;

using LocalMachine = OceanApocalypseStudios.RSML.Machine.LocalMachine;


namespace OceanApocalypseStudios.RSML.Benchmarks
{

	[MemoryDiagnoser]
	[SimpleJob(RuntimeMoniker.Net80, warmupCount: 10, iterationCount: 50)]
	[HideColumns("Job", "StdDev")]
	[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
	[SuppressMessage("Performance", "CA1822:Mark members as static")]
	public class EvaluatorBenchmarks
	{

		private const string SmallContent = "-> windows \"value\"\n@Void arg\n# Comment";

		private readonly LocalMachine ubuntu = new("ubuntu", null, null);

		private string? complexContent1;
		private string? complexContent2;

		private Evaluator? complexEvaluator1;
		private Evaluator? complexEvaluator2;
		private Evaluator? complexEvaluator3;
		private string? largeContent;
		private Evaluator? largeEvaluator;
		private string? mediumContent;
		private Evaluator? mediumEvaluator;

		private Evaluator? primitiveEvaluatorAction;
		private Evaluator? primitiveEvaluatorComment;
		private Evaluator? primitiveEvaluatorCommentWs;

		private Evaluator? primitiveEvaluatorLogic;
		private Evaluator? primitiveEvaluatorNewlines;

		private Evaluator? smallEvaluator;

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
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_LargeContent() => largeEvaluator!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_MediumContent() => mediumEvaluator!.Evaluate(ubuntu);

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
		public void Evaluate_PrimitiveLogic() => primitiveEvaluatorLogic!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Primitives")]
		public void Evaluate_PrimitiveNewlines() => primitiveEvaluatorNewlines!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("Evaluator")]
		public void Evaluate_SmallContent() => smallEvaluator!.Evaluate(ubuntu);

		[Benchmark]
		[BenchmarkCategory("IsComment")]
		public void IsComment_False_Medium() =>
			Evaluator.IsComment(
				"             * One day, I hope this string is no longer  spaced in  a weird way, but it'll let us test the IsComment() method of the     RsParser class located    in the namespace known as RSML.Parser.RsParser. Interesting,  right?"
			);

		[Benchmark]
		[BenchmarkCategory("IsComment")]
		public void IsComment_False_Small() => Evaluator.IsComment("not a comment");

		[Benchmark]
		[BenchmarkCategory("IsComment")]
		public void IsComment_True_Medium() =>
			Evaluator.IsComment(
				"# This is not a big comment, but also not really a small one. Either ways, this will let us test the method and benchmark it somewhat accurately."
			);

		[Benchmark]
		[BenchmarkCategory("IsComment")]
		public void IsComment_True_Small() => Evaluator.IsComment("# small");

		[GlobalSetup]
		public void Setup()
		{

			string datasetPath = Path.Join(
				Environment.CurrentDirectory, "..", "..", "..", "..",
				"Dataset"
			);

			primitiveEvaluatorComment = CreateConfiguredParser("# comment");
			primitiveEvaluatorCommentWs = CreateConfiguredParser("                       # comment");
			primitiveEvaluatorNewlines = CreateConfiguredParser("\n\n\n\n\n\n\n\n");
			primitiveEvaluatorLogic = CreateConfiguredParser("-> windows 10 x64 \"Some random value\"");
			primitiveEvaluatorAction = CreateConfiguredParser("@Void\n@Void\n@EndAll");

			mediumContent = File.ReadAllText(Path.Join(datasetPath, "medium_content.rsea"));
			largeContent = File.ReadAllText(Path.Join(datasetPath, "large_content.rsea"));

			complexContent1 = File.ReadAllText(Path.Join(datasetPath, "complex_content_1.rsea"));
			complexContent2 = File.ReadAllText(Path.Join(datasetPath, "complex_content_2.rsea"));

			smallEvaluator = CreateConfiguredParser(SmallContent);
			mediumEvaluator = CreateConfiguredParser(mediumContent);
			largeEvaluator = CreateConfiguredParser(largeContent);

			complexEvaluator1 = CreateConfiguredParser(complexContent1);
			complexEvaluator2 = CreateConfiguredParser(complexContent2);
			complexEvaluator3 = CreateConfiguredParser(File.ReadAllText(Path.Join(datasetPath, "complex_content_3.rsea")));

		}

		internal static Evaluator CreateConfiguredParser(string content) => new(content);

	}

}
