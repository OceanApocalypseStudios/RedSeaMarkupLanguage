using RSML.Core.Actions;


namespace RSML.Core.Parser
{

	/// <summary>
	/// A set of properties to be passed to a RSML parser.
	/// </summary>
	public readonly struct ParserProperties
	{

		/// <inheritdoc cref="RSParser.PrimaryOperatorSymbol" />
		public required string PrimaryOperatorSymbol { get; init; }

		/// <inheritdoc cref="RSParser.SecondaryOperatorSymbol" />
		public required string SecondaryOperatorSymbol { get; init; }

		/// <inheritdoc cref="RSParser.TertiaryOperatorSymbol" />
		public required string TertiaryOperatorSymbol { get; init; }

		/// <inheritdoc cref="RSParser.SecondaryOperatorAction" />
		public required OperatorAction SecondaryOperatorAction { get; init; }

		/// <inheritdoc cref="RSParser.TertiaryOperatorAction" />
		public required OperatorAction TertiaryOperatorAction { get; init; }

	}

}
