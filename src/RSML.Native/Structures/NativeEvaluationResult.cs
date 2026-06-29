using System;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

using OceanApocalypseStudios.RSML.Evaluation;


namespace OceanApocalypseStudios.RSML.Native.Structures
{

	/// <summary>
	/// The result of a native evaluation process.
	/// </summary>
	[NoReorder]
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct NativeEvaluationResult
	{

		/// <summary>
		/// Whether a match was found. Boolean.
		/// </summary>
		public readonly byte wasMatchFound = 0;

		/// <summary>
		/// The start index for the match value.
		/// </summary>
		public readonly int matchValueStart = -1;

		/// <summary>
		/// The end index for the match value.
		/// </summary>
		public readonly int matchValueEnd = -1;

		/// <summary>
		/// Creates a <see cref="NativeEvaluationResult" /> given an <see cref="EvaluationResult"/>
		/// and a <see cref="ReadOnlySpan{Char}"/>.
		/// </summary>
		/// <param name="result">A managed evaluation result</param>
		/// <param name="bufferContent">The contents of the buffer used for the evaluation</param>
		public NativeEvaluationResult(EvaluationResult result, ReadOnlySpan<char> bufferContent)
		{

			wasMatchFound = (byte)(result.WasMatchFound ? 1 : 0);
			matchValueStart = result.WasMatchFound
								? bufferContent.IndexOf(result.MatchValue!)
								: -1;
			matchValueEnd = result.WasMatchFound
								? matchValueStart + result.MatchValue!.Length
								: -1;

		}

		/// <summary>
		/// Creates a <see cref="NativeEvaluationResult" /> given an <see cref="EvaluationResult"/>
		/// and a <see cref="String"/>.
		/// </summary>
		/// <param name="result">A managed evaluation result</param>
		/// <param name="bufferContent">The contents of the buffer used for the evaluation</param>
		public NativeEvaluationResult(EvaluationResult result, string bufferContent) : this(result, bufferContent.AsSpan()) { }

		/// <summary>
		/// Creates a <see cref="NativeEvaluationResult" /> given an <see cref="EvaluationResult"/>
		/// and a <see cref="ReadOnlyMemory{Char}"/>.
		/// </summary>
		/// <param name="result">A managed evaluation result</param>
		/// <param name="bufferContent">The contents of the buffer used for the evaluation</param>
		public NativeEvaluationResult(EvaluationResult result, ReadOnlyMemory<char> bufferContent) : this(result, bufferContent.Span) { }

		/// <summary>
		/// Creates a <see cref="NativeEvaluationResult" /> given an <see cref="EvaluationResult"/>
		/// and a <see cref="DualTextBuffer"/>.
		/// </summary>
		/// <param name="result">A managed evaluation result</param>
		/// <param name="buffer">The buffer used for the evaluation</param>
		public NativeEvaluationResult(EvaluationResult result, DualTextBuffer buffer) : this(result, buffer.Text.Span) { }

	}

}
