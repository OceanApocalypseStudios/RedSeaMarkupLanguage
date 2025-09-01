using BenchmarkDotNet.Running;


namespace OceanApocalypseStudios.RSML.Benchmarks
{

	internal class Program
	{

		private static void Main() => BenchmarkRunner.Run<EvaluatorBenchmarks>();

	}

}
