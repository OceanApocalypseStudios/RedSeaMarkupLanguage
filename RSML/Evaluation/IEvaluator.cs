using System;

using OceanApocalypseStudios.RSML.Analyzer;
using OceanApocalypseStudios.RSML.Toolchain;

using LocalMachine = OceanApocalypseStudios.RSML.Machine.LocalMachine;


namespace OceanApocalypseStudios.RSML.Evaluation
{

	/// <summary>
	/// An evaluator that evaluates a RSML document and returns a match's value, if one was found.
	/// </summary>
	public interface IEvaluator : IToolchainComponent
	{

		/// <summary>
		/// The content loaded into the evaluator.
		/// </summary>
		DualTextBuffer Content { get; }

		/// <summary>
		/// Checks if a given line of RSML is a comment.
		/// </summary>
		/// <param name="line">The line</param>
		/// <returns><c>true</c> if comment</returns>
		static abstract bool IsComment(ReadOnlySpan<char> line);

		/// <summary>
		/// Checks if a given line of RSML is a comment.
		/// </summary>
		/// <param name="line">The line</param>
		/// <returns><c>true</c> if comment</returns>
		static abstract bool IsComment(string line);

		/// <summary>
		/// Evaluates the RSML document with the machine's data.
		/// </summary>
		/// <returns>A result</returns>
		EvaluationResult Evaluate();

		/// <summary>
		/// Evaluates the RSML document with the specified machine data.
		/// </summary>
		/// <param name="machineData">The machine data</param>
		/// <returns>A result</returns>
		EvaluationResult Evaluate(LocalMachine machineData);

	}

}
