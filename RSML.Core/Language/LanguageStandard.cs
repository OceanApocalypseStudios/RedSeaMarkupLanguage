using System;
using System.Collections.Generic;

using RSML.Core.Actions;
using RSML.Core.Exceptions;
using RSML.Core.Parser;


namespace RSML.Core.Language
{

	/// <summary>
	/// Represents a RSML language standard.<br />
	/// Learn more about language standards here:
	/// <a href="https://oceanapocalypsestudios.github.io/rsml-docs/latest/language/standards/" />.
	/// </summary>
	public readonly struct LanguageStandard
	{

		/// <summary>
		/// Official 25 language standard, also known as official-25.
		/// </summary>
		public static LanguageStandard Official25 => new(
			"->",
			"||",
			"^!",
			(_, a) => Console.WriteLine(a),
			(_, a) => throw new UserRaisedException(a),
			[]
		);

		/// <summary>
		/// Road-Like language standard, also known as roadlike.
		/// </summary>
		public static LanguageStandard RoadLike => new(
			"???",
			"<<",
			"!!!",
			(_, a) => Console.WriteLine(a),
			(_, a) => throw new UserRaisedException(a),
			[]
		);

		/// <summary>
		/// The parser's properties.
		/// </summary>
		public ParserProperties Properties { get; init; }

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
		public LanguageStandard(string primaryOperatorSymbol,
								string secondaryOperatorSymbol,
								string tertiaryOperatorSymbol,
								OperatorAction secondaryOperatorAction,
								OperatorAction tertiaryOperatorAction,
								Dictionary<string, SpecialAction> specialActions)
		{

			Properties = new()
			{

				PrimaryOperatorSymbol = primaryOperatorSymbol,
				SecondaryOperatorSymbol = secondaryOperatorSymbol,
				TertiaryOperatorSymbol = tertiaryOperatorSymbol,

				SecondaryOperatorAction = secondaryOperatorAction,
				TertiaryOperatorAction = tertiaryOperatorAction

			};

			SpecialActions = specialActions;

		}

		/// <summary>
		/// Initializes a new language standard for RSML.
		/// </summary>
		/// <param name="properties">A set of properties for the parser</param>
		/// <param name="specialActions">The special actions</param>
		public LanguageStandard(ParserProperties properties,
								Dictionary<string, SpecialAction> specialActions)
		{

			Properties = properties;
			SpecialActions = specialActions;

		}

		/// <summary>
		/// Initializes a new language standard for RSML.
		/// </summary>
		/// <param name="properties">A set of properties for the parser</param>
		/// <param name="specialActions">The special actions, as a set of <see cref="KeyValuePair{String, SpecialAction}" /></param>
		public LanguageStandard(ParserProperties properties,
								IEnumerable<KeyValuePair<string, SpecialAction>> specialActions)
		{

			Properties = properties;
			SpecialActions = new(specialActions);

		}

	}

}
