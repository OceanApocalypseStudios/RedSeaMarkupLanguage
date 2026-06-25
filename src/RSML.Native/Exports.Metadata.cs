using System.Runtime.InteropServices;


namespace OceanApocalypseStudios.RSML.Native
{

	// Metadata exports
	public static unsafe partial class Exports
	{

		/// <summary>
		/// Writes the name of the creator (and lead maintainer) of RSML's API to a supplied buffer.
		/// </summary>
		/// <param name="buffer">The buffer to write to</param>
		/// <param name="bufferSize">The size of the given buffer</param>
		/// <returns>The length of the author name or <c>-1</c> if the given buffer wasn't big enough.</returns>
		[UnmanagedCallersOnly(EntryPoint = "rsml_get_api_author_name")]
		public static int GetApiAuthorName(
			byte* buffer,
			int bufferSize
		)
		{

			if (bufferSize < authorName.Length)
				return -1;

			for (int i = 0; i < authorName.Length; i++)
				buffer[i] = authorName[i];

			return authorName.Length;

		}

		/// <summary>
		/// Writes the URL to RSML's documentation to a supplied buffer.
		/// </summary>
		/// <param name="buffer">The buffer to write to</param>
		/// <param name="bufferSize">The size of the given buffer</param>
		/// <returns>The length of the author name or <c>-1</c> if the given buffer wasn't big enough</returns>
		[UnmanagedCallersOnly(EntryPoint = "rsml_get_api_documentation_url")]
		public static int GetApiDocumentationUrl(
			byte* buffer,
			int bufferSize
		)
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
		public static int GetApiVersion(
			byte* buffer,
			int bufferSize
		)
		{

			if (bufferSize < apiVersion.Length)
				return -1;

			for (int i = 0; i < apiVersion.Length; i++)
				buffer[i] = apiVersion[i];

			return apiVersion.Length;

		}

		private static readonly byte[] apiVersion = "2.1.0"u8.ToArray();

		private static readonly byte[] authorName = "OceanApocalypseStudios\0\0"u8.ToArray();

		private static readonly byte[] docsLink = "https://oceanapocalypsestudios.org/RedSeaMarkupLanguage/"u8.ToArray();

	}

}
