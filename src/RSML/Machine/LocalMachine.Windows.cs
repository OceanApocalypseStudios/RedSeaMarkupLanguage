#if WINDOWS

using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Win32;


namespace OceanApocalypseStudios.RSML.Machine
{

	[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")] // this only compiles on windows there's no problem
	public partial struct LocalMachine
	{

		private void InitializeVersionData_Windows()
		{

			using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

			if (key is null)
				return;

			if (!Int32.TryParse(key.GetValue("CurrentBuildNumber") as string, out int buildNum))
				return;

			SystemVersion = buildNum switch
			{

				// Windows 11
				>= 22000 => 11,
				// Windows 10
				>= 10240 => 10,
				// Windows 8.1 (has to be called 9 so operators work in rsml)
				>= 9257 => 9,
				// Windows 8
				>= 7652 => 8,
				// Windows 7
				>= 6427 => 7,
				// Windows Vista (since first Longhorn build)
				>= 3663 => 6,
				// Windows XP
				>= 2196 => 5,
				// Prior to XP
				_ => 4

			};

		}

	}

}

#endif
