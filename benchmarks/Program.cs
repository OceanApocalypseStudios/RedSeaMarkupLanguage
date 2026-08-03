using BenchmarkDotNet.Running;

using OceanApocalypse.RSML.Benchmarks.Sources;

namespace OceanApocalypse.RSML.Benchmarks;

internal sealed class Program
{
	private static void Main(string[] args) => BenchmarkRunner.Run<BufferBenchmarks>(args: args);
}
