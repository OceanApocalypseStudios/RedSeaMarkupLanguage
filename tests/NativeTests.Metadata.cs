using System.Text;

using OceanApocalypseStudios.RSML.Native;


namespace OceanApocalypseStudios.RSML.Tests
{

	public unsafe partial class NativeTests
	{

		[TestMethod]
		public void GetApiAuthor_SameAsApiAuthor()
		{

			var nativeAuthorName = getApiAuthor();
			Assert.IsGreaterThan(0, nativeAuthorName.byteCount);

			Assert.AreEqual(Exports.AuthorName, Encoding.Default.GetString(nativeAuthorName.buffer, nativeAuthorName.byteCount));

		}

		[TestMethod]
		public void GetApiVersion_SameAsApiVersion()
		{

			var nativeApiVersion = getApiVersion();
			Assert.IsGreaterThan(0, nativeApiVersion.byteCount);

			Assert.AreEqual(Exports.ApiVersion, Encoding.Default.GetString(nativeApiVersion.buffer, nativeApiVersion.byteCount));

		}

		[TestMethod]
		public void GetDocumentationUrl_SameAsDocumentationUrl()
		{

			var nativeDocsUrl = getDocsUrl();
			Assert.IsGreaterThan(0, nativeDocsUrl.byteCount);

			Assert.AreEqual(Exports.DocumentationUrl, Encoding.Default.GetString(nativeDocsUrl.buffer, nativeDocsUrl.byteCount));

		}

	}

}
