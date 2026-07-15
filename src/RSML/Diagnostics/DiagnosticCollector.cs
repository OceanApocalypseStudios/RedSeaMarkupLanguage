using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;


namespace OceanApocalypseStudios.RSML.Diagnostics;

/// <summary>
/// A list of RSML toolchain errors.
/// </summary>
public sealed record DiagnosticCollector() : IEnumerable<Diagnostic>
{
	private readonly List<Diagnostic> diagnostics = [];

	/// <summary>
	/// Adds an error to the list of errors.
	/// </summary>
	/// <param name="diagnostic"></param>
	public DiagnosticCollector Add(Diagnostic diagnostic)
	{
		diagnostics.Add(diagnostic);
		return this;
	}

	/// <summary>
	/// Clears the <see cref="DiagnosticCollector"/>, leaving it fully empty.
	/// </summary>
	public DiagnosticCollector Clear()
	{
		diagnostics.Clear();
		return this;
	}

	/// <summary>
	/// Returns all
	/// </summary>
	/// <returns></returns>
	public ImmutableArray<Diagnostic> GetAll() => diagnostics.ToImmutableArray();

	/// <inheritdoc/>
	public IEnumerator<Diagnostic> GetEnumerator() => diagnostics.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
