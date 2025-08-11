using System;

using RSML.Analyzer.Semantics;
using RSML.Analyzer.Syntax;
using RSML.Evaluation;
using RSML.Machine;
using RSML.Reader;


namespace RSML.Toolchain
{

	/// <summary>
	/// A RSML toolchain.
	/// </summary>
	public sealed class Toolchain
	{

		/// <summary>
		/// The lexer.
		/// </summary>
		public ILexer? Lexer { get; private set; }

		/// <summary>
		/// The RSML reader.
		/// </summary>
		public IReader? Reader { get; private set; }

		/// <summary>
		/// The normalizer.
		/// </summary>
		public INormalizer? Normalizer { get; private set; }

		/// <summary>
		/// The validator.
		/// </summary>
		public IValidator? Validator { get; private set; }

		/// <summary>
		/// The evaluator.
		/// </summary>
		public IEvaluator? Evaluator { get; private set; }

		/// <summary>
		/// Binds a lexer.
		/// </summary>
		/// <param name="lexer">The lexer</param>
		/// <returns>The toolchain</returns>
		public Toolchain Bind(ILexer? lexer) => ReturnHelper(() => Lexer = lexer);

		/// <summary>
		/// Binds a reader.
		/// </summary>
		/// <param name="reader">The reader</param>
		/// <returns>The toolchain</returns>
		public Toolchain Bind(IReader? reader) => ReturnHelper(() => Reader = reader);

		/// <summary>
		/// Binds a normalizer.
		/// </summary>
		/// <param name="normalizer">The normalizer</param>
		/// <returns>The toolchain</returns>
		public Toolchain Bind(INormalizer? normalizer) => ReturnHelper(() => Normalizer = normalizer);

		/// <summary>
		/// Binds a semantic validator.
		/// </summary>
		/// <param name="validator">The validator</param>
		/// <returns>The toolchain</returns>
		public Toolchain Bind(IValidator? validator) => ReturnHelper(() => Validator = validator);

		/// <summary>
		/// Binds an evaluator.
		/// </summary>
		/// <param name="evaluator">The evaluator</param>
		/// <returns>The toolchain</returns>
		public Toolchain Bind(IEvaluator? evaluator) => ReturnHelper(() => Evaluator = evaluator);
		
		/// <summary>
		/// Unbinds all toolchain components.
		/// </summary>
		/// <returns>The toolchain</returns>
		public Toolchain UnbindAll()
		{
			
			Lexer = null;
			Reader = null;
			Normalizer = null;
			Validator = null;
			Evaluator = null;
			
			return this;

		}

		/// <summary>
		/// Evaluates the RSML document with the toolchain.
		/// </summary>
		/// <param name="machineData">The machine's data</param>
		/// <returns>The evaluation result or <c>null</c> if the evaluator is <c>null</c></returns>
		public EvaluationResult? EvaluateRsml(LocalMachine machineData) => Evaluator?.Evaluate(machineData, Reader, Lexer, Normalizer, Validator);

		private Toolchain ReturnHelper(Action action)
		{

			action.Invoke();

			return this;

		}

		/// <summary>
		/// Creates an empty toolchain.
		/// </summary>
		/// <returns></returns>
		public static Toolchain Create() =>
			new()
			{

				Lexer = null,
				Reader = null,
				Normalizer = null,
				Validator = null,
				Evaluator = null

			};

		/// <summary>
		/// Creates a toolchain using Reflection.
		/// </summary>
		/// <param name="content">The reader's content</param>
		/// <typeparam name="TLexer">The lexer</typeparam>
		/// <typeparam name="TReader">The reader</typeparam>
		/// <typeparam name="TNormalizer">The normalizer</typeparam>
		/// <typeparam name="TValidator">The validator</typeparam>
		/// <typeparam name="TEvaluator">The evaluator</typeparam>
		/// <returns></returns>
		public static Toolchain Create<TLexer, TReader, TNormalizer, TValidator, TEvaluator>(string content)
			where TLexer : ILexer
			where TReader : IReader
			where TNormalizer : INormalizer
			where TValidator : IValidator
			where TEvaluator : IEvaluator =>
			new()
			{

				Lexer = Activator.CreateInstance<TLexer>(),
				Reader = Activator.CreateInstance(typeof(TReader), content) as RsReader,
				Normalizer = Activator.CreateInstance<TNormalizer>(),
				Validator = Activator.CreateInstance<TValidator>(),
				Evaluator = Activator.CreateInstance(typeof(TValidator), content) as Evaluator

			};

	}

}
