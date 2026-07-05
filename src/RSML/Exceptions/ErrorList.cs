using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Exceptions;

/// <summary>
/// A list of RSML toolchain errors.
/// </summary>
public class ErrorList() : IEnumerable<SourceError>
{
	private readonly List<SourceError> errors = [];

	/// <summary>
	/// Adds an error to the list of errors.
	/// </summary>
	/// <param name="error"></param>
	public void Add(SourceError error) => errors.Add(error);

	/// <summary>
	/// Adds an error to the list of errors.
	/// </summary>
	/// <param name="span">The span the error relates to.</param>
	/// <param name="message">A brief description of why the error occured.</param>
	/// <param name="severity">The error severity.</param>
	public void Add(SourceSpan span, string message, Severity severity) => errors.Add(new(span, message, severity));

	/// <summary>
	/// Adds an error to the list of errors.
	/// </summary>
	/// <param name="span">The span the error relates to.</param>
	/// <param name="message">A pointer to an array of bytes that describe the error.</param>
	/// <param name="byteCount">The amount of bytes in <paramref name="message"/>.</param>
	/// <param name="messageEncoding">The error message encoding (this determines which encoding to use to decode <paramref name="message"/>).</param>
	/// <param name="severity">The error severity.</param>
	[CLSCompliant(false)]
	public unsafe void Add(SourceSpan span, byte* message, int byteCount, Encoding? messageEncoding, Severity severity) => errors.Add(new(span, (messageEncoding ?? Encoding.Default).GetString(message, byteCount), severity));

	/// <summary>
	/// Clears the <see cref="ErrorList"/>, leaving it fully empty.
	/// </summary>
	public void Clear() => errors.Clear();

	/// <inheritdoc/>
	public IEnumerator<SourceError> GetEnumerator() => errors.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
