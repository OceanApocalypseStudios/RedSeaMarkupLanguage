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
[assembly: SuppressMessage("Style", C.Ide0305, Justification = "The collection initialization in this case hurts readability and hides an allocation.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Diagnostics.DiagnosticCollector.GetAll~System.Collections.Immutable.ImmutableArray{OceanApocalypseStudios.RSML.Diagnostics.Diagnostic}")]
[assembly: SuppressMessage("Style", C.Ide0305, Justification = "The collection initialization in this case hurts readability and hides an allocation.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.ReadOnlySpanBuffer.ComputeLineStarts(System.Boolean)")]
[assembly: SuppressMessage("Style", C.Ide0305, Justification = "The collection initialization in this case hurts readability and hides an allocation.", Scope = "member", Target = "~M:OceanApocalypseStudios.RSML.Sources.ReadOnlyStringBuffer.ComputeLineStarts(System.Boolean)")]

// Issues caused by targetting more than one framework.
