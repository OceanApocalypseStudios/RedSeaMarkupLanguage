using BenchmarkDotNet.Running;

using OceanApocalypseStudios.RSML.Benchmarks.Sources;


namespace OceanApocalypseStudios.RSML.Benchmarks;

internal class Program
{
	private static void Main(string[] args) => BenchmarkRunner.Run<BufferBenchmarks>(args: args);
}
