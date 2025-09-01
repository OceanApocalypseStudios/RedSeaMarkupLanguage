using System;
using System.Diagnostics;

using OceanApocalypseStudios.RSML.Actions;
using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Performance.Stateless;

using LocalMachine = OceanApocalypseStudios.RSML.Machine.LocalMachine;


namespace OceanApocalypseStudios.RSML.Tests
{

	public class StatelessEvaluatorTests
	{

		private static readonly LocalMachine win10X64 = new("windows", "x64", 10);
		private static readonly LocalMachine ubuntu22Arm64 = new("ubuntu", "debian", "x64", 22);
		private static readonly LocalMachine debianUnknownVersionX86 = new("debian", "debian", "x86", null);
		private static readonly LocalMachine osxUnknownVersionUnknownArch = new("osx", null, null);

		[Fact]
		public void Evaluate_SpecialActionNoArgument_ImplicitEmptyString()
		{

			StatelessEvaluator.SpecialActions["MyAwesomeAction"] = (_, argument) =>
			{

				Debug.WriteLine("@MyAwesomeAction was ran.");
				Assert.Equal(String.Empty, argument);

				return SpecialActionBehavior.Success;

			};

			Assert.Null(StatelessEvaluator.Evaluate("@MyAwesomeAction", debianUnknownVersionX86).MatchValue);

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

			string? evaluationStr = StatelessEvaluator.Evaluate(data, win10X64).MatchValue;
			Assert.Equal(expected, evaluationStr);

		}

		[Theory]
		[InlineData("-> osx \"newlines are normalized\"\n-> osx \"are they really?\"\r\n-> ubuntu \"Ubuntu I guess\"\r\n", "Ubuntu I guess")]
		[InlineData("-> any any any \"Being explicit is good too\"\n", "Being explicit is good too")]
		[InlineData("-> \"this will always return\"\n", "this will always return")]
		[InlineData("", null)] // whitespace
		public void Evaluate_Ubuntu22_Arm64_CorrectValue(string data, string? expected)
		{

			string? evaluationStr = StatelessEvaluator.Evaluate(data, ubuntu22Arm64).MatchValue;
			Assert.Equal(expected, evaluationStr);

		}

		[Fact]
		public void Evaluate_LinuxMoreGlobalThanDebian()
		{

			string? evaluationStr = StatelessEvaluator.Evaluate(
														  "-> linux \"This is the output...\"\n-> debian \"...not this.\"", debianUnknownVersionX86
													  )
													  .MatchValue;

			Assert.Equal("This is the output...", evaluationStr);

		}

		[Fact]
		public void Evaluate_DebianMoreGlobalThanUbuntu()
		{

			string? evaluationStr = StatelessEvaluator.Evaluate("-> debian \"This is the output...\"\n-> ubuntu \"...not this.\"", ubuntu22Arm64)
													  .MatchValue;

			Assert.Equal("This is the output...", evaluationStr);

		}

		[Fact]
		public void Evaluate_AnyWorksEvenIfUnknown()
		{

			string? evaluationStr = StatelessEvaluator.Evaluate("-> osx any any \"Output!\"", osxUnknownVersionUnknownArch).MatchValue;
			Assert.Equal("Output!", evaluationStr);

		}

		[Fact]
		public void Evaluate_DefinedWorksOnlyIfKnown()
		{

			string? evaluationStr = StatelessEvaluator.Evaluate("-> osx defined defined \"Output!\"", osxUnknownVersionUnknownArch).MatchValue;
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

			string? evaluationStr = StatelessEvaluator.Evaluate(
														  $"-> windows {input} 10 defined \"Result A\"\n-> windows == 10 defined \"Result B\"\r\n",
														  win10X64
													  )
													  .MatchValue;

			Assert.Equal(expected, evaluationStr);

		}

		[Fact]
		public void Evaluate_ComparatorPlusWildcard_InvalidSyntax() =>
			Assert.Throws<InvalidRsmlSyntax>(() => StatelessEvaluator.Evaluate("-> windows == defined defined \"Output!\"", win10X64));

		[Fact]
		public void Evaluate_ComparatorWithNoSpacing_InvalidSyntax() =>
			Assert.Throws<InvalidRsmlSyntax>(() => StatelessEvaluator.Evaluate("-> windows ==10 any \"Output!\"", win10X64));

	}

}
