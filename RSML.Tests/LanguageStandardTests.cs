using System.Collections.Generic;

using RSML.Actions;
using RSML.Language;


namespace RSML.Tests
{

	public class LanguageStandardTests
	{

		[Fact]
		public void Official25_HasCorrectOperatorSymbols()
		{

			var standard = LanguageStandard.Official25;

			Assert.Equal("->", standard.PrimaryOperatorSymbol.ToString());
			Assert.Equal("||", standard.SecondaryOperatorSymbol.ToString());
			Assert.Equal("^!", standard.TertiaryOperatorSymbol.ToString());

		}

		[Fact]
		public void RoadLike_HasCorrectOperatorSymbols()
		{

			var standard = LanguageStandard.RoadLike;

			Assert.Equal("???", standard.PrimaryOperatorSymbol.ToString());
			Assert.Equal("<<", standard.SecondaryOperatorSymbol.ToString());
			Assert.Equal("!!!", standard.TertiaryOperatorSymbol.ToString());

		}

		[Fact]
		public void Constructor_InitializesCorrectly()
		{

			string primaryOperatorSymbol = ">";
			string secondaryOperatorSymbol = ">>";
			string tertiaryOperatorSymbol = ">>>";
			OperatorAction secondaryOperatorAction = (_, _) => { };
			OperatorAction tertiaryOperatorAction = (_, _) => { };

			Dictionary<string, SpecialAction> actions = new([ new("Test", (_, _) => 0) ]);

			LanguageStandard standard = new(
				primaryOperatorSymbol, secondaryOperatorSymbol, tertiaryOperatorSymbol,
				secondaryOperatorAction, tertiaryOperatorAction,
				actions
			);

			Assert.Equal(">", standard.PrimaryOperatorSymbol.ToString());
			_ = Assert.Single(standard.SpecialActions);

		}

	}

}
