using System;
using System.Diagnostics.CodeAnalysis;

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
[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Benchmarks have to be public.")]
public class BufferBenchmarks : IDisposable
{
	private bool isDisposed;
	private readonly Consumer consumer = new();
	private string data = "";
	private ReadOnlyStringBuffer buffer = null!;

	[Params(1, 10, 100, 1_000)]
	public int RepeatCount { get; set; } // 1 is the string itself

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
		using var buffer = new ReadOnlySpanBuffer(data);

		for (int i = 0; i <= data.Length; i++)
			consumer.Consume(buffer.GetLineNumberFromIndex(i));
	}

	[GlobalCleanup]
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (isDisposed)
			return;

		if (disposing) // managed resources
			buffer.Dispose();

		isDisposed = true;
	}
}
