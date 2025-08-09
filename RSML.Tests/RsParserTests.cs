using RSML.Evaluation;
using RSML.Exceptions;
using RSML.Machine;


namespace RSML.Tests
{

	public class RsEvaluatorTests
	{

		private static readonly LocalMachine win10X64 = new("windows", "x64", 10);
		private static readonly LocalMachine ubuntu22Arm64 = new("ubuntu", "debian", "x64", 22);
		private static readonly LocalMachine debianUnknownVersionX86 = new("debian", "debian", "x86", null);
		private static readonly LocalMachine osxUnknownVersionUnknownArch = new("osx", null, null);

		[Theory]
		[InlineData(
			"-> osx \"newlines are normalized\"\n-> osx \"are they really?\"\r\n-> windows 10 defined \"Win10 I guess\"\r\n", "Win10 I guess"
		)]
		[InlineData("-> any any any \"Being explicit is good too\"\n", "Being explicit is good too")]
		[InlineData("-> \"Being implicit is bad sometimes\"\n", "Being implicit is bad sometimes")]
		[InlineData("-> \"this will always return\"\r\n", "this will always return")]
		[InlineData("# comment", null)]
		public void Evaluate_Windows10_X64_CorrectValue(string data, string? expected)
		{

			RsEvaluator parser = new(data);
			string? evaluationStr = parser.Evaluate(win10X64).MatchValue;
			Assert.Equal(expected, evaluationStr);

		}

		[Theory]
		[InlineData("-> osx \"newlines are normalized\"\n-> osx \"are they really?\"\r\n-> ubuntu \"Ubuntu I guess\"\r\n", "Ubuntu I guess")]
		[InlineData("-> any any any \"Being explicit is good too\"\n", "Being explicit is good too")]
		[InlineData("-> \"this will always return\"\n", "this will always return")]
		[InlineData("", null)] // whitespace
		public void Evaluate_Ubuntu22_Arm64_CorrectValue(string data, string? expected)
		{

			RsEvaluator parser = new(data);
			string? evaluationStr = parser.Evaluate(ubuntu22Arm64).MatchValue;
			Assert.Equal(expected, evaluationStr);

		}

		[Fact]
		public void Evaluate_LinuxMoreGlobalThanDebian()
		{

			RsEvaluator evaluator = new("-> linux \"This is the output...\"\n-> debian \"...not this.\"");
			string? evaluationStr = evaluator.Evaluate(debianUnknownVersionX86).MatchValue;
			Assert.Equal("This is the output...", evaluationStr);

		}

		[Fact]
		public void Evaluate_DebianMoreGlobalThanUbuntu()
		{

			RsEvaluator evaluator = new("-> debian \"This is the output...\"\n-> ubuntu \"...not this.\"");
			string? evaluationStr = evaluator.Evaluate(ubuntu22Arm64).MatchValue;
			Assert.Equal("This is the output...", evaluationStr);

		}

		[Fact]
		public void Evaluate_AnyWorksEvenIfUnknown()
		{

			RsEvaluator evaluator = new("-> osx any any \"Output!\"");
			string? evaluationStr = evaluator.Evaluate(osxUnknownVersionUnknownArch).MatchValue;
			Assert.Equal("Output!", evaluationStr);

		}

		[Fact]
		public void Evaluate_DefinedWorksOnlyIfKnown()
		{

			RsEvaluator evaluator = new("-> osx defined defined \"Output!\"");
			string? evaluationStr = evaluator.Evaluate(osxUnknownVersionUnknownArch).MatchValue;
			Assert.Null(evaluationStr); // no match

		}

		[Theory]
		[InlineData("==", "Result A")]
		[InlineData(">=", "Result A")]
		[InlineData("<=", "Result A")]
		[InlineData(">", "Result B")]
		[InlineData("<", "Result B")]
		[InlineData("!=", "Result B")]
		public void Evaluate_ComparatorWorks(string input, string? expected)
		{

			RsEvaluator evaluator = new($"-> windows {input} 10 defined \"Result A\"\n-> windows == 10 defined \"Result B\"\r\n");
			string? evaluationStr = evaluator.Evaluate(win10X64).MatchValue;
			Assert.Equal(expected, evaluationStr);

		}

		[Fact]
		public void Evaluate_ComparatorPlusWildcard_InvalidSyntax()
		{

			RsEvaluator evaluator = new("-> windows == defined defined \"Output!\"");
			_ = Assert.Throws<InvalidRsmlSyntax>(() => evaluator.Evaluate(win10X64));

		}

		[Fact]
		public void Evaluate_ComparatorWithNoSpacing_InvalidSyntax()
		{

			RsEvaluator evaluator = new("-> windows ==10 any \"Output!\"");
			_ = Assert.Throws<InvalidRsmlSyntax>(() => evaluator.Evaluate(win10X64));

		}

	}

}
