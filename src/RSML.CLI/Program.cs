using System;
using System.Reflection;


namespace OceanApocalypseStudios.RSML.CLI;

internal class Program
{
	private static void Main(string[] args) =>
		Console.WriteLine(
			Assembly.GetExecutingAssembly()
				.GetCustomAttribute<AssemblyFileVersionAttribute>()
				?.Version
			?? "Not Found"
		); // todo
}
