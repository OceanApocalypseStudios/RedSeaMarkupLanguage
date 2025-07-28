using System;
using System.Collections.Generic;

using RSML.Core.Parser;


namespace RSML.Tests.Parser
{

	public class RSInsertTextAtPos
	{

		[Theory]
		[InlineData("")]
		[InlineData("   ")]
		public void InsertLineAt_ThrowsOnBlankLine(string input)
		{

			RSParser parser = new("", "->");
			Assert.Throws<ArgumentException>(() => parser.InsertLineAt(1, input));

		}

		[Fact]
		public void InsertLineAt_AddsLineCorrectly_At0()
		{

			RSParser parser = new("existing", "->");
			parser.InsertLineAt(0, "inserted");

			Assert.Equal("inserted\nexisting", parser.Content);

		}

		[Fact]
		public void InsertLineAt_AddsLineCorrectly_At1()
		{

			RSParser parser = new("existing", "->");
			parser.InsertLineAt(1, "inserted");

			Assert.Equal("existing\ninserted", parser.Content);

		}

		[Fact]
		public void InsertLineBefore_Before2SameAsAt1()
		{

			RSParser parser1 = new("existing", "->");
			parser1.InsertLineAt(1, "inserted");

			RSParser parser2 = new("existing", "->");
			parser2.InsertLineBefore(2, "inserted");

			Assert.True(parser1.Content == parser2.Content &&
				parser1.Content == "existing\ninserted");

		}

		[Fact]
		public void InsertLineAfter_After0SameAsAt1()
		{

			RSParser parser1 = new("existing", "->");
			parser1.InsertLineAt(1, "inserted");

			RSParser parser2 = new("existing", "->");
			parser2.InsertLineAfter(0, "inserted");

			Assert.True(parser1.Content == parser2.Content &&
				parser1.Content == "existing\ninserted");

		}

		[Fact]
		public void InsertLineAfter_Before3SameAsAfter1()
		{

			RSParser parser1 = new("existing", "->");
			parser1.InsertLineBefore(3, "inserted");

			RSParser parser2 = new("existing", "->");
			parser2.InsertLineAfter(1, "inserted");

			Assert.True(parser1.Content == parser2.Content &&
				parser1.Content == "existing\n# \ninserted");

		}

		[Fact]
		public void InsertLineAt_PadsIfIndexIsTooHigh()
		{

			RSParser parser = new("some random text", "->");
			parser.InsertLineAt(3, "hello");

			Assert.Equal("some random text\n# \n# \nhello", parser.Content);

		}

		[Fact]
		public void RemoveExplicitComments_RemovesCommentsAndReturnsThem()
		{
			RSParser parser = new("", "->");
			parser.Content = "# First comment\nwin.+ -> \"Logic path\"\n@EndAll\n# Another comment\nSome other line";

			var comments = parser.RemoveExplicitComments();

			// Check comments returned are correct lines with correct indexes
			Assert.Equal(2, comments.Count);
			Assert.Equal("# First comment", comments[0]);
			Assert.Equal("# Another comment", comments[3]);

			// Check content no longer contains explicit comments
			Assert.DoesNotContain("# First comment", parser.Content);
			Assert.DoesNotContain("# Another comment", parser.Content);

			// Other lines stay
			Assert.Contains("win.+ -> \"Logic path\"", parser.Content);
			Assert.Contains("@EndAll", parser.Content);
			Assert.Contains("Some other line", parser.Content);
		}

		[Fact]
		public void InsertLines_InsertsLinesAtCorrectIndexes()
		{
			RSParser parser = new("", "->");
			parser.Content = "Line 1\nLine 2\nLine 3";

			Dictionary<int, string> linesToInsert = new()
			{

				[1] = "# Inserted comment before Line 2",
				[3] = "# Inserted comment before Line 4"

			};

			parser.InsertLines(linesToInsert);

			Assert.Equal("# Inserted comment before Line 2", parser.Content.Split('\n')[1]);
			Assert.Equal("# Inserted comment before Line 4", parser.Content.Split('\n')[3]);
		}

		[Fact]
		public void RemoveExplicitComments_NoComments_DoesNothing()
		{
			RSParser parser = new("", "->");
			parser.Content = "No comments here\nJust lines\nLogic stuff";

			var comments = parser.RemoveExplicitComments();

			Assert.Empty(comments);
			Assert.Equal(3, parser.Lines);
		}

		[Fact]
		public void InsertLines_EmptyDictionary_DoesNothing()
		{

			RSParser parser = new("Line 1\nLine 2", "->");
			parser.InsertLines(new Dictionary<int, string>());

			Assert.Equal(2, parser.Lines);
			Assert.Equal("Line 1\nLine 2", parser.Content);

		}

	}

}

