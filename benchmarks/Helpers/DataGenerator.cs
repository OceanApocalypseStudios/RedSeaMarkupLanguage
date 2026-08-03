using System;
using System.Linq;


namespace OceanApocalypse.RSML.Benchmarks.Helpers;

internal static class DataGenerator
{
	internal const string SampleData =
		"This is purely random\r\n" +
		"text,\n\r" +
		"completely made up to\u2028\u2029" +
		"test whatever needs to\u2029" +
		"be tested.\n" +
		"Come to think\tabout it, this\r" +
		"string is quite random. I guess you could\u2028" +
		"say it's \r\r" +
		"System.Random. I know\r\n\r\n\n" +
		"where the doo\r" +
		"r is.\n\n\n\r\n\n\r" +
		"Here are some random\r" +
		"characters because why\u2029" +
		"not? !%#%/$&)$35#&(=?%(/*-+\r\n\u2029";

	public static string GetSampleData(int repeatCount) => String.Join("", Enumerable.Repeat(SampleData, repeatCount));
}
