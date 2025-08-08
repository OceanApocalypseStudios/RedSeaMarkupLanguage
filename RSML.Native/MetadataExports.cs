using System.Runtime.InteropServices;
using System.Text;


namespace RSML.Native
{

	/// <summary>
	/// Exports for RSML metadata.
	/// </summary>
	public static unsafe class MetadataExports
	{

		private const byte ApiAuthorNameLen = 16;

		private static readonly byte[] authorName =
		[
			0x4F, 0x63, 0x65, 0x61, 0x6E, 0x41,
			0x70, 0x6F, 0x63, 0x61, 0x6C, 0x79,
			0x70, 0x73, 0x65, 0x53, 0x74, 0x75,
			0x64, 0x69, 0x6F, 0x73
		]; // OceanApocalypseStudios

		private static readonly byte[] docsLink = Encoding.UTF8.GetBytes($"https://oceanapocalypsestudios.org/rsml-docs/{"change me"}/"); // todo

		private static readonly byte[] utf8ApiVersion = Encoding.UTF8.GetBytes("change me"); // todo

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

	}

}
