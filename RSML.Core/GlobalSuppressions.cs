// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// COLLECTION INITIALIZATION //
[assembly: SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "<Pending>", Scope = "member", Target = "~M:RSML.Core.Parser.RSParser.InsertLineBefore(System.Int32,System.String)")]
[assembly: SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "<Pending>", Scope = "member", Target = "~P:RSML.Core.Parser.RSParser.Content")]
[assembly: SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "<Pending>", Scope = "member", Target = "~M:RSML.Core.Parser.RSParser.#ctor(System.String,RSML.Core.Parser.ParserProperties)")]
[assembly: SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "<Pending>", Scope = "member", Target = "~M:RSML.Core.Parser.RSParser.#ctor(System.String,System.String)")]
[assembly: SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "<Pending>", Scope = "member", Target = "~M:RSML.Core.Parser.RSParser.#ctor(System.String,RSML.Core.Language.LanguageStandard)")]

// CONDITIONAL EXPRESSIONS //
// (These ones specifically would make the code unreadable)
[assembly: SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "<Pending>", Scope = "member", Target = "~M:RSML.Core.Parser.RSParser.GetCommentType(System.String)~System.Nullable{RSML.Core.Language.CommentType}")]
[assembly: SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "<Pending>", Scope = "member", Target = "~M:RSML.Core.Parser.RSParser.HandleSpecialActionCall(System.String)~System.Byte")]
