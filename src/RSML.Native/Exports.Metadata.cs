using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using OceanApocalypseStudios.RSML.Native.Structures;


namespace OceanApocalypseStudios.RSML.Native
{

	// Metadata exports
	public static unsafe partial class Exports
	{

		/// <summary>
		/// Writes the name of the creator (and lead maintainer) of RSML's API to a buffer.
		/// If the buffer's byte count is -1, the pointer is null and the error message was saved.
		/// If the buffer's byte count is -2, the pointer is null and the error message was saved.
		/// </summary>
		/// <returns>The buffer and its byte count</returns>
		[UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)], EntryPoint = "rsml_get_api_author_name")]
		public static BufferResult GetApiAuthorName()
		{

			try
			{

				fixed (byte* data = authorNameBytes)
					return new(data, authorNameByteCount);

			}
			catch (Exception ex)
			{

				try
				{

					if (lastErrorMessage != IntPtr.Zero)
						Marshal.FreeHGlobal(lastErrorMessage);

					lastErrorMessage = Marshal.StringToHGlobalAuto(ex.Message);

				}
				catch
				{
					return new(null, -2);
				}

				return new(null, -1);

			}

		}

		/// <summary>
		/// Writes the URL to RSML's documentation to a buffer.
		/// If the buffer's byte count is -1, the pointer is null and the error message was saved.
		/// If the buffer's byte count is -2, the pointer is null and the error message was saved.
		/// </summary>
		/// <returns>The buffer and its byte count</returns>
		[UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ], EntryPoint = "rsml_get_api_documentation_url")]
		public static BufferResult GetApiDocumentationUrl()
		{

			try
			{

				fixed (byte* data = documentationUrlBytes)
					return new(data, documentationUrlByteCount);

			}
			catch (Exception ex)
			{

				try
				{

					if (lastErrorMessage != IntPtr.Zero)
						Marshal.FreeHGlobal(lastErrorMessage);

					lastErrorMessage = Marshal.StringToHGlobalAuto(ex.Message);

				}
				catch
				{
					return new(null, -2);
				}

				return new(null, -1);

			}

		}

		/// <summary>
		/// Writes the API version to a supplied buffer.
		/// If the buffer's byte count is -1, the pointer is null and the error message was saved.
		/// If the buffer's byte count is -2, the pointer is null and the error message was saved.
		/// </summary>
		/// <returns>The buffer and its byte count</returns>
		[UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ], EntryPoint = "rsml_get_api_version")]
		public static BufferResult GetApiVersion()
		{

			try
			{

				fixed (byte* data = apiVersionBytes)
					return new(data, apiVersionByteCount);

			}
			catch (Exception ex)
			{

				try
				{

					if (lastErrorMessage != IntPtr.Zero)
						Marshal.FreeHGlobal(lastErrorMessage);

					lastErrorMessage = Marshal.StringToHGlobalAuto(ex.Message);

				}
				catch
				{
					return new(null, -2);
				}

				return new(null, -1);

			}

		}

		internal const string ApiVersion = "2.1.0";
		internal const string AuthorName = "OceanApocalypseStudios";
		internal const string DocumentationUrl = "https://oceanapocalypsestudios.org/RedSeaMarkupLanguage/";

		private readonly static byte[] apiVersionBytes = Encoding.Default.GetBytes(ApiVersion);
		private readonly static byte[] authorNameBytes = Encoding.Default.GetBytes(AuthorName);
		private readonly static byte[] documentationUrlBytes = Encoding.Default.GetBytes(DocumentationUrl);

		private readonly static int apiVersionByteCount = Encoding.Default.GetByteCount(ApiVersion);
		private readonly static int authorNameByteCount = Encoding.Default.GetByteCount(AuthorName);
		private readonly static int documentationUrlByteCount = Encoding.Default.GetByteCount(DocumentationUrl);

	}

}
