using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;
using OceanApocalypseStudios.RSML.Analyzer.Semantics;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Evaluation;


namespace OceanApocalypseStudios.RSML.Native
{

	/// <summary>
	/// C ABI exports for RSML toolchain components.
	/// </summary>
	public static unsafe class ToolchainExports
	{

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static RsmlToken FromManagedToNativeSyntaxToken(SyntaxToken token) => new()
		{
			kind = (byte)token.Kind,
			startIndex = token.BufferRange.Start.IsFromEnd ? (0 - token.BufferRange.Start.Value) : token.BufferRange.Start.Value,
			endIndex = token.BufferRange.End.IsFromEnd ? (0 - token.BufferRange.End.Value) : token.BufferRange.End.Value
		};

		internal static RsmlLine FromManagedToNativeSyntaxLine(SyntaxLine line) => new()
		{
			item1 = FromManagedToNativeSyntaxToken(line.Item1),
			item2 = FromManagedToNativeSyntaxToken(line.Item2),
			item3 = FromManagedToNativeSyntaxToken(line.Item3),
			item4 = FromManagedToNativeSyntaxToken(line.Item4),
			item5 = FromManagedToNativeSyntaxToken(line.Item5),
			item6 = FromManagedToNativeSyntaxToken(line.Item6),
			item7 = FromManagedToNativeSyntaxToken(line.Item7),
			item8 = FromManagedToNativeSyntaxToken(line.Item8)
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static SyntaxToken FromNativeToManagedSyntaxToken(RsmlToken token) => new((TokenKind)token.kind, token.startIndex, token.endIndex);
		internal static SyntaxLine FromNativeToManagedSyntaxLine(RsmlLine* line) => new(
				FromNativeToManagedSyntaxToken(line->item1),
				FromNativeToManagedSyntaxToken(line->item2),
				FromNativeToManagedSyntaxToken(line->item3),
				FromNativeToManagedSyntaxToken(line->item4),
				FromNativeToManagedSyntaxToken(line->item5),
				FromNativeToManagedSyntaxToken(line->item6),
				FromNativeToManagedSyntaxToken(line->item7),
				FromNativeToManagedSyntaxToken(line->item8)
			);

		/// <summary>
		/// Normalizes a line of RSML.
		/// </summary>
		/// <param name="inputLine">The line to normalize</param>
		/// <param name="outputLine">The normalized line</param>
		/// <returns>
		/// <list type="bullet"><c>-4:</c> Output token count exceeds 8<br /></list>
		/// <list type="bullet"><c>-3:</c> An error occured while normalizing the input line<br /></list>
		/// <list type="bullet"><c>-2:</c> The input line is empty<br /></list>
		/// <list type="bullet"><c>-1:</c> At least one of the given pointers is null (<c>IntPtr.Zero</c>)<br /></list>
		/// <list type="bullet"><c>0:</c> Success<br /></list>
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) }, EntryPoint = "rsml_normalize_line")]
		public static int NormalizeRsmlLine(nint inputLine, nint outputLine)
		{

			try
			{

				System.Diagnostics.Debug.Assert(sizeof(RsmlToken) == 12);
				System.Diagnostics.Debug.Assert(sizeof(RsmlLine) == 96);

				if (inputLine == IntPtr.Zero || outputLine == IntPtr.Zero)
					return -1;

				var src = (RsmlLine*)inputLine.ToPointer();
				var line = FromNativeToManagedSyntaxLine(src);

				if (line.IsEmpty)
					return -2;

				Normalizer.NormalizeLine(ref line, out int tokenCount);

				if (tokenCount > 8)
					return -4;

				var dst = (RsmlLine*)outputLine.ToPointer();
				*dst = *src;

				dst->item1 = FromManagedToNativeSyntaxToken(line.Item1);
				dst->item2 = FromManagedToNativeSyntaxToken(line.Item2);
				dst->item3 = FromManagedToNativeSyntaxToken(line.Item3);
				dst->item4 = FromManagedToNativeSyntaxToken(line.Item4);
				dst->item5 = FromManagedToNativeSyntaxToken(line.Item5);
				dst->item6 = FromManagedToNativeSyntaxToken(line.Item6);
				dst->item7 = FromManagedToNativeSyntaxToken(line.Item7);
				dst->item8 = FromManagedToNativeSyntaxToken(line.Item8);

				return 0;

			}
			catch { return -3; }

		}

		/// <summary>
		/// Evaluates a RSML document from the local machine.
		/// </summary>
		/// <param name="document">The document to evaluate</param>
		/// <param name="documentLength">The length of the document</param>
		/// <param name="resultBuffer">The buffer in which to store the evaluation result</param>
		/// <param name="resultBufferLength">The length of the result buffer</param>
		/// <returns>
		/// <c>-1:</c> An error occured while evaluating the document OR the result buffer is too small to hold the result<br />
		/// <c>0:</c> No matches were found (result buffer unassigned)<br />
		/// <c>1:</c> A match was found (result buffer assigned)<br />
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) }, EntryPoint = "rsml_evaluate_document")]
		public static int EvaluateRsmlDocument(byte* document, int documentLength, byte* resultBuffer, int resultBufferLength)
		{

			// fixme: fix this method
			// todo: ensure each error has its own return code instead of a "-1 catch all"

			ReadOnlySpan<byte> span = new(document, documentLength);
			Evaluator evaluator = new(span);
			EvaluationResult result;

			try
			{
				result = evaluator.Evaluate();
			}
			catch (Exception)
			{
				return -1;
			}

			if (!result.WasMatchFound)
				return 0;

			if (resultBufferLength > result.MatchValue!.Length)
				return -1;

			var asBytes = Encoding.UTF8.GetBytes(result.MatchValue!);

			for (int i = 0; i < asBytes.Length; i++)
				resultBuffer[i] = asBytes[i];

			return 1;

		}

	}

}
