using System;
using System.Collections.Generic;

using RSML.Actions;
using RSML.Exceptions;


namespace RSML.Language
{

	/// <summary>
	/// Represents a RSML language standard.<br />
	/// Learn more about language standards here:
	/// <a href="https://oceanapocalypsestudios.github.io/rsml-docs/latest/language/standards/" />.
	/// </summary>
	public ref struct LanguageStandard
	{

		/// <summary>
		/// Official 25 language standard, also known as official-25.
		/// </summary>
		public static LanguageStandard Official25 => new(
			"->",
			"||",
			"^!",
			(_, a) => {
				foreach (var c in a)
					Console.Write(c);

				Console.WriteLine();
			},
			(_, a) => throw new UserRaisedException(a.ToString()),
			[]
		);

		/// <summary>
		/// Road-Like language standard, also known as roadlike.
		/// </summary>
		public static LanguageStandard RoadLike => new(
			"???",
			"<<",
			"!!!",
			(_, a) => {
				foreach (var c in a)
					Console.Write(c);

				Console.WriteLine();
			},
			(_, a) => throw new UserRaisedException(a.ToString()),
			[]
		);

		/// <summary>
		/// The primary operator's symbol.
		/// </summary>
		public ReadOnlySpan<char> PrimaryOperatorSymbol { get; set; }

		/// <summary>
		/// The secondary operator's symbol.
		/// </summary>
		public ReadOnlySpan<char> SecondaryOperatorSymbol { get; set; }

		/// <summary>
		/// The tertiary operator's symbol.
		/// </summary>
		public ReadOnlySpan<char> TertiaryOperatorSymbol { get; set; }

		/// <summary>
		/// The secondary operator's action.
		/// </summary>
		public OperatorAction SecondaryOperatorAction { get; set; }

		/// <summary>
		/// The tertiary operator's action.
		/// </summary>
		public OperatorAction TertiaryOperatorAction { get; set; }

		/// <summary>
		/// The special actions this standard defines.
		/// </summary>
		public Dictionary<string, SpecialAction> SpecialActions { get; init; } = [];

		/// <summary>
		/// Initializes a new language standard for RSML.
		/// </summary>
		/// <param name="primaryOperatorSymbol">The symbol for the primary operator</param>
		/// <param name="secondaryOperatorSymbol">The symbol for the secondary operator</param>
		/// <param name="tertiaryOperatorSymbol">The symbol for the tertiary operator</param>
		/// <param name="secondaryOperatorAction">The action for the secondary operator</param>
		/// <param name="tertiaryOperatorAction">The action for the tertiary operator</param>
		/// <param name="specialActions">The special actions</param>
		public LanguageStandard(ReadOnlySpan<char> primaryOperatorSymbol,
								ReadOnlySpan<char> secondaryOperatorSymbol,
								ReadOnlySpan<char> tertiaryOperatorSymbol,
								OperatorAction secondaryOperatorAction,
								OperatorAction tertiaryOperatorAction,
								Dictionary<string, SpecialAction> specialActions)
		{

			PrimaryOperatorSymbol = primaryOperatorSymbol;
			SecondaryOperatorSymbol = secondaryOperatorSymbol;
			TertiaryOperatorSymbol = tertiaryOperatorSymbol;

			SecondaryOperatorAction = secondaryOperatorAction;
			TertiaryOperatorAction = tertiaryOperatorAction;

			SpecialActions = specialActions;

		}

	}

}
