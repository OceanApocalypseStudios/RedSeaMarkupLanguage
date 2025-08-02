using BenchmarkDotNet.Running;


namespace RSML.Benchmarks
{

	internal class Program
	{

		static void Main() => BenchmarkRunner.Run<RSParserBenchmarks>();

	}

}
