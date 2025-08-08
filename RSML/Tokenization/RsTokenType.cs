namespace RSML.Tokenization
{

	/// <summary>
	/// A RSML token's type.
	/// </summary>
	public enum RsTokenType : byte
	{

		/// <summary>
		/// An invalid or undefined token.
		/// </summary>
		UndefinedToken,

		/// <summary>
		/// The return operator.
		/// </summary>
		ReturnOperator,

		/// <summary>
		/// The error out operator.
		/// </summary>
		ThrowErrorOperator,

		/// <summary>
		/// The system identification keyword.
		/// </summary>
		SystemName,

		/// <summary>
		/// The system architecture identification keyword.
		/// </summary>
		ArchitectureIdentifier,

		/// <summary>
		/// The major version ID token. This is version of the system, <em>unless</em>
		/// <c>defined</c> or <c>any</c> were used instead.
		/// </summary>
		MajorVersionId,

		/// <summary>
		/// The equals keyword (<c>==</c>).
		/// </summary>
		Equals,

		/// <summary>
		/// The different keyword (<c>!=</c>).
		/// </summary>
		Different,

		/// <summary>
		/// The greater-than keyword (<c>&gt;</c>).
		/// </summary>
		GreaterThan,

		/// <summary>
		/// The less-than keyword (<c>&lt;</c>).
		/// </summary>
		LessThan,

		/// <summary>
		/// The greater-than keyword (<c>&gt;=</c>).
		/// </summary>
		GreaterOrEqualsThan,

		/// <summary>
		/// The less-than or equals to keyword (<c>&lt;=</c>).
		/// </summary>
		LessOrEqualsThan,

		/// <summary>
		/// The value token. This is the argument passed to an operator.
		/// </summary>
		LogicPathValue,

		/// <summary>
		/// The <c>defined</c> keyword used to specify a version must be defined.
		/// </summary>
		DefinedKeyword,

		/// <summary>
		/// The <c>any</c> keyword.
		/// </summary>
		WildcardKeyword,

		/// <summary>
		/// The special action symbol (<c>@</c>).
		/// </summary>
		SpecialActionSymbol,

		/// <summary>
		/// The special action name.
		/// </summary>
		SpecialActionName,

		/// <summary>
		/// The special action argument.
		/// </summary>
		SpecialActionArgument,

		/// <summary>
		/// The comment symbol (<c>#</c>).
		/// </summary>
		CommentSymbol,

		/// <summary>
		/// The comment text. Refers to any ignored portion of the RSML document.
		/// </summary>
		CommentText,

		/// <summary>
		/// The end of line token.
		/// </summary>
		Eol = 254,

		/// <summary>
		/// The end of file token.
		/// </summary>
		Eof = 255

	}

}
