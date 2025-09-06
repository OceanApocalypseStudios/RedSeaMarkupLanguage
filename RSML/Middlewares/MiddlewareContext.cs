using System;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;


namespace OceanApocalypseStudios.RSML.Middlewares
{

	/// <summary>
	/// A context for middlewares.
	/// </summary>
	public readonly struct MiddlewareContext
	{

		/// <summary>
		/// Creates a middleware context.
		/// </summary>
		public MiddlewareContext(int bufferIndex, SyntaxLine validatedLine, string? line)
		{

			BufferIndex = bufferIndex;
			ValidatedLine = validatedLine;
			Line = line?.AsMemory() ?? ReadOnlyMemory<char>.Empty;

		}

		/// <summary>
		/// Creates a middleware context.
		/// </summary>
		public MiddlewareContext(int bufferIndex, SyntaxLine validatedLine, ReadOnlyMemory<char> line)
		{

			BufferIndex = bufferIndex;
			ValidatedLine = validatedLine;
			Line = line;

		}

		/// <summary>
		/// The index in the whole buffer (could be wrong by a few units).
		/// </summary>
		public int BufferIndex { get; } = -1;

		/// <summary>
		/// The amount of tokens in the validated line.
		/// </summary>
		public int ValidatedLineLength => ValidatedLine.Length;

		/// <summary>
		/// The validated line, in tokens.
		/// </summary>
		public SyntaxLine ValidatedLine { get; } = new();

		/// <summary>
		/// The line, as a string.
		/// </summary>
		public ReadOnlyMemory<char> Line { get; }

	}

}
