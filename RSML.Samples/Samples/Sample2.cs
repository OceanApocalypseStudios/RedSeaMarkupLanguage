using RSML.Language;
using RSML.Parser;


namespace RSML.Samples.Samples
{

	internal class Sample2 : ISample
	{

		public string Content => """
			# i have a lot of comments here
			this should be ignored
			(arch|ubuntu|debian).+ -> "tux is happy!"
			osx.* -> "fine"
			any -> "anything is fine"
			""";

		public EvaluationProperties Properties => new("debian-x86", true);

		/*
		 * 
		 * Expected Output
		 * ===============
		 * - Line 1: Ignored
		 * - Line 2: Ignored
		 * - Line 3: Match found, ends here
		 * - All Other Lines: Not reached
		 *
		 */

		public EvaluationResult EvaluateSample() => new RSParser(Content, LanguageStandard.Official25).Evaluate(Properties);

	}

}
