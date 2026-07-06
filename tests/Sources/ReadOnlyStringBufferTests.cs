using OceanApocalypseStudios.RSML.Sources.Buffers;


namespace OceanApocalypseStudios.RSML.Tests.Sources;

[TestClass]
public class ReadOnlyStringBufferTests
{
	private const string TestString01 = "Hey\r\nThis\rIs\u2029A Test \n Method\r\n\r\n.\u2028";

	[TestMethod]
	[DataRow(TestString01, 0, 0)]  // H in "Hey"
	[DataRow(TestString01, 3, 1)]  // CR in "Hey\r\n"
	[DataRow(TestString01, 4, 1)]  // LF in "Hey\r\n"
	[DataRow(TestString01, 5, 1)]  // T in "This"
	[DataRow(TestString01, 13, 3)] // A in "A Test"
	[DataRow(TestString01, 15, 3)] // T in "Test"
	[DataRow(TestString01, 22, 4)] // M in "Method"
	[DataRow(TestString01, 29, 5)] // First LF in "\r\n\r\n."
	[DataRow(TestString01, 30, 6)] // Second CR in "\r\n\r\n."
	[DataRow(TestString01, 31, 6)] // Second LF in "\r\n\r\n."
	[DataRow(TestString01, 33, 7)] // End of file
	public void CountLinesBefore_CountsCorrectly(string data, int index, int expectedLineCount)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		buffer.BuildCache();
		Assert.AreEqual(expectedLineCount, buffer.CountLinesBefore(index));
	}
}
