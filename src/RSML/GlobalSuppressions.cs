// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using SM = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute; // SUPPRESS MESSAGE
using C = OceanApocalypseStudios.RSML.InternalUtils;


// Code style.
[assembly: SM("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.CharSpanExtensions.GetCharAt(System.Char[],System.Int32)~System.Char")]
[assembly: SM("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.CharSpanExtensions.GetCharAt(System.ReadOnlySpan{System.Char},System.Int32)~System.Char")]
[assembly: SM("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.CharSpanExtensions.GetCharAt(System.Span{System.Char},System.Int32)~System.Char")]
[assembly: SM("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.CharSpanExtensions.GetCharAt(System.String,System.Int32)~System.Char")]
[assembly: SM("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.Buffers.ReadOnlyStringBuffer.GetLineSeparatorBefore(System.Int32,System.Int32@)~System.Int32")]

// Issues caused by targetting more than one framework.
// [assembly: SM("Style", C.Ide0056, Justification = ".NET Standard 2.0 does not support the use of Index.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.Buffers.StringBuffer.GetLineSeparatorBefore(System.Int32, System.Int32)~System.Int32")]
