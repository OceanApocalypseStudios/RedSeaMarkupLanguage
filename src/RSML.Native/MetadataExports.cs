using System.Runtime.InteropServices;
using System.Text;


namespace OceanApocalypseStudios.RSML.Native
{

	/// <summary>
	/// Exports for RSML metadata.
	/// </summary>
	public static unsafe class MetadataExports
	{

		private const string ApiVersion = "2.1.0";
		private const byte ApiAuthorNameLen = 16;

		private static readonly byte[] authorName = "OceanApocalypseStudios"u8.ToArray();
		private static readonly byte[] docsLink = Encoding.UTF8.GetBytes($"https://oceanapocalypsestudios.org/RedSeaMarkupLanguage/");
		private static readonly byte[] utf8ApiVersion = Encoding.UTF8.GetBytes(ApiVersion);

		/// <summary>
		/// Writes the name of the creator (and lead maintainer) of RSML's API to a supplied buffer.
		/// </summary>
		/// <param name="buffer">The buffer to write to</param>
		/// <param name="bufferSize">The size of the given buffer</param>
		/// <returns>The length of the author name or <c>-1</c> if the given buffer wasn't big enough.</returns>
		[UnmanagedCallersOnly(EntryPoint = "rsml_get_api_author_name")]
		public static int GetApiAuthorName(byte* buffer, int bufferSize)
		{

			if (bufferSize < ApiAuthorNameLen)
				return -1;

			for (int i = 0; i < ApiAuthorNameLen; i++)
				buffer[i] = authorName[i];

			return ApiAuthorNameLen;

		}

		/// <summary>
		/// Writes the URL to RSML's documentation to a supplied buffer.
		/// </summary>
		/// <param name="buffer">The buffer to write to</param>
		/// <param name="bufferSize">The size of the given buffer</param>
		/// <returns>The length of the author name or <c>-1</c> if the given buffer wasn't big enough</returns>
		[UnmanagedCallersOnly(EntryPoint = "rsml_get_api_documentation_url")]
		public static int GetApiDocumentationUrl(byte* buffer, int bufferSize)
		{

			if (bufferSize < docsLink.Length)
				return -1;

			for (int i = 0; i < docsLink.Length; i++)
				buffer[i] = docsLink[i];

			return docsLink.Length;

		}

		/// <summary>
		/// Writes the API version to a supplied buffer.
		/// </summary>
		/// <param name="buffer">The buffer to write to</param>
		/// <param name="bufferSize">The size of the given buffer</param>
		/// <returns>The length of the API version string or <c>-1</c> if the given buffer wasn't big enough.</returns>
		[UnmanagedCallersOnly(EntryPoint = "rsml_get_api_version")]
		public static int GetApiVersion(byte* buffer, int bufferSize)
		{

			int len = utf8ApiVersion.Length;

			if (bufferSize < len)
				return -1;

			for (int i = 0; i < len; i++)
				buffer[i] = utf8ApiVersion[i];

			return len;

		}

	}

}
