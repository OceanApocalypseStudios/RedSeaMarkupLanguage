using System;
using System.Collections.Generic;
using System.Text;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;


namespace OceanApocalypseStudios.RSML.Tests
{

	[TestClass]
	public class LexerTests
	{

		[TestMethod]
		public void Lexer_TokenizeLine_EolIfBufferConsumed()
		{

			DualTextBuffer buffer = new("  ");
			var line = Lexer.TokenizeLine(buffer);

			Assert.AreEqual(1, line.Length);
			Assert.AreEqual(TokenKind.Eol, line[0].Kind);
			Assert.IsTrue(line[0].IsOffLimits);

		}

		[TestMethod]
		[DataRow("#", "")]
		[DataRow("# ", " ")]
		[DataRow("# UselessText", " UselessText")]
		[DataRow("    # Useless text", " Useless text")]
		[DataRow("#     Useless text", "     Useless text")]
		[DataRow("    #", "")]
		[DataRow("    # ", " ")]
		[DataRow("    #Useless text", "Useless text")]
		public void Lexer_TokenizeLine_CommentBehavior(string comment, string expectedCommentText)
		{

			DualTextBuffer buffer = new(comment);
			var line = Lexer.TokenizeLine(buffer);

			Assert.AreEqual(3, line.Length);

			Assert.AreEqual("#", comment[line[0].BufferRange]);
			Assert.AreEqual(TokenKind.CommentSymbol, line[0].Kind);

			Assert.AreEqual(expectedCommentText, comment[line[1].BufferRange]);
			Assert.AreEqual(TokenKind.CommentText, line[1].Kind);

			Assert.IsTrue(line[2].IsOffLimits);
			Assert.AreEqual(TokenKind.Eol, line[2].Kind);

		}

		[TestMethod]
		[DataRow("@", "", "")]
		[DataRow("@Name", "Name", "")]
		[DataRow(" @Name Argument", "Name", "Argument")]
		[DataRow("  @Name     Argument1 Argument2", "Name", "Argument1")]
		[DataRow("    @Name  Argument1Argument2  Argument3  Argument4 Argument5", "Name", "Argument1Argument2")]
		public void Lexer_TokenizeLine_SpecialActionBehavior(string content, string expectedActionName, string expectedActionArgument)
		{

			DualTextBuffer buffer = new(content);
			var line = Lexer.TokenizeLine(buffer);

			Assert.AreEqual(4, line.Length);

			Assert.AreEqual("@", content[line[0].BufferRange]);
			Assert.AreEqual(TokenKind.SpecialActionSymbol, line[0].Kind);

			Assert.AreEqual(expectedActionName, content[line[1].BufferRange]);
			Assert.AreEqual(TokenKind.SpecialActionName, line[1].Kind);

			Assert.AreEqual(expectedActionArgument, content[line[2].BufferRange]);
			Assert.AreEqual(TokenKind.SpecialActionArgument, line[2].Kind);

			Assert.IsTrue(line[3].IsOffLimits);
			Assert.AreEqual(TokenKind.Eol, line[3].Kind);

		}

		[TestMethod]
		[DataRow(" ->", TokenKind.ReturnOperator, "->")]
		[DataRow(" !>", TokenKind.ThrowErrorOperator, "!>")]
		[DataRow(" N>", TokenKind.ReverseReturnOperator, "N>")]
		[DataRow("???", TokenKind.None, "")]
		public void Lexer_TokenizeLine_OperatorBehavior(string content, TokenKind kind, string expectedOperator)
		{

			DualTextBuffer buffer = new(content);
			var line = Lexer.TokenizeLine(buffer);

			Assert.AreEqual(2, line.Length);

			Assert.AreEqual(expectedOperator, line[0].IsOffLimits ? "" : content[line[0].BufferRange]);
			Assert.AreEqual(kind, line[0].Kind);

			Assert.IsTrue(line[1].IsOffLimits);
			Assert.AreEqual(TokenKind.Eol, line[1].Kind);

		}

	}

}
