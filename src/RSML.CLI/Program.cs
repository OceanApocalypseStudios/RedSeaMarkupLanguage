using System;
using System.Linq;
using System.Reflection;

namespace OceanApocalypseStudios.RSML.CLI;

#pragma warning disable S1118 // it's an entry point class
internal sealed class Program
#pragma warning restore S1118
{
	private static void Main(string[] args)
	{
		// todo: implement

		Console.WriteLine("Assembly Version: " +
			(Assembly.GetExecutingAssembly()
				.GetName()
				.Version
				?.ToString()
			)
		);
		Console.WriteLine("File Version: " +
			(Assembly.GetExecutingAssembly()
				.GetCustomAttribute<AssemblyFileVersionAttribute>()
				?.Version
			)
		);
		Console.WriteLine("Informational Version: " +
			(Assembly.GetExecutingAssembly()
				.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
				?.InformationalVersion
			)
		);
		Console.WriteLine("Semantic Version: " +
			(Assembly.GetExecutingAssembly()
				.GetCustomAttributes<AssemblyMetadataAttribute>()
				.FirstOrDefault(a => a.Key == "SemVersion")
				?.Value
			)
		);
	}
}
