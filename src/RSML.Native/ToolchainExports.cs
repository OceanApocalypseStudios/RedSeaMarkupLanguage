using System;
using System.Runtime.InteropServices;
using System.Text;

using OceanApocalypseStudios.RSML.Evaluation;


namespace OceanApocalypseStudios.RSML.Native
{

	/// <summary>
	/// C ABI exports for RSML toolchain components.
	/// </summary>
	public static unsafe class ToolchainExports
	{

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
		[UnmanagedCallersOnly(EntryPoint = "rsml_evaluate_document")]
		public static int EvaluateRsmlDocument(byte* document, int documentLength, byte* resultBuffer, int resultBufferLength)
		{

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
