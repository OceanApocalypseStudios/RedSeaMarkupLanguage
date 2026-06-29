using OceanApocalypseStudios.RSML.Evaluation;
using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Host;


namespace OceanApocalypseStudios.RSML.Tests
{

	[TestClass]
	public class EvaluatorTests
	{

		[TestMethod]
		[DataRow("any", "Main")]
		[DataRow("defined", "Fallback")]
		[DataRow("undefined", "Main")]
		[DataRow("arm64", "Fallback")] // not a keyword, but still useful to test am i fucking right
		[DataRow("x86", "Fallback")]
		public void Evaluate_KeywordsWorkWhenUnknown(string keywordValue, string expectedValue)
		{

			Evaluator evaluator = new($"-> osx {keywordValue} \"Main\"\r\n-> \"Fallback\"");
			string? evaluationStr = evaluator.Evaluate(osxUnknownVersionUnknownArch).MatchValue;
			Assert.AreEqual(expectedValue, evaluationStr);

		}

		[TestMethod]
		[DataRow("any", "Main")]
		[DataRow("defined", "Main")]
		[DataRow("undefined", "Fallback")]
		[DataRow("arm64", "Fallback")]
		[DataRow("x86", "Main")]
		public void Evaluate_KeywordsWorkWhenKnown(string keywordValue, string expectedValue)
		{

			Evaluator evaluator = new($"-> linux {keywordValue} \"Main\"\r\n-> \"Fallback\"");
			string? evaluationStr = evaluator.Evaluate(debianUnknownVersionX86).MatchValue;
			Assert.AreEqual(expectedValue, evaluationStr);

		}

		[TestMethod]
		public void Evaluate_ComparatorPlusWildcard1_InvalidSyntax()
		{

			Evaluator evaluator = new("-> windows == defined defined \"Output!\"");
			_ = Assert.Throws<InvalidRsmlSyntax>(() => evaluator.Evaluate(win10X64));

		}

		[TestMethod]
		public void Evaluate_ComparatorPlusWildcard2_InvalidSyntax()
		{

			Evaluator evaluator = new("-> windows <= defined defined \"Output!\"");
			_ = Assert.Throws<InvalidRsmlSyntax>(() => evaluator.Evaluate(win10X64));

		}

		[TestMethod]
		public void Evaluate_InBetweenPlusWildcard_InvalidSyntax()
		{

			Evaluator evaluator = new("-> windows 9 any defined \"Output!\"");
			_ = Assert.Throws<InvalidRsmlSyntax>(() => evaluator.Evaluate(win10X64));

		}

		[TestMethod]
		public void Evaluate_InBetweenUpperThenLower1_InvalidSyntax()
		{

			Evaluator evaluator = new("-> windows 11 9 defined \"Output!\"");
			_ = Assert.Throws<InvalidRsmlSyntax>(() => evaluator.Evaluate(win10X64));

		}

		[TestMethod]
		public void Evaluate_InBetweenUpperThenLower2_InvalidSyntax()
		{

			Evaluator evaluator = new("N> ubuntu 29 20 any \"Output!\"");
			_ = Assert.Throws<InvalidRsmlSyntax>(() => evaluator.Evaluate(ubuntu22Arm64));

		}

		[TestMethod]
		public void Evaluate_InBetween()
		{

			Evaluator evaluator = new("-> windows 9 11 defined \"Output!\"");
			Assert.AreEqual("Output!", evaluator.Evaluate(win10X64).MatchValue);

		}

		[TestMethod]
		public void Evaluate_NotOperator_InBetween()
		{

			Evaluator evaluator = new("N> defined 7 11 x64 \"Output A\"\nN> defined 7 11 arm64 \"Output B\"");
			Assert.AreEqual("Output B", evaluator.Evaluate(win10X64).MatchValue);

		}

		[TestMethod]
		public void Evaluate_NotOperator()
		{

			Evaluator evaluator = new("N> ubuntu 22 any \"Output A\"\nN> ubuntu 20 any \"Output B\"");
			Assert.AreEqual("Output B", evaluator.Evaluate(ubuntu22Arm64).MatchValue);

		}

		[TestMethod]
		[DataRow("==", "Result A")]
		[DataRow(">=", "Result A")]
		[DataRow("<=", "Result A")]
		[DataRow(">", "Result B")]
		[DataRow("<", "Result B")]
		[DataRow("!=", "Result B")]
		public void Evaluate_ComparatorWorks(
			string input,
			string? expected
		)
		{

			Evaluator evaluator = new($"-> windows {input} 10 defined \"Result A\"\n-> windows == 10 defined \"Result B\"\r\n");
			string? evaluationStr = evaluator.Evaluate(win10X64).MatchValue;
			Assert.AreEqual(expected, evaluationStr);

		}

		[TestMethod]
		public void Evaluate_DebianMoreGlobalThanUbuntu()
		{

			Evaluator evaluator = new("-> debian \"This is the output...\"\n-> ubuntu \"...not this.\"");
			string? evaluationStr = evaluator.Evaluate(ubuntu22Arm64).MatchValue;
			Assert.AreEqual("This is the output...", evaluationStr);

		}

		[TestMethod]
		public void Evaluate_DefinedWorksOnlyIfKnown()
		{

			Evaluator evaluator = new("-> osx defined defined \"Output!\"");
			string? evaluationStr = evaluator.Evaluate(osxUnknownVersionUnknownArch).MatchValue;
			Assert.IsNull(evaluationStr); // no match

		}

		[TestMethod]
		public void Evaluate_LinuxMoreGlobalThanDebian()
		{

			Evaluator evaluator = new("-> linux \"This is the output...\"\n-> debian \"...not this.\"");
			string? evaluationStr = evaluator.Evaluate(debianUnknownVersionX86).MatchValue;
			Assert.AreEqual("This is the output...", evaluationStr);

		}

		[TestMethod]
		[DataRow("@Void")]
		[DataRow("@Void\n")]
		[DataRow("@Void\n# Comment")]
		[DataRow("@Void\n# Comment\n")]
		[DataRow("@Void \n")]
		[DataRow("@Void ")]
		[DataRow("@Void Argument")]
		[DataRow("@EndAll\n-> linux \"Result C\"")]
		[DataRow("@EndAll\n-> linux \"Result C\"\n")]
		public void Evaluate_SpecialActionWorks(string input)
		{

			Evaluator evaluator = new(input);
			Assert.IsNull(evaluator.Evaluate(debianUnknownVersionX86).MatchValue);

		}

		[TestMethod]
		[DataRow("-> osx \"newlines are normalized\"\n-> osx \"are they really?\"\r\n-> ubuntu \"Ubuntu I guess\"\r\n", "Ubuntu I guess")]
		[DataRow("-> any any any \"Being explicit is good too\"\n", "Being explicit is good too")]
		[DataRow("-> \"this will always return\"\n", "this will always return")]
		[DataRow("", null)] // whitespace
		public void Evaluate_Ubuntu22_Arm64_CorrectValue(
			string data,
			string? expected
		)
		{

			Evaluator parser = new(data);
			string? evaluationStr = parser.Evaluate(ubuntu22Arm64).MatchValue;
			Assert.AreEqual(expected, evaluationStr);

		}

		[TestMethod]
		[DataRow("-> osx \"newlines are normalized\"\n-> osx \"are they really?\"\r\n-> windows 10 defined \"Win10 I guess\"\r\n", "Win10 I guess")]
		[DataRow("-> any any any \"Being explicit is good too\"\n", "Being explicit is good too")]
		[DataRow("-> \"Being implicit is bad sometimes\"\n", "Being implicit is bad sometimes")]
		[DataRow("-> \"this will always return\"\r\n", "this will always return")]
		[DataRow("# comment", null)]
		public void Evaluate_Windows10_X64_CorrectValue(
			string data,
			string? expected
		)
		{

			Evaluator parser = new(data);
			string? evaluationStr = parser.Evaluate(win10X64).MatchValue;
			Assert.AreEqual(expected, evaluationStr);

		}

		[TestMethod]
		[DataRow("#")]
		[DataRow("#Comment")]
		[DataRow("# ")]
		[DataRow("# Comment")]
		[DataRow("#                                   Comment")]
		[DataRow("    # Still a comment")]
		[DataRow("                                            #")]
		public void Evaluator_IsComment(string input) => Assert.IsTrue(Evaluator.IsComment(input));

		private static readonly HostInfo debianUnknownVersionX86 = new(
			"debian",
			"debian",
			"x86",
			null
		);

		private static readonly HostInfo osxUnknownVersionUnknownArch = new("osx", null, null);

		private static readonly HostInfo ubuntu22Arm64 = new(
			"ubuntu",
			"debian",
			"arm64",
			22
		);

		private static readonly HostInfo win10X64 = new("windows", "x64", 10);

	}

}
