// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
using C = OceanApocalypseStudios.RSML.Constants;

// Code analysis.
[assembly: SuppressMessage("Design", C.CA1043, Justification = "SourceLocation refers to a specific location in the source, not a span.", Scope = "member", Target = "~P:OceanApocalypseStudios.RSML.Sources.IReadOnlyBuffer.Item(OceanApocalypseStudios.RSML.Sources.SourceLocation)")]
[assembly: SuppressMessage("Design", C.CA1043, Justification = "SourceLocation refers to a specific location in the source, not a span.", Scope = "member", Target = "~P:OceanApocalypseStudios.RSML.Sources.ReadOnlySpanBuffer.Item(OceanApocalypseStudios.RSML.Sources.SourceLocation)")]
[assembly: SuppressMessage("Design", C.CA1043, Justification = "SourceLocation refers to a specific location in the source, not a span.", Scope = "member", Target = "~P:OceanApocalypseStudios.RSML.Sources.ReadOnlyStringBuffer.Item(OceanApocalypseStudios.RSML.Sources.SourceLocation)")]

// Code style.
[assembly: SuppressMessage("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.ReadOnlyStringBuffer.GetSourceSpan(System.Int32,System.Int32)~OceanApocalypseStudios.RSML.Diagnostics.Result{OceanApocalypseStudios.RSML.Sources.SourceSpan}")]
[assembly: SuppressMessage("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.ReadOnlySpanBuffer.GetSourceSpan(System.Int32,System.Int32)~OceanApocalypseStudios.RSML.Diagnostics.Result{OceanApocalypseStudios.RSML.Sources.SourceSpan}")]
[assembly: SuppressMessage("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.ReadOnlySpanBuffer.Slice(System.Int32,System.Int32)~OceanApocalypseStudios.RSML.Diagnostics.Result{System.String}")]
[assembly: SuppressMessage("Style", C.Ide0046, Justification = "The conditional expression in this case hurts readability.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.ReadOnlyStringBuffer.Slice(System.Int32,System.Int32)~OceanApocalypseStudios.RSML.Diagnostics.Result{System.String}")]
[assembly: SuppressMessage("Style", C.Ide0305, Justification = "The collection initialization in this case hurts readability and hides an allocation.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Diagnostics.DiagnosticCollector.GetAll~System.Collections.Immutable.ImmutableArray{OceanApocalypseStudios.RSML.Diagnostics.Diagnostic}")]
[assembly: SuppressMessage("Style", C.Ide0305, Justification = "The collection initialization in this case hurts readability and hides an allocation.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.ReadOnlySpanBuffer.ComputeLineStarts(System.Boolean)")]

// Issues caused by targetting more than one framework.
[assembly: SuppressMessage("Style", C.Ide0057, Justification = ".NET Framework does not have the Range type.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.ReadOnlySpanBuffer.CountUntilNotWhitespace(System.Int32)~OceanApocalypseStudios.RSML.Diagnostics.Result{System.Int32}")]
[assembly: SuppressMessage("Style", C.Ide0057, Justification = ".NET Framework does not have the Range type.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.ReadOnlySpanBuffer.CountWhile(System.Func{System.Int32,System.Char,System.Boolean},System.Int32)~OceanApocalypseStudios.RSML.Diagnostics.Result{System.Int32}")]
[assembly: SuppressMessage("Style", C.Ide0057, Justification = ".NET Framework does not have the Range type.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.ReadOnlySpanBuffer.CountUntilWhitespace(System.Int32)~OceanApocalypseStudios.RSML.Diagnostics.Result{System.Int32}")]
