using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Tests.Sources;

/// <summary>
/// Tests for the official-provided <see cref="ReadOnlyStringBuffer"/> class.
/// </summary>
public class ReadOnlyStringBufferTests
{
	private const string TestString01 = "Hey\r\nThis\rIs\u2029A Test \n Method\r\n\r\n.\u2028";
	private const string TestString02 = "Hey\r\nThis\rIs\u2029A Test \n Method\r\n\r\n.";

	/*
	 * todo: test all buffer methods
	 * todo: expand on these tested methods (test them further)
	 */

	/*
	[Theory]
	public void CountUntilLineSeparator(string data, int index, int expectedLineC)
	{

	}
	*/

	[Theory]
	#region String ends with newline
	[InlineData(TestString01, 0, 0)]  // H in "Hey"
	[InlineData(TestString01, 3, 0)]  // CR in "Hey\r\n"
	[InlineData(TestString01, 4, 0)]  // LF in "Hey\r\n"
	[InlineData(TestString01, 5, 1)]  // T in "This"
	[InlineData(TestString01, 6, 1)]  // h in "This"
	[InlineData(TestString01, 13, 3)] // A in "A Test"
	[InlineData(TestString01, 15, 3)] // T in "Test"
	[InlineData(TestString01, 22, 4)] // M in "Method"
	[InlineData(TestString01, 28, 4)] // First CR in "\r\n\r\n."
	[InlineData(TestString01, 29, 4)] // First LF in "\r\n\r\n."
	[InlineData(TestString01, 30, 5)] // Second CR in "\r\n\r\n."
	[InlineData(TestString01, 31, 5)] // Second LF in "\r\n\r\n."
	[InlineData(TestString01, 32, 6)] // Dot/point in "\r\n\r\n."
	[InlineData(TestString01, 33, 6)] // U2028 in ".\u2028"
	[InlineData(TestString01, 34, 7)] // End of file
	#endregion
	#region String ends without newline
	[InlineData(TestString02, 0, 0)]  // H in "Hey"
	[InlineData(TestString02, 3, 0)]  // CR in "Hey\r\n"
	[InlineData(TestString02, 4, 0)]  // LF in "Hey\r\n"
	[InlineData(TestString02, 5, 1)]  // T in "This"
	[InlineData(TestString02, 6, 1)]  // h in "This"
	[InlineData(TestString02, 13, 3)] // A in "A Test"
	[InlineData(TestString02, 15, 3)] // T in "Test"
	[InlineData(TestString02, 22, 4)] // M in "Method"
	[InlineData(TestString02, 28, 4)] // First CR in "\r\n\r\n."
	[InlineData(TestString02, 29, 4)] // First LF in "\r\n\r\n."
	[InlineData(TestString02, 30, 5)] // Second CR in "\r\n\r\n."
	[InlineData(TestString02, 31, 5)] // Second LF in "\r\n\r\n."
	[InlineData(TestString02, 32, 6)] // Dot/point in "\r\n\r\n."
	[InlineData(TestString02, 33, 7)] // EOF
	#endregion
	public void GetLineNumberForIndex(string data, int index, int expectedLineCount)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		buffer.BuildCache();
		Assert.Equal(expectedLineCount, buffer.GetLineNumberFromIndex(index));
	}

	[Theory]
	#region String ends with newline
	[InlineData(TestString01, 0, 3)] // "Hey"
	[InlineData(TestString01, 1, 4)] // "This"
	[InlineData(TestString01, 2, 2)] // "Is"
	[InlineData(TestString01, 3, 7)] // "A Test "
	[InlineData(TestString01, 4, 7)] // " Method"
	[InlineData(TestString01, 5, 0)] // ""
	[InlineData(TestString01, 6, 1)] // "."
	#endregion
	#region String ends without newline
	[InlineData(TestString02, 0, 3)] // "Hey"
	[InlineData(TestString02, 1, 4)] // "This"
	[InlineData(TestString02, 2, 2)] // "Is"
	[InlineData(TestString02, 3, 7)] // "A Test "
	[InlineData(TestString02, 4, 7)] // " Method"
	[InlineData(TestString02, 5, 0)] // ""
	[InlineData(TestString02, 6, 1)] // "."
	#endregion
	public void GetLengthOfLine(string data, int lineNumber, int expectedLength)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		buffer.BuildCache();
		Assert.Equal(expectedLength, buffer.GetLengthOfLine(lineNumber));
	}

	[Theory]
	#region String ends with newline
	[InlineData(TestString01, 0, "Hey")]      // H in "Hey"
	[InlineData(TestString01, 3, "Hey")]      // CR in "Hey\r\n"
	[InlineData(TestString01, 4, "Hey")]      // LF in "Hey\r\n"
	[InlineData(TestString01, 5, "This")]     // T in "This"
	[InlineData(TestString01, 13, "A Test ")] // A in "A Test"
	[InlineData(TestString01, 15, "A Test ")] // T in "Test"
	[InlineData(TestString01, 22, " Method")] // M in "Method"
	[InlineData(TestString01, 28, " Method")] // First CR in "\r\n\r\n."
	[InlineData(TestString01, 29, " Method")] // First LF in "\r\n\r\n."
	[InlineData(TestString01, 30, "")]        // Second CR in "\r\n\r\n."
	[InlineData(TestString01, 31, "")]        // Second LF in "\r\n\r\n."
	[InlineData(TestString01, 32, ".")]       // Dot/point in Second LF in "\r\n\r\n."
	[InlineData(TestString01, 33, ".")]       // U2028 in ".\u2028"
	[InlineData(TestString01, 34, "")]        // End of file
	#endregion
	#region String ends without newline
	[InlineData(TestString02, 0, "Hey")]      // H in "Hey"
	[InlineData(TestString02, 3, "Hey")]      // CR in "Hey\r\n"
	[InlineData(TestString02, 4, "Hey")]      // LF in "Hey\r\n"
	[InlineData(TestString02, 5, "This")]     // T in "This"
	[InlineData(TestString02, 13, "A Test ")] // A in "A Test"
	[InlineData(TestString02, 15, "A Test ")] // T in "Test"
	[InlineData(TestString02, 22, " Method")] // M in "Method"
	[InlineData(TestString02, 28, " Method")] // First CR in "\r\n\r\n."
	[InlineData(TestString02, 29, " Method")] // First LF in "\r\n\r\n."
	[InlineData(TestString02, 30, "")]        // Second CR in "\r\n\r\n."
	[InlineData(TestString02, 31, "")]        // Second LF in "\r\n\r\n."
	[InlineData(TestString02, 32, ".")]       // Dot/point in Second LF in "\r\n\r\n."
	[InlineData(TestString02, 33, "")]        // End of file
	#endregion
	public void TryGetLineFromIndex(string data, int index, string expectedLine)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		buffer.BuildCache();

		Span<char> alloc = stackalloc char[expectedLine.Length];
		Assert.True(buffer.TryGetLineFromIndex(index, alloc, out int written));

		Assert.Equal(expectedLine, alloc.ToString());
		Assert.Equal(expectedLine.Length, written);
	}
}
