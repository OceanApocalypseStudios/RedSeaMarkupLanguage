namespace OceanApocalypse.RSML.Language.Lexing.Tokens;

/// <summary>
/// Represents a specific kind of token.
/// </summary>
public enum TokenKind
{
	/// <summary>
	/// An unknown token kind.
	/// </summary>
	Unknown,

	/// <summary>
	/// The EOF token kind. Represents the end of a source.
	/// </summary>
	Eof,

	/// <summary>
	/// The name of a variable, constant, function or type.
	/// </summary>
	Identifier,

	/// <summary>
	/// A numeric literal.
	/// </summary>
	Number,

	/// <summary>
	/// A string literal.
	/// </summary>
	StringLiteral,

	/// <summary>
	/// A built-in constant.
	/// </summary>
	SystemConstant,

	/// <summary>
	/// The return keyword. Stops execution of the current scope with a given value.
	/// </summary>
	Return,

	/// <summary>
	/// The if keyword. Conditionalizes a statement into running only if the condition is met.
	/// </summary>
	If,

	/// <summary>
	/// The requires keyword. Indicates extensions the file depends on.
	/// </summary>
	Requires,

	/// <summary>
	/// The end keyword. Ends the file.
	/// </summary>
	End,

	/// <summary>
	/// The previous keyword. Modifies end into closing the previous region instead.
	/// </summary>
	PreviousModifier,

	/// <summary>
	/// The region keyword. Creates a conditionalized region.
	/// </summary>
	Region,

	/// <summary>
	/// The let keyword. Declares and assigns a constant.
	/// </summary>
	Let,

	/// <summary>
	/// The mut keyword. Modifies let into creating a variable instead.
	/// </summary>
	MutableModifier,

	/// <summary>
	/// The fn keyword. Modifies let into creating a function instead.
	/// </summary>
	FunctionModifier,

	/// <summary>
	/// The type keyword. Creates a type.
	/// </summary>
	Type,

	/// <summary>
	/// The as keyword.
	/// </summary>
	As,

	/// <summary>
	/// The struct keyword. Used with type and as to create a struct type.
	/// </summary>
	Struct,

	/// <summary>
	/// The assignment operator (=).
	/// </summary>
	Assignment,

	/// <summary>
	/// The equality operator (==).
	/// </summary>
	Equality,

	/// <summary>
	/// The inequality operator (!=).
	/// </summary>
	Inequality,

	/// <summary>
	/// The greater-than operator (>).
	/// </summary>
	GreaterThan,

	/// <summary>
	/// The less-than operator (&lt;).
	/// </summary>
	LessThan,

	/// <summary>
	/// The greater-than-or-equal-to operator (>=).
	/// </summary>
	GreaterThanOrEqualTo,

	/// <summary>
	/// The less-than-or-equal-to operator (&lt;=).
	/// </summary>
	LessThanOrEqualTo,

	/// <summary>
	/// The colon (:).
	/// </summary>
	Colon,

	/// <summary>
	/// The comma (,).
	/// </summary>
	Comma,

	/// <summary>
	/// The semicolon (;).
	/// </summary>
	Semicolon,

	/// <summary>
	/// The plus sign, used for sum (+).
	/// </summary>
	Plus,

	/// <summary>
	/// The hyphen (minus sign), used for subtraction (-).
	/// </summary>
	Minus,

	/// <summary>
	/// The star, used for multiplication (*).
	/// </summary>
	Star,

	/// <summary>
	/// The slash used for division (/).
	/// </summary>
	Slash,

	/// <summary>
	/// The open brace ({).
	/// </summary>
	OpenBrace,

	/// <summary>
	/// The closed brace (}).
	/// </summary>
	CloseBrace,

	/// <summary>
	/// The open parenthesis.
	/// </summary>
	OpenParenthesis,

	/// <summary>
	/// The closed parenthesis.
	/// </summary>
	CloseParenthesis,

	/// <summary>
	/// The member access mark (<c>.</c>), which is a dot.
	/// </summary>
	MemberAccess
}
