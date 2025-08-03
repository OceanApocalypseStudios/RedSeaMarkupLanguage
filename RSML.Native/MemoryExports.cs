using System;
using System.Runtime.InteropServices;


namespace RSML.Native
{

	/// <summary>
	/// Exports for public memory-related operations.
	/// </summary>
	public static class MemoryExports
	{

		/// <summary>
		/// Frees memory.
		/// </summary>
		/// <param name="ptr">A pointer</param>
		/// <returns><c>1</c> if the memory was freed, <c>0</c> if it was already free, <c>-1</c> if an exception occured</returns>
		[UnmanagedCallersOnly(EntryPoint = "rsml_free_memory")]
		public static int FreeMemory(nint ptr)
		{

			try
			{

				if (ptr == IntPtr.Zero)
					return 0;

				Marshal.FreeHGlobal(ptr);

				return 1;

			}
			catch
			{
				return -1;
			}

		}

	}

}
