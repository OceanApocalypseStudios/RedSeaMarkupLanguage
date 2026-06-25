using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using OceanApocalypseStudios.RSML.Analyzer;


namespace OceanApocalypseStudios.RSML.Native
{

	/// <summary>
	/// C ABI exports for RSML toolchain and general components.
	/// </summary>
	public static unsafe partial class Exports
	{

		/// <summary>
		/// Allocates a buffer to be public to all RSML toolchain tools.
		/// </summary>
		/// <param name="content">The buffer's contents</param>
		/// <param name="byteCount">The amount of bytes the content has</param>
		/// <returns>
		/// <list type="bullet"><c>-3:</c> Unknown error<br /></list>
		/// <list type="bullet"><c>-2:</c> The given amount of bytes is less than 0<br /></list>
		/// <list type="bullet"><c>-1:</c> The given pointer is null or the input buffer is null<br /></list>
		/// <list type="bullet"><c>0:</c> Success<br /></list>
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)], EntryPoint = "rsml_alloc_buffer")]
		public static int AllocRsmlBuffer(
			byte* content,
			int byteCount
		)
		{

			try
			{

				string data = Encoding.Default.GetString(content, byteCount);

				if (data == "")
					throw new ArgumentNullException(null, "String is empty");

				buffer = new(data);

				return 0;

			}
			catch (ArgumentNullException ane)
			{

				if (lastErrorMessage != IntPtr.Zero)
					Marshal.FreeHGlobal(lastErrorMessage);

				lastErrorMessage = Marshal.StringToHGlobalAuto(ane.Message);

				return -1;

			}
			catch (ArgumentOutOfRangeException aoo)
			{

				if (lastErrorMessage != IntPtr.Zero)
					Marshal.FreeHGlobal(lastErrorMessage);

				lastErrorMessage = Marshal.StringToHGlobalAuto(aoo.Message);

				return -2;

			}
			catch (Exception ex)
			{

				if (lastErrorMessage != IntPtr.Zero)
					Marshal.FreeHGlobal(lastErrorMessage);

				lastErrorMessage = Marshal.StringToHGlobalAuto(ex.Message);

				return -3;

			}

		}

		/// <summary>
		/// Destroys memory that is no longer necessary but still in use by RSML.
		/// </summary>
		/// <returns><c>0</c> if successful; <c>-1</c> if not successful</returns>
		[UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)], EntryPoint = "rsml_cleanup")]
		public static int Cleanup()
		{

			try
			{

				if (lastErrorMessage != IntPtr.Zero)
				{

					Marshal.FreeHGlobal(lastErrorMessage);
					lastErrorMessage = IntPtr.Zero;

				}

				buffer = null;

				return 0;

			}
			catch (Exception)
			{
				return -1;
			}

		}

		/// <summary>
		/// Returns the last saved error message. Can be a null pointer (<c>IntPtr.Zero</c>).
		/// </summary>
		/// <returns>The pointer to the error message</returns>
		[UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)], EntryPoint = "rsml_get_last_error_message")]
		public static nint GetLastErrorMessage() => lastErrorMessage;

		/// <summary>
		/// Internally used buffer.
		/// </summary>
		internal static DualTextBuffer? buffer;

		/// <summary>
		/// Pointer to the last saved error message.
		/// </summary>
		internal static nint lastErrorMessage = IntPtr.Zero;

	}

}
