using System.Diagnostics;

using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Tests.Sources;

/// <summary>
/// Tests for the official-provided <see cref="ReadOnlyStringBuffer"/> class.
/// </summary>
public class ReadOnlyStringBufferTests
{
	private const string TestString01 = "Hey\r\nThis\rIs\u2029A Test \n Method\r\n\r\n.\u2028";
	private const string TestString02 = "Hey\r\nThis\rIs\u2029A Test \n Method\r\n\r\n.";
	private const string TestString03 = "This  string     has a lotofwhitespace     charact\u2029\u2028ers out of\r\nnowhere !! ";
	private const string TestString04 = "this STRING MIXES a LOT\n of \u2029dIffereNT cas1ngs RAND0mLy!?";
	private const string TestString05 = "!rrrrrrrrr";
	private const string TestString06 = "r!!!!!!!!!";

	[Theory]
	#region String ends with newline
	[InlineData(TestString01, 0, 3, true)]  // H in "Hey"
	[InlineData(TestString01, 3, 0, true)]  // CR in "Hey\r\n"
	[InlineData(TestString01, 4, 0, false)]  // LF in "Hey\r\n"
	[InlineData(TestString01, 5, 4, false)]  // T in "This"
	[InlineData(TestString01, 6, 3, false)]  // h in "This"
	[InlineData(TestString01, 13, 7, false)] // A in "A Test"
	[InlineData(TestString01, 15, 5, false)] // T in "Test"
	[InlineData(TestString01, 22, 6, true)] // M in "Method"
	[InlineData(TestString01, 28, 0, true)] // First CR in "\r\n\r\n."
	[InlineData(TestString01, 29, 0, false)] // First LF in "\r\n\r\n."
	[InlineData(TestString01, 30, 0, true)] // Second CR in "\r\n\r\n."
	[InlineData(TestString01, 31, 0, false)] // Second LF in "\r\n\r\n."
	[InlineData(TestString01, 32, 1, false)] // Dot/point in "\r\n\r\n."
	[InlineData(TestString01, 33, 0, false)] // U2028 in ".\u2028"
	[InlineData(TestString01, 34, 0, false)] // End of file
	#endregion
	#region String ends without newline
	[InlineData(TestString02, 0, 3, true)]  // H in "Hey"
	[InlineData(TestString02, 3, 0, true)]  // CR in "Hey\r\n"
	[InlineData(TestString02, 4, 0, false)]  // LF in "Hey\r\n"
	[InlineData(TestString02, 5, 4, false)]  // T in "This"
	[InlineData(TestString02, 6, 3, false)]  // h in "This"
	[InlineData(TestString02, 13, 7, false)] // A in "A Test"
	[InlineData(TestString02, 15, 5, false)] // T in "Test"
	[InlineData(TestString02, 22, 6, true)] // M in "Method"
	[InlineData(TestString02, 28, 0, true)] // First CR in "\r\n\r\n."
	[InlineData(TestString02, 29, 0, false)] // First LF in "\r\n\r\n."
	[InlineData(TestString02, 30, 0, true)] // Second CR in "\r\n\r\n."
	[InlineData(TestString02, 31, 0, false)] // Second LF in "\r\n\r\n."
	[InlineData(TestString02, 32, 0, false)] // Dot/point in "\r\n\r\n."
	[InlineData(TestString02, 33, 0, false)] // End of file
	#endregion
	public void CountUntilLineSeparator(string data, int index, int expectedCount, bool expectedCrLf)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		buffer.BuildCache();
		Assert.Equal(expectedCount, buffer.CountUntilLineSeparator(index, out bool actualCrLf));
		Assert.Equal(expectedCrLf, actualCrLf);
	}

	[Theory]
	#region Attributes
	[InlineData(0)]
	[InlineData(2)]
	[InlineData(14)]
	[InlineData(99)]
	[InlineData(-10)]
	#endregion
	public void CountUntilLineSeparator_ThrowsIfEmpty(int index)
	{
		var buffer = new ReadOnlyStringBuffer(String.Empty);
		bool isCrLf = true;
		Assert.Throws<BufferException>(() => buffer.CountUntilLineSeparator(index, out isCrLf));
		Assert.False(isCrLf);
	}

	[Theory]
	#region Attributes
	[InlineData(0, false)]
	[InlineData(2, false)]
	[InlineData(14, false)]
	[InlineData(99, true)]
	[InlineData(34, false)] // this method allows for EOF convention
	[InlineData(35, true)]
	[InlineData(-10, false)]
	[InlineData(-34, false)]
	[InlineData(-35, true)]
	#endregion
	public void CountUntilLineSeparator_ThrowsIfOutOfRange(int index, bool throws)
	{
		var buffer = new ReadOnlyStringBuffer(TestString01);
		bool isCrLf = true;

		if (throws)
		{
			Debug.WriteLine("Expecting an exception...");
			Assert.Throws<IndexOutOfRangeException>(() => buffer.CountUntilLineSeparator(index, out isCrLf));
			Assert.False(isCrLf);
		}
		else
		{
			Debug.WriteLine("Not expecting an exception...");
			buffer.CountUntilLineSeparator(index, out isCrLf); // if this throws, test fails
		}
	}

	[Theory]
	#region Attributes
	[InlineData(TestString03, 0, 0)]  // T in "This"
	[InlineData(TestString03, 2, 0)]  // i in "This"
	[InlineData(TestString03, 3, 0)]  // s in "This"
	[InlineData(TestString03, 4, 2)]  // whitespace after "This"
	[InlineData(TestString03, 5, 1)]  // whitespace after "This"
	[InlineData(TestString03, 6, 0)]  // s in "string"
	[InlineData(TestString03, 9, 0)]  // i in "string"
	[InlineData(TestString03, 11, 0)] // g in "string"
	[InlineData(TestString03, 12, 5)] // whitespace after "string"
	[InlineData(TestString03, 14, 3)] // whitespace after "string"
	[InlineData(TestString03, 15, 2)] // whitespace after "string"
	[InlineData(TestString03, 16, 1)] // whitespace after "string"
	[InlineData(TestString03, 20, 1)] // whitespace after "has"
	[InlineData(TestString03, 21, 0)] // "a" after "has"
	[InlineData(TestString03, 28, 0)] // w in "lotofwhitespace"
	[InlineData(TestString03, 38, 5)] // whitespace after "lotofwhitespace"
	[InlineData(TestString03, 39, 4)] // whitespace after "lotofwhitespace"
	[InlineData(TestString03, 42, 1)] // whitespace after "lotofwhitespace"
	[InlineData(TestString03, 46, 0)] // r in "charact\u2029\u2028"
	[InlineData(TestString03, 50, 2)] // U2029 in "charact\u2029\u2028"
	[InlineData(TestString03, 51, 1)] // U2028 in "charact\u2029\u2028"
	[InlineData(TestString03, 52, 0)] // e in "\u2029\u2028ers"
	[InlineData(TestString03, 59, 1)] // whitespace after "out"
	[InlineData(TestString03, 62, 2)] // CR in "\r\nnowhere"
	[InlineData(TestString03, 63, 1)] // LF in "\r\nnowhere"
	[InlineData(TestString03, 65, 0)] // o in "nowhere"
	[InlineData(TestString03, 69, 0)] // r in "nowhere"
	[InlineData(TestString03, 70, 0)] // last e in "nowhere"
	[InlineData(TestString03, 71, 1)] // whitespace after "nowhere"
	[InlineData(TestString03, 73, 0)] // second exclamation mark
	[InlineData(TestString03, 74, 1)] // whitespace after exclamation marks
	[InlineData(TestString03, 75, 0)] // End of file
	#endregion
	public void CountUntilNotWhitespace(string data, int index, int expectedCount)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		buffer.BuildCache();
		Assert.Equal(expectedCount, buffer.CountUntilNotWhitespace(index));
	}

	[Theory]
	#region Attributes
	[InlineData(0)]
	[InlineData(2)]
	[InlineData(14)]
	[InlineData(99)]
	[InlineData(-10)]
	#endregion
	public void CountUntilNotWhitespace_ThrowsIfEmpty(int index)
	{
		var buffer = new ReadOnlyStringBuffer(String.Empty);
		Assert.Throws<BufferException>(() => buffer.CountUntilWhitespace(index));
	}

	[Theory]
	#region Attributes
	[InlineData(0, false)]
	[InlineData(2, false)]
	[InlineData(14, false)]
	[InlineData(99, true)]
	[InlineData(34, false)] // this method allows for EOF convention
	[InlineData(35, true)]
	[InlineData(-10, false)]
	[InlineData(-34, false)]
	[InlineData(-35, true)]
	#endregion
	public void CountUntilNotWhitespace_ThrowsIfOutOfRange(int index, bool throws)
	{
		var buffer = new ReadOnlyStringBuffer(TestString01);

		if (throws)
		{
			Debug.WriteLine("Expecting an exception...");
			Assert.Throws<IndexOutOfRangeException>(() => buffer.CountUntilNotWhitespace(index));
		}
		else
		{
			Debug.WriteLine("Not expecting an exception...");
			buffer.CountUntilNotWhitespace(index); // if this throws, test fails
		}
	}

	[Theory]
	#region Attributes
	[InlineData(TestString03, 0, 4)]  // T in "This"
	[InlineData(TestString03, 2, 2)]  // i in "This"
	[InlineData(TestString03, 3, 1)]  // s in "This"
	[InlineData(TestString03, 4, 0)]  // whitespace after "This"
	[InlineData(TestString03, 5, 0)]  // whitespace after "This"
	[InlineData(TestString03, 6, 6)]  // s in "string"
	[InlineData(TestString03, 9, 3)]  // i in "string"
	[InlineData(TestString03, 11, 1)] // g in "string"
	[InlineData(TestString03, 12, 0)] // whitespace after "string"
	[InlineData(TestString03, 14, 0)] // whitespace after "string"
	[InlineData(TestString03, 15, 0)] // whitespace after "string"
	[InlineData(TestString03, 16, 0)] // whitespace after "string"
	[InlineData(TestString03, 20, 0)] // whitespace after "has"
	[InlineData(TestString03, 21, 1)] // "a" after "has"
	[InlineData(TestString03, 28, 10)] // w in "lotofwhitespace"
	[InlineData(TestString03, 38, 0)] // whitespace after "lotofwhitespace"
	[InlineData(TestString03, 39, 0)] // whitespace after "lotofwhitespace"
	[InlineData(TestString03, 42, 0)] // whitespace after "lotofwhitespace"
	[InlineData(TestString03, 46, 4)] // r in "charact\u2029\u2028"
	[InlineData(TestString03, 50, 0)] // U2029 in "charact\u2029\u2028"
	[InlineData(TestString03, 51, 0)] // U2028 in "charact\u2029\u2028"
	[InlineData(TestString03, 52, 3)] // e in "\u2029\u2028ers"
	[InlineData(TestString03, 59, 0)] // whitespace after "out"
	[InlineData(TestString03, 62, 0)] // CR in "\r\nnowhere"
	[InlineData(TestString03, 63, 0)] // LF in "\r\nnowhere"
	[InlineData(TestString03, 65, 6)] // o in "nowhere"
	[InlineData(TestString03, 69, 2)] // r in "nowhere"
	[InlineData(TestString03, 70, 1)] // last e in "nowhere"
	[InlineData(TestString03, 71, 0)] // whitespace after "nowhere"
	[InlineData(TestString03, 73, 1)] // second exclamation mark
	[InlineData(TestString03, 74, 0)] // whitespace after exclamation marks
	[InlineData(TestString03, 75, 0)] // End of file
	#endregion
	public void CountUntilWhitespace(string data, int index, int expectedCount)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		buffer.BuildCache();
		Assert.Equal(expectedCount, buffer.CountUntilWhitespace(index));
	}

	[Theory]
	#region Attributes
	[InlineData(0)]
	[InlineData(2)]
	[InlineData(14)]
	[InlineData(99)]
	[InlineData(-10)]
	#endregion
	public void CountUntilWhitespace_ThrowsIfEmpty(int index)
	{
		var buffer = new ReadOnlyStringBuffer(String.Empty);
		Assert.Throws<BufferException>(() => buffer.CountUntilWhitespace(index));
	}

	[Theory]
	#region Attributes
	[InlineData(0, false)]
	[InlineData(2, false)]
	[InlineData(14, false)]
	[InlineData(99, true)]
	[InlineData(34, false)] // this method allows for EOF convention
	[InlineData(35, true)]
	[InlineData(-10, false)]
	[InlineData(-34, false)]
	[InlineData(-35, true)]
	#endregion
	public void CountUntilWhitespace_ThrowsIfOutOfRange(int index, bool throws)
	{
		var buffer = new ReadOnlyStringBuffer(TestString01);

		if (throws)
		{
			Debug.WriteLine("Expecting an exception...");
			Assert.Throws<IndexOutOfRangeException>(() => buffer.CountUntilWhitespace(index));
		}
		else
		{
			Debug.WriteLine("Not expecting an exception...");
			buffer.CountUntilWhitespace(index); // if this throws, test fails
		}
	}

	[Theory]
	#region Attributes
	[InlineData(TestString01)]
	[InlineData(TestString02)]
	[InlineData(TestString03)]
	[InlineData(TestString04)]
	#endregion
	public void CountWhile_SameAsLengthIfAlwaysTrue(string data)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		Assert.Equal(data.Length, buffer.CountWhile((_, _) => true, 0));
	}

	[Theory]
	#region Regular string
	[InlineData(TestString03, 0, 8)] // T in "This
	[InlineData(TestString03, 2, 6)] // i in "This"
	[InlineData(TestString03, 5, 3)] // whitespace before "string"
	[InlineData(TestString03, 7, 1)] // t in "This  string"
	[InlineData(TestString03, 8, 0)] // r in "This  string"
	[InlineData(TestString03, 9, 37)] // i in "string"
	[InlineData(TestString03, 10, 36)]
	[InlineData(TestString03, 11, 35)]
	[InlineData(TestString03, 15, 31)]
	[InlineData(TestString03, 18, 28)]
	[InlineData(TestString03, 20, 26)]
	[InlineData(TestString03, 22, 24)]
	[InlineData(TestString03, 24, 22)]
	[InlineData(TestString03, 29, 17)]
	[InlineData(TestString03, 31, 15)]
	[InlineData(TestString03, 34, 12)]
	[InlineData(TestString03, 37, 9)]
	[InlineData(TestString03, 41, 5)]
	[InlineData(TestString03, 44, 2)]
	[InlineData(TestString03, 45, 1)]
	[InlineData(TestString03, 46, 0)]
	[InlineData(TestString03, 47, 6)] // second in "charact"
	[InlineData(TestString03, 48, 5)]
	[InlineData(TestString03, 50, 3)]
	[InlineData(TestString03, 51, 2)]
	[InlineData(TestString03, 53, 0)]
	[InlineData(TestString03, 54, 15)] // s in "ers "
	[InlineData(TestString03, 55, 14)]
	[InlineData(TestString03, 58, 11)]
	[InlineData(TestString03, 61, 8)]
	[InlineData(TestString03, 63, 6)]
	[InlineData(TestString03, 64, 5)]
	[InlineData(TestString03, 67, 2)]
	[InlineData(TestString03, 68, 1)]
	[InlineData(TestString03, 69, 0)]
	[InlineData(TestString03, 70, 5)]
	[InlineData(TestString03, 72, 3)] // after last r in the whole string
	[InlineData(TestString03, 74, 1)] // before End of file
	[InlineData(TestString03, 75, 0)] // End of file
	#endregion
	#region String with several r's
	[InlineData(TestString05, 0, 1)] // "!"
	[InlineData(TestString05, 1, 0)]
	[InlineData(TestString05, 4, 0)]
	[InlineData(TestString05, 7, 0)]
	[InlineData(TestString05, 9, 0)]
	[InlineData(TestString05, 10, 0)] // EOF
	#endregion
	#region String with a single r
	[InlineData(TestString06, 0, 0)] // "r"
	[InlineData(TestString06, 1, 9)]
	[InlineData(TestString06, 4, 6)]
	[InlineData(TestString06, 7, 3)]
	[InlineData(TestString06, 9, 1)]
	[InlineData(TestString06, 10, 0)] // EOF
	#endregion
	public void CountWhile_CountsWhileNotLowercaseR(string data, int index, int expectedCount)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		Assert.Equal(expectedCount, buffer.CountWhile((_, c) => c != 'r', index));
	}

	[Theory]
	#region Attributes
	[InlineData(TestString04, 0, 0)] // t in "this STRING"
	[InlineData(TestString04, 3, 0)] // s in "this STRING"
	[InlineData(TestString04, 4, 14)] // whitespace after "this"
	[InlineData(TestString04, 5, 13)] // S in "STRING"
	[InlineData(TestString04, 6, 12)] // T in "STRING"
	[InlineData(TestString04, 8, 10)] // I in "STRING"
	[InlineData(TestString04, 9, 9)] // N in "STRING"
	[InlineData(TestString04, 10, 8)] // G in "STRING"
	[InlineData(TestString04, 11, 7)] // whitespace after "STRING"
	[InlineData(TestString04, 12, 6)] // M in "MIXES"
	[InlineData(TestString04, 14, 4)] // X in "MIXES"
	[InlineData(TestString04, 16, 2)] // S in "MIXES"
	[InlineData(TestString04, 17, 1)] // whitespace after "MIXES"
	[InlineData(TestString04, 18, 0)] // "a" surrounded by whitespace
	[InlineData(TestString04, 19, 6)] // whitespace after sole "a"
	[InlineData(TestString04, 21, 4)] // O in "LOT"
	[InlineData(TestString04, 22, 3)] // T in "LOT"
	[InlineData(TestString04, 23, 2)] // LF after "LOT"
	[InlineData(TestString04, 24, 1)] // whitespace after "LOT\n"
	[InlineData(TestString04, 26, 0)] // f in "of"
	[InlineData(TestString04, 27, 2)] // whitespace after "of"
	[InlineData(TestString04, 28, 1)] // U2029 after "of "
	[InlineData(TestString04, 29, 0)] // d in "dIffereNT"
	[InlineData(TestString04, 30, 1)] // I in "dIffereNT"
	[InlineData(TestString04, 31, 0)] // first f in "dIffereNT"
	[InlineData(TestString04, 32, 0)] // second f in "dIffereNT"
	[InlineData(TestString04, 35, 0)] // second e in "dIffereNT"
	[InlineData(TestString04, 36, 3)] // N in "dIffereNT"
	[InlineData(TestString04, 37, 2)] // T in "dIffereNT"
	[InlineData(TestString04, 38, 1)] // whitespace after "dIffereNT"
	[InlineData(TestString04, 39, 0)] // c in "cas1ngs"
	[InlineData(TestString04, 42, 0)] // 1 in "cas1ngs"
	[InlineData(TestString04, 46, 5)] // whitespace after "cas1ngs"
	[InlineData(TestString04, 49, 2)] // N in "RAND0mLy"
	[InlineData(TestString04, 51, 0)] // 0 in "RAND0mLy"
	[InlineData(TestString04, 55, 0)] // exclamation mark
	[InlineData(TestString04, 56, 0)] // question mark
	[InlineData(TestString04, 57, 0)] // End of file
	#endregion
	public void CountWhile_CountsWhileUppercaseOrWhitespace(string data, int index, int expectedCount)
	{
		var buffer = new ReadOnlyStringBuffer(data);
		Assert.Equal(expectedCount, buffer.CountWhile((_, c) => Char.IsWhiteSpace(c) || c.IsNewline() || Char.IsUpper(c), index));
	}

	[Theory]
	#region Attributes
	[InlineData(0)]
	[InlineData(2)]
	[InlineData(14)]
	[InlineData(99)]
	[InlineData(-10)]
	#endregion
	public void CountWhile_ThrowsIfEmpty(int index)
	{
		var buffer = new ReadOnlyStringBuffer(String.Empty);
		Assert.Throws<BufferException>(() => buffer.CountWhile((_, _) => true, index));
	}

	[Theory]
	#region Attributes
	[InlineData(0, false)]
	[InlineData(2, false)]
	[InlineData(14, false)]
	[InlineData(99, true)]
	[InlineData(34, false)] // this method allows for EOF convention
	[InlineData(35, true)]
	[InlineData(-10, false)]
	[InlineData(-34, false)]
	[InlineData(-35, true)]
	#endregion
	public void CountWhile_ThrowsIfOutOfRange(int index, bool throws)
	{
		var buffer = new ReadOnlyStringBuffer(TestString01);

		if (throws)
		{
			Debug.WriteLine("Expecting an exception...");
			Assert.Throws<IndexOutOfRangeException>(() => buffer.CountWhile((_, _) => true, index));
		}
		else
		{
			Debug.WriteLine("Not expecting an exception...");
			buffer.CountWhile((_, _) => true, index); // if this throws, test fails
		}
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
