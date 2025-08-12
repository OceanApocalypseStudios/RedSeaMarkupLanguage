using System.Diagnostics;
using System.Linq;

using RSML.Analyzer.Syntax;
using RSML.Evaluation;
using RSML.Machine;
using RSML.Middlewares;


namespace RSML.Tests
{

	public class MiddlewareTests
	{

		private static readonly LocalMachine win10X64 = new("windows", "x64", 10);

		[Fact]
		public void Middleware_RunsInCorrectPlace_TwoLength()
		{

			Evaluator evaluator = new("-> ubuntu \"Testing\"\n# Yeah, I'm just testing\n# Just messing around\n@EndAll\n-> \"yup :)\"");

			// ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
			var result = evaluator.BindMiddleware(
									  MiddlewareRunnerLocation.TwoLength, tokens =>
									  {

										  Debug.WriteLine("Middleware TwoLength was ran!");
										  Assert.Equal(TokenKind.CommentSymbol, tokens.ToArray()[0].Kind);

										  return MiddlewareResult.ContinueEvaluation;

									  }
								  )
								  .Evaluate(win10X64);

			Assert.Null(result.MatchValue);

		}

		[Fact]
		public void Middleware_RunsInCorrectPlace_FiveLength()
		{

			Evaluator evaluator = new("-> ubuntu \"Testing\"\n# Yeah, I'm just testing\n# Just messing around\n@EndAll\n-> \"yup :)\"");

			// ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
			var result = evaluator.BindMiddleware(
									  MiddlewareRunnerLocation.FiveLengthBeforeHandling, tokens =>
									  {

										  Debug.WriteLine("Middleware FiveLength was ran!");
										  Assert.Equal(5, tokens.Count());

										  return MiddlewareResult.ContinueEvaluation;

									  }
								  )
								  .Evaluate(win10X64);

			Assert.Null(result.MatchValue);

		}

	}

}
