using System;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using RSML.Language;
using RSML.Parser;


namespace RSML.Benchmarks
{

	[MemoryDiagnoser]
	[SimpleJob(RuntimeMoniker.Net80)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "<Pending>")]
	public class RSParserBenchmarks
	{

		private string smallContent = null!;
		private string mediumContent = null!;
		private string largeContent = null!;
		private string complexContent = null!;

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
				_ = sb.AppendLine(i % 5 == 0
					? $"line{i} -> \"value{i}\""
					: $"# Comment {i}");

			return sb.ToString();

		}

		private static string GenerateComplexContent(int lines)
		{

			var sb = new StringBuilder();
			var rand = new Random();

			for (int i = 0; i < lines; i++)
			{

				_ = rand.Next(0, 4) switch
				{

					0 => sb.AppendLine($"line{i} -> \"value{i}\""),
					1 => sb.AppendLine($"@SpecialAction arg{i}"),
					2 => sb.AppendLine($"match{i} || \"action{i}\""),
					_ => sb.AppendLine($"# Comment {i}")

				};

			}

			return sb.ToString();

		}

		private static RSParser CreateConfiguredParser(string content)
		{

			var parser = new RSParser(content, LanguageStandard.Official25);
			parser.RegisterSpecialAction("SpecialAction", (_, _) => 0);
			return parser;

		}

		[Benchmark]
		public void Evaluate_SmallContent()
		{

			var parser = CreateConfiguredParser(smallContent);
			parser.Evaluate("test");

		}

		[Benchmark]
		public void Evaluate_MediumContent()
		{

			var parser = CreateConfiguredParser(mediumContent);
			parser.Evaluate("line50");

		}

		[Benchmark]
		public void Evaluate_LargeContent()
		{

			var parser = CreateConfiguredParser(largeContent);
			parser.Evaluate("line5000");

		}

		[Benchmark]
		public void Evaluate_ComplexContent()
		{

			var parser = CreateConfiguredParser(complexContent);
			parser.Evaluate("line250");

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
		public void GetCommentType_Explicit()
		{

			var parser = CreateConfiguredParser(smallContent);
			parser.GetCommentType("# comment");

		}

		[Benchmark]
		public void GetCommentType_Implicit()
		{

			var parser = CreateConfiguredParser(smallContent);
			parser.GetCommentType("just text");

		}

	}

}
