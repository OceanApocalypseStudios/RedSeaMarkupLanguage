using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;


// ReSharper disable UnusedAutoPropertyAccessor.Global


namespace OceanApocalypseStudios.RSML.Machine
{

	/// <summary>
	/// A representation of several attributes of the user's local machine.
	/// </summary>
	public partial struct LocalMachine
	{

		/// <summary>
		/// The system's name or <c>null</c> if undetected.
		/// </summary>
		public string? SystemName { get; private set; } = null;

		/// <summary>
		/// The name of the Linux distribution or <c>null</c> if undetected or not Linux.
		/// </summary>
		public string? DistroName { get; private set; } = null;

		/// <summary>
		/// The name of the Linux distribution or <c>null</c> if undetected or not Linux.
		/// </summary>
		public string? DistroFamily { get; private set; } = null;

		/// <summary>
		/// The system's version or <c>-1</c> if undetected.
		/// </summary>
		public int SystemVersion { get; private set; } = -1;

		/// <summary>
		/// The system version, as a string.
		/// </summary>
		public string? StringifiedSystemVersion
		{

			get
			{

				if (field is null && SystemVersion != -1)
					field = SystemVersion.ToString();

				return field;

			}

		} = null;

		/// <summary>
		/// The architecture in which the OS runs at.
		/// This may be different from the process' architecture.
		/// </summary>
		public string? ProcessorArchitecture { get; private set; } = null;

		/// <summary>
		/// Collects data from the system and creates a new instance of the struct.
		/// </summary>
		public LocalMachine()
		{

			InitializeSystemName();
			InitializeProcessorArch();
			InitializeVersionData();

			if (SystemName == "linux")
				InitializeDistroData();

		}

		/// <summary>
		/// Creates a new struct of system attributes.
		/// </summary>
		/// <param name="osName">The OS name or null if undefined</param>
		/// <param name="processorArchitecture">The processor architecture or null if undefined</param>
		/// <param name="osMajorVersion">The OS's major version or null or -1 if undefined</param>
		public LocalMachine(string? osName, string? processorArchitecture, int? osMajorVersion)
		{

			SystemName = osName;
			ProcessorArchitecture = processorArchitecture;
			SystemVersion = osMajorVersion ?? -1;

		}

		/// <summary>
		/// Creates a new struct of system attributes for a Linux distribution.
		/// </summary>
		/// <param name="distroName">The distro's name or null if undefined</param>
		/// <param name="distroFamily">The distro's family or null if undefined</param>
		/// <param name="processorArchitecture">The processor architecture or null if undefined</param>
		/// <param name="distroMajorVersion">The OS's major version or null or -1 if undefined</param>
		public LocalMachine(string? distroName, string? distroFamily, string? processorArchitecture, int? distroMajorVersion)
		{

			SystemName = "linux";
			DistroName = distroName;
			DistroFamily = distroFamily;
			ProcessorArchitecture = processorArchitecture;
			SystemVersion = distroMajorVersion ?? -1;

		}

		private void InitializeProcessorArch() =>
			ProcessorArchitecture = RuntimeInformation.OSArchitecture switch
			{

				Architecture.Arm => "arm32",
				Architecture.Arm64 => "arm64",
				Architecture.X64 => "x64",
				Architecture.X86 => "x86",
				Architecture.LoongArch64 => "loongarch64",
				_ => null

			};

		private void InitializeDistroData()
		{

			if (!File.Exists("/etc/os-release"))
				return;

			string[] lines = File.ReadAllLines("/etc/os-release");

			string? idLine = lines.FirstOrDefault(l => l.StartsWith("ID="));

			if (idLine is null)
				return; // we don't care about anything else

			DistroName = idLine.Split('=')[1].Trim('"');

			string? versionLine = lines.FirstOrDefault(l => l.StartsWith("VERSION_ID="));

			if (versionLine is not null && Int32.TryParse(versionLine.Split('=')[1].Trim('"').Split('.')[0], out int versionNum))
				SystemVersion = versionNum;

			if (DistroName == "fedora")
			{

				// fedora's ID_LIKE looks weird so we'll just say it's fedora
				DistroFamily = "fedora";

			}

			string? likeLine = lines.FirstOrDefault(l => l.StartsWith("ID_LIKE="));

			if (likeLine is not null)
				DistroFamily = likeLine.Split('=')[1].Trim('"');

		}

		internal void InitializeVersionData_FreeBsd()
		{

			try
			{

				using Process? unameR = Process.Start(
					new ProcessStartInfo
					{

						FileName = "uname",
						Arguments = "-r",
						RedirectStandardOutput = true,
						UseShellExecute = false

					}
				);

				string? fullVer = unameR?.StandardOutput.ReadLine()?.Trim();

				if (!Int32.TryParse(fullVer?.Split('.')[0], out int versionNum))
					return;

				SystemVersion = versionNum;

			}
			catch
			{
				// ignored
			}

		}

		internal void InitializeVersionData_Osx()
		{

			try
			{

				using Process? proc = Process.Start(
					new ProcessStartInfo
					{

						FileName = "sw_vers",
						Arguments = "-productVersion",
						RedirectStandardOutput = true,
						UseShellExecute = false

					}
				);

				string? fullVer = proc?.StandardOutput.ReadLine();

				if (!Int32.TryParse(fullVer?.Split('.')[0], out int versionNum))
					return;

				SystemVersion = versionNum;

			}
			catch
			{
				// ignored
			}

		}

		private void InitializeVersionData() =>

#if WINDOWS

			InitializeVersionData_Windows();
#else
			switch (SystemName)
			{

				case "freebsd":
					InitializeVersionData_FreeBsd();
					break;

				case "osx":
					InitializeVersionData_Osx();
					break;

				// we initialize linux version data alongside the distro so for now it doesn't matter

			}

#endif


		private void InitializeSystemName()
		{

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				SystemName = "windows";

			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				SystemName = "osx";

			else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
				SystemName = "freebsd";

			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				SystemName = "linux";

			else
				SystemName = null; // no system name data

		}

		/// <summary>
		/// The machine RSML is running on.
		/// </summary>
		public static LocalMachine CurrentMachine { get; } = new();

		/// <summary>
		/// Merges two <see cref="LocalMachine" /> instances,
		/// using the latter as fallback for the first.
		/// </summary>
		/// <param name="main">The machine whose non-null attributes are used over the fallback's</param>
		/// <param name="fallback">The machine that serves as the fallback for <paramref name="main"/>'s null attributes</param>
		/// <returns>A new instance of a <see cref="LocalMachine"/></returns>
		public static LocalMachine MergeWithFallback(LocalMachine main, LocalMachine fallback) => main.SystemName.IsAsciiEqualsIgnoreCase("linux") || main.DistroName is not null || main.DistroFamily is not null
				? new(main.DistroName ?? fallback.DistroName, main.DistroFamily ?? fallback.DistroFamily, main.ProcessorArchitecture ?? fallback.ProcessorArchitecture, main.SystemVersion == -1 ? fallback.SystemVersion : -1)
				: new(main.SystemName ?? fallback.SystemName, main.ProcessorArchitecture ?? fallback.ProcessorArchitecture, main.SystemVersion == -1 ? fallback.SystemVersion : -1);

		/// <summary>
		/// Merges this <see cref="LocalMachine"/> instance
		/// with another one, which is treated as a fallback.
		/// </summary>
		/// <param name="fallback">The machine that serves as the fallback for this instance's null attributes</param>
		/// <returns>A new instance of a <see cref="LocalMachine"/></returns>
		public LocalMachine MergeWithFallback(LocalMachine fallback) => MergeWithFallback(this, fallback);

		/// <summary>
		/// Merges an instance of <see cref="LocalMachine"/> with
		/// this one, which is treated as a fallback.
		/// </summary>
		/// <param name="main">The machine whose non-null attributes are used over this instance's</param>
		/// <returns>A new instance of a <see cref="LocalMachine"/></returns>
		public LocalMachine MergeAsFallback(LocalMachine main) => MergeWithFallback(main, this);

		/// <summary>
		/// Creates a new struct of system attributes for a Linux distribution.
		/// </summary>
		/// <param name="distroName">The distro's name or null if undefined</param>
		/// <param name="distroFamily">The distro's family or null if undefined</param>
		/// <param name="processorArchitecture">The processor architecture or null if undefined</param>
		/// <param name="distroMajorVersion">The OS's major version or null or -1 if undefined</param>
		public static LocalMachine Linux(string? distroName, string? distroFamily, string? processorArchitecture, int? distroMajorVersion) =>
			new(distroName, distroFamily, processorArchitecture, distroMajorVersion);

	}

}
