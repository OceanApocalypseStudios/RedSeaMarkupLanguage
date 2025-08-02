using RSML.Language;
using RSML.Parser;


namespace RSML.Samples.Samples
{

	internal class Sample5 : ISample
	{

		public string Content => """
			# ignored
			ignored

			# line above is ignored

			any || "This one is made to demonstrate the errors and shit ig"
			any || "precisely what the lack of double quotes does"
			any -> this should error out
			any -> "but will it tho???"
			""";

		public EvaluationProperties Properties => new("does-not-matter", true);

		/*
		 * 
		 * Expected Output
		 * ===============
		 * - Line 1-5: Ignored
		 * - Line 6-7: Match all, runs secondary action.
		 * - Line 8: Errors out
		 * - All Other Lines: Not reached
		 *
		 */

		public EvaluationResult EvaluateSample() => new RSParser(Content, LanguageStandard.Official25).Evaluate(Properties);

	}

}
