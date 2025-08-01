using System;


namespace RSML.Language
{

	/// <summary>
	/// Comment types. You can use these as flags:
	/// <code>CommentType.Explicit | CommentType.Implicit</code> stands for both explicit
	/// AND implicit comments.
	/// </summary>
	[Flags]
	public enum CommentType : byte
	{

		/// <summary>
		/// Explicit comments are prefixed with <c>#</c> and must appear at the very
		/// start of the line, not being valid if they appear after other characters in the
		/// same line.
		/// </summary>
		Explicit = 1 << 0,

		/// <summary>
		/// Implicit comments are any lines that do not verify at least one of the
		/// following conditions:
		/// <list type="bullet">Contains operators</list>
		/// <list type="bullet">Contains the <c>@</c> character at the start</list>
		/// <list type="bullet">Does not start with <c>#</c> (as that makes the comment explicit)</list>
		/// </summary>
		Implicit = 1 << 1,

		/// <summary>
		/// Whitespace comments are a subset of implicit comments that only contain whitespace.
		/// </summary>
		Whitespace = 1 << 2

	}

}
