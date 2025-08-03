using RSML.Language;
using RSML.Trimming;


namespace RSML.Tests.Trimming
{

	public class TrimmerTests
	{

		[Theory]
		[InlineData(
			"# A comment\nwin.+ -> \"Logic Path\"\nimplicit comment\n@EndAll\nosx.+ -> \"this will be kept\"",
			"win.+->\"Logic Path\"\n@EndAll\nosx.+->\"this will be kept\"\n"
		)]
		[InlineData(
			"# A comment\nwin.+ -> \"Logic Path\"\nimplicit comment\n@EndAll\nosx.+ -> \"this will be kept\"\n",
			"win.+->\"Logic Path\"\n@EndAll\nosx.+->\"this will be kept\"\n"
		)]
		[InlineData(
			"# A comment\r\n# Another comment\r\n# Too many comments\r\nimplicit comment\r\n",
			""
		)]
		public void SoftTrimmer_TrimsCorrectly(string input, string expected)
		{

			SoftTrimmer trimmer = new();
			string trimmed = trimmer.Trim(input, LanguageStandard.Official25);

			Assert.Equal(expected, trimmed.ReplaceLineEndings("\n")); // line ending normalization so tests pass in all systems

		}

	}

}
