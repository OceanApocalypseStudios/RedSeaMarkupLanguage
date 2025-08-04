namespace RSML.Tokenization
{

	/// <summary>
	/// A RSML token's type.
	/// </summary>
	public enum RsTokenType : byte
	{

		/// <summary>
		/// The return operator. In RSML, this is:
		/// <c>return</c>
		/// <c>returnif</c>
		/// <c>-></c>
		/// <c>=></c>
		/// </summary>
		ReturnOperator,

		/// <summary>
		/// The error out operator. In RSML, this is:
		/// <c>error</c>
		/// <c>errorif</c>
		/// <c>!></c>
		/// </summary>
		ThrowErrorOperator,

		/// <summary>
		/// The Microsoft Windows system identification keyword.
		/// </summary>
		Windows,

		/// <summary>
		/// The OSX system identification keyword.
		/// </summary>
		OSX,

		/// <summary>
		/// The Linux system identification keyword.
		/// Use this if you want to specify any Linux distro
		/// or any Linux distro with a certain kernel version.
		/// </summary>
		Linux,

		/// <summary>
		/// The FreeBSD system identification keyword.
		/// </summary>
		FreeBsd,

		/// <summary>
		/// The Linux system identification keyword for Debian-based
		/// distributions.
		/// </summary>
		Debian,

		/// <summary>
		/// The Linux system identification keyword for Arch-based
		/// distributions.
		/// </summary>
		Arch,

		/// <summary>
		/// The Linux system identification keyword for Alpine distro.
		/// </summary>
		Alpine,

		/// <summary>
		/// The Linux system identification keyword for Gentoo distro.
		/// </summary>
		Gentoo,

		/// <summary>
		/// The x64 architecture identification keyword.
		/// </summary>
		X64,

		/// <summary>
		/// The x86 architecture identification keyword.
		/// </summary>
		X86,

		/// <summary>
		/// The arm32 (or simply arm) architecture identification keyword.
		/// </summary>
		Arm32,

		/// <summary>
		/// The arm64 architecture identification keyword.
		/// </summary>
		Arm64,

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
		/// The lower-than keyword (<c>&lt;</c>).
		/// </summary>
		LowerThan,

		/// <summary>
		/// The greater-than keyword (<c>&gt;=</c>).
		/// </summary>
		GreaterOrEqualsThan,

		/// <summary>
		/// The lower-than or equals to keyword (<c>&lt;=</c>).
		/// </summary>
		LowerOrEqualsThan,

		/// <summary>
		/// The value token. This is the argument passed to an operator.
		/// </summary>
		Value,

		/// <summary>
		/// The major version ID token. This is version of the system, <em>unless</em>
		/// <c>defined</c> or <c>any</c> were used instead.
		/// </summary>
		MajorVersionId,

		/// <summary>
		/// The <c>defined</c> keyword used to specify a version must be defined.
		/// </summary>
		DefinedKeyword,

		/// <summary>
		/// The <c>any</c> keyword.
		/// </summary>
		WildcardKeyword,

		/// <summary>
		/// The special action handler (<c>@</c>).
		/// </summary>
		SpecialActionHandler,

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
