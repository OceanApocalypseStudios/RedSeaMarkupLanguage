using System;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;

using OceanApocalypseStudios.RSML.Benchmarks.Helpers;
using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Benchmarks.Sources;

[SimpleJob(RuntimeMoniker.Net10_0)]
[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.Net481)]
[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.NativeAot10_0)]
[SimpleJob(RuntimeMoniker.NativeAot80)]
[MemoryDiagnoser]
[CsvExporter]
[RPlotExporter]
public class BufferBenchmarks
{
	private readonly Consumer consumer = new();
	private string data = "";
	private ReadOnlyStringBuffer buffer = null!;

	[Params(1, 10, 100, 1_000, 10_000)]
	public int RepeatCount; // 1 is the string itself

	[GlobalSetup]
	public void Setup()
	{
		data = DataGenerator.GetSampleData(RepeatCount);
		buffer = new(data);
	}

	[Benchmark]
	public void ReadOnlyStringBuffer_GetLine()
	{
		for (int i = 0; i <= data.Length; i++)
			consumer.Consume(buffer.GetLineNumberFromIndex(i));
	}

	[Benchmark]
	public void ReadOnlySpanBuffer_GetLine()
	{
		for (int i = 0; i <= data.Length; i++)
			consumer.Consume(new ReadOnlySpanBuffer(data).GetLineNumberFromIndex(i));
	}
}
