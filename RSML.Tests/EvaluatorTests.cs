using OceanApocalypseStudios.RSML.Evaluation;
using OceanApocalypseStudios.RSML.Exceptions;

using LocalMachine = OceanApocalypseStudios.RSML.Machine.LocalMachine;


namespace OceanApocalypseStudios.RSML.Tests
{

	public class EvaluatorTests
	{

		private static readonly LocalMachine debianUnknownVersionX86 = new("debian", "debian", "x86", null);
		private static readonly LocalMachine osxUnknownVersionUnknownArch = new("osx", null, null);
		private static readonly LocalMachine ubuntu22Arm64 = new("ubuntu", "debian", "x64", 22);

		private static readonly LocalMachine win10X64 = new("windows", "x64", 10);

		[Fact]
		public void Evaluate_AnyWorksEvenIfUnknown()
		{

			Evaluator evaluator = new("-> osx any any \"Output!\"");
			string? evaluationStr = evaluator.Evaluate(osxUnknownVersionUnknownArch).MatchValue;
			Assert.Equal("Output!", evaluationStr);

		}

		[Fact]
		public void Evaluate_ComparatorPlusWildcard_InvalidSyntax()
		{

			Evaluator evaluator = new("-> windows == defined defined \"Output!\"");
			_ = Assert.Throws<InvalidRsmlSyntax>(() => evaluator.Evaluate(win10X64));

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

			Evaluator evaluator = new($"-> windows {input} 10 defined \"Result A\"\n-> windows == 10 defined \"Result B\"\r\n");
			string? evaluationStr = evaluator.Evaluate(win10X64).MatchValue;
			Assert.Equal(expected, evaluationStr);

		}

		[Fact]
		public void Evaluate_DebianMoreGlobalThanUbuntu()
		{

			Evaluator evaluator = new("-> debian \"This is the output...\"\n-> ubuntu \"...not this.\"");
			string? evaluationStr = evaluator.Evaluate(ubuntu22Arm64).MatchValue;
			Assert.Equal("This is the output...", evaluationStr);

		}

		[Fact]
		public void Evaluate_DefinedWorksOnlyIfKnown()
		{

			Evaluator evaluator = new("-> osx defined defined \"Output!\"");
			string? evaluationStr = evaluator.Evaluate(osxUnknownVersionUnknownArch).MatchValue;
			Assert.Null(evaluationStr); // no match

		}

		[Fact]
		public void Evaluate_LinuxMoreGlobalThanDebian()
		{

			Evaluator evaluator = new("-> linux \"This is the output...\"\n-> debian \"...not this.\"");
			string? evaluationStr = evaluator.Evaluate(debianUnknownVersionX86).MatchValue;
			Assert.Equal("This is the output...", evaluationStr);

		}

		[Theory]
		[InlineData("-> osx \"newlines are normalized\"\n-> osx \"are they really?\"\r\n-> ubuntu \"Ubuntu I guess\"\r\n", "Ubuntu I guess")]
		[InlineData("-> any any any \"Being explicit is good too\"\n", "Being explicit is good too")]
		[InlineData("-> \"this will always return\"\n", "this will always return")]
		[InlineData("", null)] // whitespace
		public void Evaluate_Ubuntu22_Arm64_CorrectValue(string data, string? expected)
		{

			Evaluator parser = new(data);
			string? evaluationStr = parser.Evaluate(ubuntu22Arm64).MatchValue;
			Assert.Equal(expected, evaluationStr);

		}

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

			Evaluator parser = new(data);
			string? evaluationStr = parser.Evaluate(win10X64).MatchValue;
			Assert.Equal(expected, evaluationStr);

		}

		[Theory]
		[InlineData("#")]
		[InlineData("#Comment")]
		[InlineData("# ")]
		[InlineData("# Comment")]
		[InlineData("#                                   Comment")]
		[InlineData("    # Still a comment")]
		[InlineData("                                            #")]
		public void Evaluator_IsComment(string input) => Assert.True(Evaluator.IsComment(input));

	}

}
