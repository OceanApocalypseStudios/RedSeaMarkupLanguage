using System;
using System.Collections.ObjectModel;

using RSML.Actions;
using RSML.Analyzer.Syntax;
using RSML.Machine;
using RSML.Reader;
using RSML.Toolchain;


namespace RSML.Evaluation
{

	/// <summary>
	/// An evaluator that evaluates a RSML document and returns a match's value, if one was found.
	/// </summary>
	public interface IEvaluator : IToolchainComponent
	{

		/// <summary>
		/// The content loaded into the evaluator.
		/// </summary>
		string Content { get; set; }

		/// <summary>
		/// The special actions in use.
		/// </summary>
		ReadOnlyDictionary<string, SpecialAction> SpecialActions { get; }

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

		/// <summary>
		/// Evaluates the RSML document with the specified machine data and a reader.
		/// Note that the above-mentioned toolchain components should have the same standardized version as the evaluator itself,
		/// when using the official evaluator.
		/// </summary>
		/// <param name="machineData">The machine data</param>
		/// <param name="reader">The custom reader</param>
		/// <returns></returns>
		EvaluationResult Evaluate(LocalMachine machineData, IReader? reader);

		/// <summary>
		/// Registers a special action.
		/// </summary>
		/// <param name="name">The name of the action</param>
		/// <param name="action">The action to register</param>
		/// <returns>Self. Fluent API.</returns>
		IEvaluator RegisterSpecialAction(string name, SpecialAction action);

	}

}
