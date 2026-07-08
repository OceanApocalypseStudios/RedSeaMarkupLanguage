using OceanApocalypseStudios.RSML.Sources.Buffers;


namespace OceanApocalypseStudios.RSML.Tests.Sources;

[TestClass]
public class ReadOnlyStringBufferTests
{
	private const string TestString01 = "Hey\r\nThis\rIs\u2029A Test \n Method\r\n\r\n.\u2028";

	/*
	 * todo: test all buffer methods (close open issues first)
	 * todo: expand on these tested methods (test them further)
	 */

	[TestMethod]
	[DataRow(TestString01, 0, 0)]  // H in "Hey"
	[DataRow(TestString01, 3, 0)]  // CR in "Hey\r\n"
	[DataRow(TestString01, 4, 0)]  // LF in "Hey\r\n"
	[DataRow(TestString01, 5, 1)]  // T in "This"
	[DataRow(TestString01, 6, 1)]  // h in "This"
	[DataRow(TestString01, 13, 3)] // A in "A Test"
	[DataRow(TestString01, 15, 3)] // T in "Test"
	[DataRow(TestString01, 22, 4)] // M in "Method"
	[DataRow(TestString01, 28, 4)] // First CR in "\r\n\r\n."
	[DataRow(TestString01, 29, 4)] // First LF in "\r\n\r\n."
	[DataRow(TestString01, 30, 5)] // Second CR in "\r\n\r\n."
	[DataRow(TestString01, 31, 5)] // Second LF in "\r\n\r\n."
	[DataRow(TestString01, 32, 6)] // Dot/point in "\r\n\r\n."
	[DataRow(TestString01, 33, 6)] // U2028 in ".\u2028"
	[DataRow(TestString01, 34, 7)] // End of file
	public void GetLineNumberForIndex(string data, int index, int expectedLineCount)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		buffer.BuildCache();
		Assert.AreEqual(expectedLineCount, buffer.GetLineNumberForIndex(index));
	}

	[TestMethod]
	[DataRow(TestString01, 0, 3)] // "Hey"
	[DataRow(TestString01, 1, 4)] // "This"
	[DataRow(TestString01, 2, 2)] // "Is"
	[DataRow(TestString01, 3, 7)] // "A Test "
	[DataRow(TestString01, 4, 7)] // " Method"
	[DataRow(TestString01, 5, 0)] // ""
	[DataRow(TestString01, 6, 1)] // "."
	[DataRow(TestString01, 7, 0)] // EOF - empty by default
	public void GetLengthOfLine(string data, int lineNumber, int expectedLength)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		buffer.BuildCache();
		Assert.AreEqual(expectedLength, buffer.GetLengthOfLine(lineNumber));
	}

	[TestMethod]
	[DataRow(TestString01, 0, "Hey")]      // H in "Hey"
	[DataRow(TestString01, 3, "Hey")]      // CR in "Hey\r\n"
	[DataRow(TestString01, 4, "Hey")]      // LF in "Hey\r\n"
	[DataRow(TestString01, 5, "This")]     // T in "This"
	[DataRow(TestString01, 13, "A Test ")] // A in "A Test"
	[DataRow(TestString01, 15, "A Test ")] // T in "Test"
	[DataRow(TestString01, 22, " Method")] // M in "Method"
	[DataRow(TestString01, 28, " Method")] // First CR in "\r\n\r\n."
	[DataRow(TestString01, 29, " Method")] // First LF in "\r\n\r\n."
	[DataRow(TestString01, 30, "")]        // Second CR in "\r\n\r\n."
	[DataRow(TestString01, 31, "")]        // Second LF in "\r\n\r\n."
	[DataRow(TestString01, 32, ".")]       // Dot/point in Second LF in "\r\n\r\n."
	[DataRow(TestString01, 33, ".")]       // U2028 in ".\u2028"
	[DataRow(TestString01, 34, "")]        // End of file
	public void TryGetLineFromIndex(string data, int index, string expectedLine)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		buffer.BuildCache();

		Span<char> alloc = stackalloc char[expectedLine.Length];
		Assert.IsTrue(buffer.TryGetLineFromIndex(index, alloc, out int written));

		Assert.AreEqual(expectedLine, alloc.ToString());
		Assert.AreEqual(expectedLine.Length, written);
	}
}
