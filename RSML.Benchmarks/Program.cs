using System;

using BenchmarkDotNet.Running;


namespace RSML.Benchmarks
{

	internal class Program
	{

		private static void Main() => BenchmarkRunner.Run<EvaluatorBenchmarks>();

	}

}
