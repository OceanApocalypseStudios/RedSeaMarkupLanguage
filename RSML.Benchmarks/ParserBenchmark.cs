using System;
using System.Net.Http.Headers;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using RSML.Evaluation;


namespace RSML.Benchmarks
{

	[MemoryDiagnoser]
	[SimpleJob(RuntimeMoniker.Net80)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "<Pending>")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")] // vs thinks the above one is unnecessary for whatever reason
	public class RsParserBenchmarks
	{

		private string complexContent = null!;
		private string largeContent = null!;
		private string mediumContent = null!;
		private string smallContent = null!;

		[GlobalSetup]
		public void Setup()
		{

			smallContent = "test -> \"value\"\n@SpecialAction arg\n# Comment";
			mediumContent = GenerateContent(100);
			largeContent = GenerateContent(10000);
			complexContent = GenerateComplexContent(500);

		}

		private static string GenerateContent(int lines)
		{

			var sb = new StringBuilder();

			for (int i = 0; i < lines; i++)
			{

				_ = sb.AppendLine(
					i % 5 == 0
						? $"-> line{i} \"value{i}\""
						: $"# Comment {i}"
				);

			}

			return sb.ToString();

		}

		private static string GenerateComplexContent(int lines)
		{

			var sb = new StringBuilder();
			var rand = new Random();

			for (int i = 0; i < lines; i++)
			{

				_ = rand.Next(0, 6) switch
				{

					0 => sb.AppendLine($"-> line{i} \"value{i}\""),
					1 => sb.AppendLine($"@SpecialAction arg{i}"),
					2 => sb.AppendLine($"!> match{i} x64 \"value{i}\""),
					3 => sb.AppendLine($"-> line{i} 25 x64 \"value{i}\""),
					4 => sb.AppendLine($"# Comment {i}"),
					_ => sb.AppendLine($"     # Comment {i}")

				};

			}

			return sb.ToString();

		}

		private static RsEvaluator CreateConfiguredParser(string content)
		{

			RsEvaluator parser = new(content);
			parser.RegisterSpecialAction("SpecialAction", (_, _) => 0);

			return parser;

		}

		[Benchmark]
		public void Evaluate_SmallContent()
		{

			var parser = CreateConfiguredParser(smallContent);
			parser.Evaluate(new("test", null, null));

		}

		[Benchmark]
		public void Evaluate_MediumContent()
		{

			var parser = CreateConfiguredParser(mediumContent);
			parser.Evaluate(new("line50", null, null));

		}

		[Benchmark]
		public void Evaluate_LargeContent()
		{

			var parser = CreateConfiguredParser(largeContent);
			parser.Evaluate(new("line5000", null, null));

		}

		[Benchmark]
		public void Evaluate_ComplexContent()
		{

			var parser = CreateConfiguredParser(complexContent);
			parser.Evaluate(new("line250", null, null));

		}

		[Benchmark]
		public void ContentProperty_SmallContent()
		{

			var parser = CreateConfiguredParser(smallContent);
			var _ = parser.Content;

		}

		[Benchmark]
		public void ContentProperty_LargeContent()
		{

			var parser = CreateConfiguredParser(largeContent);
			var _ = parser.Content;

		}

		[Benchmark]
		public void IsComment_True_Medium() => new RsEvaluator("# ").IsComment("# This is not a big comment, but also not really a small one. Either ways, this will let us test the method and benchmark it somewhat accurately.");

		[Benchmark]
		public void IsComment_True_Small() => new RsEvaluator("# ").IsComment("# small");

		[Benchmark]
		public void IsComment_False_Medium() => new RsEvaluator("# ").IsComment("             * One day, I hope this string is no longer  spaced in  a weird way, but it'll let us test the IsComment() method of the     RsParser class locatedin the namespace known as RSML.Parser.RsParser. Interesting,  right?");

		[Benchmark]
		public void IsComment_False_Small() => new RsEvaluator("# ").IsComment("not a comment");

	}

}
