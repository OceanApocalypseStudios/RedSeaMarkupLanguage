using OceanApocalypseStudios.RSML.Sources.Buffers;

namespace OceanApocalypseStudios.RSML.Tests
{

	[TestClass]
	public class QuickFireTesting
	{

		[TestMethod]
		[DataRow(0, "Hello", 2, 0)]
		[DataRow(1, "A\r\nB", 1, 0)]
		[DataRow(2, "A\r\nB", 2, 0)]
		[DataRow(3, "\r\nHello", 0, 0)]
		[DataRow(4, "\r\nHello", 1, 0)]
		[DataRow(5, "A\nB", 1, 0)]
		[DataRow(6, "A\rB", 1, 0)]
		[DataRow(7, "Line1\r\nLine2\r\nLine3", 7, 1)]
		[DataRow(8, "Start\u2028Middle\nEnd\r\nLast", 10, 1)]
		public void ItFreakingWorks(int _, string test, int index, int expected)
		{

			StringBuffer buffer = new(test);
			var b = buffer.TryGetSourceLocation(index, out var location);

			Assert.IsTrue(b);
			Assert.AreEqual(expected, location.Line);

		}

	}

}
