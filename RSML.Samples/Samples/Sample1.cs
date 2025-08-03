using RSML.Language;
using RSML.Parser;


namespace RSML.Samples.Samples
{

	internal class Sample1 : ISample
	{

		public string Content =>
			"""
			(ubuntu|debian).+ -> "Debian-based"
			@EndAll
			win.+ -> "windows"
			osx.+ -> "this won't be reached obviously"
			# comment
			implicit comment

			# whitespace comment above
			""";

		public EvaluationProperties Properties => new("win-x64", false);

		/*
		 *
		 * Expected Output
		 * ===============
		 * - Line 1: No match, ignored
		 * - Line 2: Ends all
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
