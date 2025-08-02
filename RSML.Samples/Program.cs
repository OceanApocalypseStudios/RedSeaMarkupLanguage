using System;
using System.Diagnostics;
using System.Text;
using RSML.Language;
using RSML.Parser;


namespace RSML.Samples
{

	internal class Program
	{

		private readonly static string smallContent = "test -> \"value\"\n@SpecialAction arg\n# Comment";
		private readonly static string mediumContent = GenerateContent(100);
		private readonly static string largeContent = GenerateContent(10000);
		private readonly static string complexContent = GenerateComplexContent(500);

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

		static void Main()
		{

			Console.Write("Insert sample ID: ");
			int id = Convert.ToInt32(Console.ReadLine());
			Console.Write('\n');

			// ok

			if (id == 0)
			{

				var sRes = CreateConfiguredParser(smallContent).Evaluate("test");
				Console.WriteLine($"{sRes} : {sRes.WasMatchFound}");
				return;

			}

			if (id == -1)
			{

				var sRes = CreateConfiguredParser(mediumContent).Evaluate("line50");
				Console.WriteLine($"{sRes} : {sRes.WasMatchFound}");
				return;

			}

			if (id == -2)
			{

				var sRes = CreateConfiguredParser(largeContent).Evaluate("line5000");
				Console.WriteLine($"{sRes} : {sRes.WasMatchFound}");
				return;

			}

			if (id == -3)
			{

				var sRes = CreateConfiguredParser(complexContent).Evaluate("line250");
				Console.WriteLine($"{sRes} : {sRes.WasMatchFound}");
				return;

			}

			var res = SampleRunner.Run(id);
			Console.WriteLine($"{res?.ToString()} : {res?.WasMatchFound}");
			return;

		}

	}

}
