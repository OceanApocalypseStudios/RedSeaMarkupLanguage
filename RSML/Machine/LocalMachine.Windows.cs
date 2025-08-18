#if WINDOWS

using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Win32;


namespace RSML.Machine
{

	[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")] // this only compiles on windows there's no problem
	public partial struct LocalMachine
	{

		private void InitializeVersionData_Windows()
		{

			using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

			if (key is null)
				return;

			string? buildNumStr = key.GetValue("CurrentBuildNumber") as string;

			if (Int32.TryParse(buildNumStr, out int buildNum) && buildNum >= 22000)
			{

				SystemVersion = 11; // Windows 11

				return;

			}

			object? majorVersionObj = key.GetValue("CurrentMajorVersionNumber");

			if (majorVersionObj is int majorVersion)
			{

				SystemVersion = majorVersion; // Windows 10, most likely

				return;

			}

			// fallback
			if (key.GetValue("CurrentVersion") is not string version)
				return;

			switch (version)
			{

				case "5.1":
					// ReSharper disable once CommentTypo
					SystemVersion = 5; // WINDOWS XPPPPPP LESGOOOOOOOO

					return;

				case "6.0":
					// this is Windows Vista
					// but like we don't take strings soooo
					// number 6 it'll be
					SystemVersion = 6;

					return; // cursed af

				case "6.3":
					// this technically can also be Windows 10
					// but Windows 10 usually has CurrentMajorVersionNumber, so THAT has greater priority

					SystemVersion = 9; // yes, Windows 8.1 will be 9 in this case, cuz no decimals

					return;

			}

			string[] parts = version.Split('.');

			if (parts.Length < 2)
				return;

			/*
			 * Something funny that happens is:
			 * - Windows 7 => 6.1
			 * - Windows 8 => 6.2
			 *
			 * Notice it?
			 * For 7 and 8, it's [0] + [1].
			 */

			if (!Int32.TryParse(parts[0], out int ver1) || !Int32.TryParse(parts[1], out int ver2))
				return;

			SystemVersion = ver1 + ver2;

		}

	}

}

#endif
