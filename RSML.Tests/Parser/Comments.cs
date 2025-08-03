using System.Diagnostics.CodeAnalysis;

using RSML.Language;
using RSML.Parser;


namespace RSML.Tests.Parser
{

	[SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "<Pending>")]
	public partial class RsParserTests
	{

		private const string SampleContent = """
											 test -> "value"
											 @SpecialAction arg
											 # Comment
											 """;

		[Fact]
		public void GetCommentType_ReturnsCorrectType()
		{

			var parser = new RsParser(SampleContent, LanguageStandard.Official25);

			Assert.Equal(CommentType.Explicit, parser.GetCommentType("# comment"));
			Assert.Equal(CommentType.Whitespace, parser.GetCommentType(""));
			Assert.Equal(CommentType.Whitespace, parser.GetCommentType("   "));
			Assert.Equal(CommentType.Implicit, parser.GetCommentType("just some text"));
			Assert.Null(parser.GetCommentType("test -> \"value\""));

		}

	}

}
