using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;


namespace OceanApocalypseStudios.RSML.Benchmarks
{

	internal class Program
	{

		// Entry point.
		private static void Main() => Release();

		// Regular benchmarking.
		private static void Release() => BenchmarkRunner.Run<EvaluatorBenchmarks>();

		// Debugging benchmarks.
		private static void Debug() => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(null, new DebugInProcessConfig());

	}

}
