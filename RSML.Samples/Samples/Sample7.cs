using RSML.Language;
using RSML.Parser;


namespace RSML.Samples.Samples
{

	internal class Sample7 : ISample
	{

		public string Content => """
			# ignored
			ignored

			# line above is ignored

			# this throws errors fun fact lol
			any ^! "Error out biatch"
			""";

		public EvaluationProperties Properties => new("does-not-matter", true);

		/*
		 * 
		 * Expected Output
		 * ===============
		 * - Line 1-6: Ignored
		 * - Line 7: Match all, runs tertiary action
		 * - All Other Lines: Not reached
		 *
		 * Note
		 * ====
		 * This works in both Official25 and Roadlike.
		 *
		 */

		public EvaluationResult EvaluateSample() => new RSParser(Content, LanguageStandard.Official25).Evaluate(Properties);

	}

}
