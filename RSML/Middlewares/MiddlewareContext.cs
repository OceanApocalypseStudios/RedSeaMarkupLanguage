using RSML.Analyzer.Syntax;


namespace RSML.Middlewares
{

	/// <summary>
	/// A context for middlewares.
	/// </summary>
	public readonly struct MiddlewareContext
	{

		/// <summary>
		/// Creates a middleware context.
		/// </summary>
		public MiddlewareContext(int bufferIndex, SyntaxToken[]? validatedLine, SyntaxToken[]? rawLine, string? line)
		{

			BufferIndex = bufferIndex;
			ValidatedLine = validatedLine;
			RawLine = rawLine;
			Line = line;

		}

		/// <summary>
		/// The index in the whole buffer (could be wrong by a few units).
		/// </summary>
		public int BufferIndex { get; } = -1;

		/// <summary>
		/// The amount of tokens in the validated line.
		/// </summary>
		public int ValidatedLineLength => ValidatedLine?.Length ?? 0;

		/// <summary>
		/// The validated line, as tokens.
		/// </summary>
		public SyntaxToken[]? ValidatedLine { get; } = null;

		/// <summary>
		/// The raw line, as tokens.
		/// </summary>
		public SyntaxToken[]? RawLine { get; } = null;

		/// <summary>
		/// The line, as a string.
		/// </summary>
		public string? Line { get; } = null;

	}

}
