using RSML.Language;
using RSML.Parser;


namespace RSML.Samples.Samples
{

	internal class Sample4 : ISample
	{

		public string Content =>
			"""
			# ignored
			ignored

			# line above is ignored

			.+ => "this is a comment because of the wrong operator used"
			win.+ -> "bill gates"
			any -> "anything is ok"
			.+ -> "if expanded any, this won't be reached"

			osx.+ -> "unreachable"
			""";

		public EvaluationProperties Properties => new("osx-arm64", false);

		/*
		 *
		 * Expected Output
		 * ===============
		 * - Line 1-5: Ignored
		 * - Line 6: Lack of operator (=> instead of ->), ignored
		 * - Line 7: No match, ignored
		 * - Line 8: any is not expanded, no match, ignored
		 * - Line 9: Matches all
		 * - All Other Lines: Not reached
		 *
		 */

		public EvaluationResult EvaluateSample()
		{

			RsParser parser = new(Content, LanguageStandard.Official25);

			return parser.Evaluate(Properties);

		}

	}

}
