using System;
using System.Diagnostics.CodeAnalysis;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Evaluation;
using OceanApocalypseStudios.RSML.Reader;

using LocalMachine = OceanApocalypseStudios.RSML.Machine.LocalMachine;


namespace OceanApocalypseStudios.RSML.Toolchain
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
		/// The evaluator.
		/// </summary>
		public IEvaluator? Evaluator { get; private set; }

		/// <summary>
		/// Binds a toolchain component.
		/// </summary>
		/// <param name="component">The component to attach</param>
		/// <returns>The toolchain</returns>
		public Toolchain Bind(IToolchainComponent component)
		{

			switch (component)
			{

				case ILexer lexer:
					Lexer = lexer;

					break;

				case IReader reader:
					Reader = reader;

					break;

				case IEvaluator evaluator:
					Evaluator = evaluator;

					break;

			}

			return this;

		}

		/// <summary>
		/// Binds a toolchain component by creating a new one.
		/// </summary>
		/// <typeparam name="TComponent">The type of the component</typeparam>
		/// <returns>The toolchain</returns>
		/// <remarks>This method uses Activator.CreateInstance, therefore not being AOT-friendly.</remarks>
		[RequiresDynamicCode("This method uses Activator.CreateInstance")]
		public Toolchain Bind<TComponent>()
			where TComponent : IToolchainComponent =>
			Bind(Activator.CreateInstance<TComponent>());

		/// <summary>
		/// Unbinds all toolchain components.
		/// </summary>
		/// <returns>The toolchain</returns>
		public Toolchain UnbindAll()
		{

			Lexer = null;
			Reader = null;
			Evaluator = null;

			return this;

		}

		/// <summary>
		/// Evaluates the RSML document with the toolchain.
		/// </summary>
		/// <param name="machineData">The machine's data</param>
		/// <returns>The evaluation result or <c>null</c> if the evaluator is <c>null</c></returns>
		public EvaluationResult? EvaluateRsml(LocalMachine machineData) => Evaluator?.Evaluate(machineData, Reader);

		/// <summary>
		/// Creates an empty toolchain.
		/// </summary>
		/// <returns></returns>
		public static Toolchain Create() =>
			new()
			{

				Lexer = null,
				Reader = null,
				Evaluator = null

			};

		/// <summary>
		/// Creates a toolchain using Reflection.
		/// </summary>
		/// <param name="content">The reader's content</param>
		/// <typeparam name="TLexer">The lexer</typeparam>
		/// <typeparam name="TReader">The reader</typeparam>
		/// <typeparam name="TEvaluator">The evaluator</typeparam>
		/// <returns>The toolchain</returns>
		/// <remarks>This method uses Activator.CreateInstance, therefore not being AOT-friendly.</remarks>
		[RequiresDynamicCode("This method uses Activator.CreateInstance")]
		public static Toolchain Create<TLexer, TReader, TEvaluator>(string content)
			where TLexer : ILexer
			where TReader : IReader
			where TEvaluator : IEvaluator =>
			new()
			{

				Lexer = Activator.CreateInstance<TLexer>(),
				Reader = Activator.CreateInstance(typeof(TReader), content) as IReader,
				Evaluator = Activator.CreateInstance(typeof(TEvaluator), content) as IEvaluator

			};

	}

}
