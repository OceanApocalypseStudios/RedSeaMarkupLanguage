using System.Text;

using OceanApocalypseStudios.RSML.Native;


namespace OceanApocalypseStudios.RSML.Tests
{

	public unsafe partial class NativeTests
	{

		[Fact]
		public void GetApiAuthor_SameAsApiAuthor()
		{

			var nativeAuthorName = getApiAuthor();
			Assert.True(nativeAuthorName.byteCount > 0);

			Assert.Equal(Exports.AuthorName, Encoding.Default.GetString(nativeAuthorName.buffer, nativeAuthorName.byteCount));

		}

		[Fact]
		public void GetApiVersion_SameAsApiVersion()
		{

			var nativeApiVersion = getApiVersion();
			Assert.True(nativeApiVersion.byteCount > 0);

			Assert.Equal(Exports.ApiVersion, Encoding.Default.GetString(nativeApiVersion.buffer, nativeApiVersion.byteCount));

		}

		[Fact]
		public void GetDocumentationUrl_SameAsDocumentationUrl()
		{

			var nativeDocsUrl = getDocsUrl();
			Assert.True(nativeDocsUrl.byteCount > 0);

			Assert.Equal(Exports.DocumentationUrl, Encoding.Default.GetString(nativeDocsUrl.buffer, nativeDocsUrl.byteCount));

		}

	}

}
