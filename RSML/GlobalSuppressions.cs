// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// COLLECTION INITIALIZATION //
[assembly: SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "<Pending>", Scope = "member", Target = "~P:RSML.Parser.RSParser.Content")]
[assembly: SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "<Pending>", Scope = "member", Target = "~M:RSML.Parser.RSParser.#ctor(System.String,RSML.Language.LanguageStandard)")]

// CONDITIONAL EXPRESSIONS //
// (These ones specifically would make the code unreadable)
[assembly: SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "<Pending>", Scope = "member", Target = "~M:RSML.Parser.RSParser.Evaluate(RSML.Parser.EvaluationProperties)~RSML.Parser.EvaluationResult")]
[assembly: SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "<Pending>", Scope = "member", Target = "~M:RSML.Parser.RSParser.HandleSpecialActionCall(System.ReadOnlySpan{System.Char},System.ReadOnlySpan{System.Char})~System.Byte")]
[assembly: SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "<Pending>", Scope = "member", Target = "~M:RSML.Parser.RSParser.GetCommentType(System.ReadOnlySpan{System.Char})~System.Nullable{RSML.Language.CommentType}")]
