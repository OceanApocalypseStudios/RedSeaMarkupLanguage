using System.Diagnostics.CodeAnalysis;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

using OceanApocalypseStudios.RSML.Evaluation;
using OceanApocalypseStudios.RSML.Machine;


namespace OceanApocalypseStudios.RSML.Benchmarks
{

	[MemoryDiagnoser]
	[HideColumns("Job", "StdDev")]
	[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
	[SuppressMessage("Performance", "CA1822:Mark members as static")]
	public class EvaluatorBenchmarks
	{

		private readonly LocalMachine ubuntu = new("ubuntu", null, null);

		private Evaluator? complexEvaluator1;
		private Evaluator? complexEvaluator2;
		private Evaluator? complexEvaluator3;

		private Evaluator? smallEvaluator;
		private Evaluator? mediumEvaluator;
		private Evaluator? largeEvaluator;

		private Evaluator? primitiveEvaluatorAction;
		private Evaluator? primitiveEvaluatorComment;
		private Evaluator? primitiveEvaluatorCommentWs;
		private Evaluator? primitiveEvaluatorLogic;
		private Evaluator? primitiveEvaluatorNewlines;

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

			primitiveEvaluatorAction = CreateConfiguredParser("@Void\n@Void\n@EndAll");
			primitiveEvaluatorComment = CreateConfiguredParser("# comment");
			primitiveEvaluatorCommentWs = CreateConfiguredParser("                       # comment");
			primitiveEvaluatorLogic = CreateConfiguredParser("-> windows 10 x64 \"Some random value\"");
			primitiveEvaluatorNewlines = CreateConfiguredParser("\n\n\n\n\n\n\n\n");

			smallEvaluator = CreateConfiguredParser("-> windows \"value\"\n@Void arg\n# Comment");
			mediumEvaluator = CreateConfiguredParser(Dataset.MediumContent.Data);
			largeEvaluator = CreateConfiguredParser(Dataset.LargeContent.Data);

			complexEvaluator1 = CreateConfiguredParser(Dataset.ComplexContent1.Data);
			complexEvaluator2 = CreateConfiguredParser(Dataset.ComplexContent2.Data);
			complexEvaluator3 = CreateConfiguredParser(Dataset.ComplexContent3.Data);

		}

		internal static Evaluator CreateConfiguredParser(string content) => new(content);

	}

}
