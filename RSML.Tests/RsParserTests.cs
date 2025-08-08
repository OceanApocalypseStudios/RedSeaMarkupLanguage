using RSML.Evaluation;
using RSML.Machine;


namespace RSML.Tests
{

	public class RsEvaluatorTests
	{

		private static readonly LocalMachine win10X64 = new("windows", "x64", 10);
		private static readonly LocalMachine ubuntu22Arm64 = new("ubuntu", "debian", "x64", 22);

		[Theory]
		[InlineData("-> \"this will always return\"\r\n", "this will always return")]
		[InlineData("# comment", null)]
		public void Evaluate_Windows10_X64_CorrectValue(string data, string? expected)
		{

			RsEvaluator parser = new(data);
			var evaluationStr = parser.Evaluate(win10X64).MatchValue;
			Assert.Equal(expected, evaluationStr);

		}

		[Theory]
		[InlineData("-> \"this will always return\"\n", "this will always return")]
		[InlineData("", null)] // whitespace
		public void Evaluate_Ubuntu22_Arm64_CorrectValue(string data, string? expected)
		{

			RsEvaluator parser = new(data);
			var evaluationStr = parser.Evaluate(ubuntu22Arm64).MatchValue;
			Assert.Equal(expected, evaluationStr);

		}

	}

}
