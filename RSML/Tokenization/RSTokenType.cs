namespace RSML.Tokenization
{

	/// <summary>
	/// A RSMl token's type.
	/// </summary>
	public enum RsTokenType : byte
	{

		/// <summary>
		/// Primary operator.
		/// </summary>
		PrimaryOperator = 0,

		/// <summary>
		/// Secondary operator.
		/// </summary>
		SecondaryOperator,

		/// <summary>
		/// Tertiary operator.
		/// </summary>
		TertiaryOperator,

		/// <summary>
		/// Special action handler (@).
		/// </summary>
		SpecialActionHandler,

		/// <summary>
		/// Logic path's left side.
		/// </summary>
		LogicPathLeft,

		/// <summary>
		/// Logic path's right side.
		/// </summary>
		LogicPathRight,

		/// <summary>
		/// Special action's name.
		/// </summary>
		SpecialActionName,

		/// <summary>
		/// Special action's argument.
		/// </summary>
		SpecialActionArgument,

		/// <summary>
		/// Explicit comment start (#).
		/// </summary>
		CommentStart,

		/// <summary>
		/// Ignored text.
		/// </summary>
		CommentText,

		/// <summary>
		/// End of line.
		/// </summary>
		EOL = 254,

		/// <summary>
		/// End of file.
		/// </summary>
		EOF = 255

	}

}
