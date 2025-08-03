using System;

using RSML.Exceptions;
using RSML.Language;
using RSML.Parser;


namespace RSML.Tests.Parser
{

	public partial class RsParserTests
	{

		[Fact]
		public void Evaluate_WithPrimaryOperatorMatch_ReturnsCorrectValue()
		{

			RsParser parser = new("test -> \"value\"", LanguageStandard.Official25);

			var result = parser.Evaluate("test");

			Assert.True(result.WasMatchFound);
			Assert.Equal("value", result.MatchValue);

		}

		[Fact]
		public void Evaluate_WithSpecialAction_ExecutesAction()
		{

			bool actionExecuted = false;
			var parser = new RsParser("@TestAction", LanguageStandard.Official25);

			parser.RegisterSpecialAction(
				"TestAction", (_, _) =>
				{
					actionExecuted = true;

					return 0;
				}
			);

			parser.Evaluate("test");

			Assert.True(actionExecuted);

		}

		[Fact]
		public void Evaluate_WithEndAll_StopsEvaluation()
		{

			string content = "line1\n@EndAll\ntest -> \"this shouldn't be accessed\"";
			RsParser parser = new(content, LanguageStandard.Official25);

			var result = parser.Evaluate("test");

			Assert.False(result.WasMatchFound);

		}

		[Theory]
		[InlineData("win.* -> xs")] // too small input
		[InlineData("win.+ -> no quotes")]
		public void HandleOperatorAction_WithInvalidSyntax_ThrowsInvalidRSMLSyntax(string input)
		{

			var parser = new RsParser(input, LanguageStandard.Official25);

			try
			{

				parser.Evaluate(EvaluationProperties.FromMachineRid(true));
				Assert.Fail("No error was raised.");

			}
			catch (Exception ex)
			{

				Assert.IsType<InvalidRsmlSyntax>(ex);

			}

		}

		[Fact]
		public void Evaluate_WithExpandAny_ExpandsAnyToRegex()
		{

			var parser = new RsParser("any -> \"value\"", LanguageStandard.Official25);

			var result = parser.Evaluate("some-random-rid", true);

			Assert.True(result.WasMatchFound);
			Assert.Equal("value", result.MatchValue);

		}

	}

}
