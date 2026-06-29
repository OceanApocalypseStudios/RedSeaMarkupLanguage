using System.Linq;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Host;


namespace OceanApocalypseStudios.RSML.Tests
{

	[TestClass]
	public class StructureTests
	{

		[TestMethod]
		public void SyntaxLine_Compact1()
		{

			SyntaxLine expected = new(
				new(TokenKind.SystemName, 0, 2),
				new(TokenKind.SpecialActionArgument, 37, 44),
				new(TokenKind.ProcessorArchitecture, 11, 13),
				new(TokenKind.Integer, 14, 19),
				new(TokenKind.DefinedKeyword, 4, 9),
				SyntaxToken.Empty,
				SyntaxToken.Empty,
				SyntaxToken.Empty
			);

			SyntaxLine actual = new(
				SyntaxToken.Empty,
				new(TokenKind.SystemName, 0, 2),
				new(TokenKind.SpecialActionArgument, 37, 44),
				SyntaxToken.Empty,
				new(TokenKind.ProcessorArchitecture, 11, 13),
				new(TokenKind.Integer, 14, 19),
				SyntaxToken.Empty,
				new(TokenKind.DefinedKeyword, 4, 9)
			);

			actual.Compact();

			Assert.IsTrue(expected == actual);

		}

		[TestMethod]
		public void SyntaxLine_Compact2()
		{

			SyntaxLine expected = new(
				new(TokenKind.SpecialActionArgument, 37, 44),
				new(TokenKind.DefinedKeyword, 4, 9),
				new(TokenKind.Integer, 14, 19),
				SyntaxToken.Empty,
				SyntaxToken.Empty,
				SyntaxToken.Empty,
				SyntaxToken.Empty,
				SyntaxToken.Empty
			);

			SyntaxLine actual = new(
				SyntaxToken.Empty,
				SyntaxToken.Empty,
				new(TokenKind.SpecialActionArgument, 37, 44),
				SyntaxToken.Empty,
				SyntaxToken.Empty,
				SyntaxToken.Empty,
				new(TokenKind.DefinedKeyword, 4, 9),
				new(TokenKind.Integer, 14, 19)
			);

			actual.Compact();

			Assert.IsTrue(expected == actual);

		}

		[TestMethod]
		public void SyntaxLine_GetEnumerator()
		{

			int x = -1;
			SyntaxLine line = new(
				new(TokenKind.SystemName, 0, 2),
				new(TokenKind.SystemName, 4, 9),
				new(TokenKind.SystemName, 11, 13),
				new(TokenKind.SystemName, 14, 19),
				new(TokenKind.SystemName, 22, 25),
				new(TokenKind.SystemName, 37, 44),
				SyntaxToken.Empty,
				SyntaxToken.Empty
			);

			foreach (var token in line)
			{

				Assert.IsTrue(token == line[++x]);
				Assert.IsTrue(token == line.ElementAt(x));

			}

		}

		[TestMethod]
		public void HostInfo_FromJson()
		{

			string jsonString =
				"""
				{
					"SystemName": "linux",
					"SystemVersion": 22,
					"ProcessorArchitecture": "x86",
					"DistroName": "ubuntu",
					"DistroFamily": "debian"
				}
				""";

			var actual = HostInfoConverter.FromJson(jsonString);
			HostInfo expected = new("ubuntu", "debian", "x86", 22);

			Assert.IsTrue(expected.Equals(actual));

		}

		[TestMethod]
		public void HostInfo_ToJson()
		{

			string expected =
				"""
				{
					"SystemName": "linux",
					"SystemVersion": 22,
					"DistroName": "ubuntu",
					"DistroFamily": "debian",
					"ProcessorArchitecture": "x86"
				}
				""";

			HostInfo hostInfo = new("ubuntu", "debian", "x86", 22);
			var actual = HostInfoConverter.ToJson(hostInfo);

			Assert.AreEqual(expected, actual);

		}

	}

}
