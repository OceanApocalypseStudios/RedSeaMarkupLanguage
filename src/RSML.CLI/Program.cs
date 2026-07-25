using System;
using System.Linq;
using System.Reflection;


namespace OceanApocalypseStudios.RSML.CLI;

internal class Program
{
	private static void Main(string[] args)
	{
		// todo: implement

		Console.WriteLine("Assembly Version: " +
			(Assembly.GetExecutingAssembly()
				.GetName()
				.Version
				?.ToString()
			?? "Not Found")
		);
		Console.WriteLine("File Version: " +
			(Assembly.GetExecutingAssembly()
				.GetCustomAttribute<AssemblyFileVersionAttribute>()
				?.Version
			?? "Not Found")
		);
		Console.WriteLine("Informational Version: " +
			(Assembly.GetExecutingAssembly()
				.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
				?.InformationalVersion
			?? "Not Found")
		);
		Console.WriteLine("Semantic Version: " +
			(Assembly.GetExecutingAssembly()
				.GetCustomAttributes<AssemblyMetadataAttribute>()
				.FirstOrDefault(a => a.Key == "SemVersion")
				?.Value
			?? "Not Found")
		);
	}
}
