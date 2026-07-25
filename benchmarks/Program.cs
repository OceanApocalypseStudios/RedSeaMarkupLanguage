using BenchmarkDotNet.Running;

using OceanApocalypseStudios.RSML.Benchmarks.Sources;

namespace OceanApocalypseStudios.RSML.Benchmarks;

internal sealed class Program
{
	private static void Main(string[] args) => BenchmarkRunner.Run<BufferBenchmarks>(args: args);
}
