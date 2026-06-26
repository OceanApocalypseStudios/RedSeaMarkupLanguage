using System.Linq;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Host;


namespace OceanApocalypseStudios.RSML.Tests
{

	public class StructureTests
	{

		[Fact]
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

				Assert.True(token == line[++x]);
				Assert.True(token == line.ElementAt(x));

			}

		}

		[Fact]
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

			Assert.True(expected.Equals(actual));

		}

		[Fact]
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

			Assert.Equal(expected, actual);

		}

	}

}
