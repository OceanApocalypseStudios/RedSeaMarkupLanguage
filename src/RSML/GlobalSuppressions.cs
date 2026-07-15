// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
using C = OceanApocalypseStudios.RSML.Constants;

// Code style.
[assembly: SuppressMessage("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.CharSpanExtensions.GetCharAt(System.Char[],System.Int32)~System.Char")]
[assembly: SuppressMessage("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.CharSpanExtensions.GetCharAt(System.ReadOnlySpan{System.Char},System.Int32)~System.Char")]
[assembly: SuppressMessage("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.CharSpanExtensions.GetCharAt(System.Span{System.Char},System.Int32)~System.Char")]
[assembly: SuppressMessage("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.CharSpanExtensions.GetCharAt(System.String,System.Int32)~System.Char")]
[assembly: SuppressMessage("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.CharSpanExtensions.<G>$7E0B8C73485617FC5155886DD83A182C.GetCharAt(System.Int32)~System.Char")]
[assembly: SuppressMessage("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.ReadOnlyStringBuffer.Slice(System.Int32,System.Int32)~OceanApocalypseStudios.RSML.Diagnostics.Result{System.Char[]}")]
[assembly: SuppressMessage("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.ReadOnlyStringBuffer.GetSourceSpan(System.Int32,System.Int32)~OceanApocalypseStudios.RSML.Diagnostics.Result{OceanApocalypseStudios.RSML.Sources.SourceSpan}")]
[assembly: SuppressMessage("Style", C.Ide0305, Justification = "The collection initialization in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Diagnostics.DiagnosticCollector.GetAll~System.Collections.Immutable.ImmutableArray{OceanApocalypseStudios.RSML.Diagnostics.Diagnostic}")]

// Issues caused by targetting more than one framework.
// [assembly: SM("Style", C.Ide0056, Justification = ".NET Standard 2.0 does not support the use of Index.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.Buffers.StringBuffer.GetLineSeparatorBefore(System.Int32, System.Int32)~System.Int32")]
