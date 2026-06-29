using System.Text;

using OceanApocalypseStudios.RSML.Evaluation;
using OceanApocalypseStudios.RSML.Host;
using OceanApocalypseStudios.RSML.Native.Structures;


namespace OceanApocalypseStudios.RSML.Tests
{

	public unsafe partial class NativeTests
	{

		[TestMethod]
		public void NativeEvaluator_AnyWorksEvenIfUnknown()
		{

			string buffer = "-> osx any any \"Output!\"";
			int byteCount = Encoding.Default.GetByteCount(buffer);

			fixed (byte* bufferPtr = Encoding.Default.GetBytes(buffer))
			{

				var allocErrorCode = allocate(bufferPtr, byteCount);
				Assert.AreEqual(0, allocErrorCode);

			}

			NativeEvaluationResult result = new();
			NativeEvaluationResult* resultPtr = &result;

			var errorCode = evaluate((nint)resultPtr, 2, 0, -1, 0, 0); // osx, no version, no arch

			Assert.AreEqual(0, errorCode);
			Assert.AreEqual<byte>(1, result.wasMatchFound);
			Assert.AreEqual("Output!", buffer[result.matchValueStart..result.matchValueEnd]);

			cleanup();

		}

		[TestMethod]
		public void NativeEvaluator_ComparatorPlusWildcard_InvalidSyntax()
		{

			string buffer = "-> windows == defined defined \"Output!\"";
			int byteCount = Encoding.Default.GetByteCount(buffer);

			fixed (byte* bufferPtr = Encoding.Default.GetBytes(buffer))
			{

				var allocErrorCode = allocate(bufferPtr, byteCount);
				Assert.AreEqual(0, allocErrorCode);

			}

			NativeEvaluationResult result = new();
			NativeEvaluationResult* resultPtr = &result;

			var errorCode = evaluate((nint)resultPtr, 1, 0, 10, 3, 0); // win 10 x64

			Assert.AreEqual(-1, errorCode);
			Assert.AreEqual<byte>(0, result.wasMatchFound);

			cleanup();

		}

		[TestMethod]
		[DataRow("==", "Result A")]
		[DataRow(">=", "Result A")]
		[DataRow("<=", "Result A")]
		[DataRow(">", "Result B")]
		[DataRow("<", "Result B")]
		[DataRow("!=", "Result B")]
		public void NativeEvaluator_ComparatorWorks(
			string input,
			string? expected
		)
		{

			string buffer = $"-> windows {input} 10 defined \"Result A\"\n-> windows == 10 defined \"Result B\"\r\n";
			int byteCount = Encoding.Default.GetByteCount(buffer);

			fixed (byte* bufferPtr = Encoding.Default.GetBytes(buffer))
			{

				var allocErrorCode = allocate(bufferPtr, byteCount);
				Assert.AreEqual(0, allocErrorCode);

			}

			NativeEvaluationResult result = new();
			NativeEvaluationResult* resultPtr = &result;

			var errorCode = evaluate((nint)resultPtr, 1, 0, 10, 3, 0); // win 10 x64

			Assert.AreEqual(0, errorCode);
			Assert.AreEqual<byte>(1, result.wasMatchFound);
			Assert.AreEqual(expected, buffer[result.matchValueStart..result.matchValueEnd]);

			cleanup();

		}

		[TestMethod]
		[DataRow("-> windows 10 x64 \"Test A\"", 0)]
		[DataRow("-> windows 10 x86 \"Test B\"", 1)]
		[DataRow("-> windows <= 7 defined \"Test C\"", 1)]
		[DataRow("!> \"Test D\"", -5)]
		[DataRow("-> ubuntu defined any \"Test E\"", 1)]
		[DataRow("-> bindows =! -5 64arm \"Test F\"", -1)]
		[DataRow("-> defined < 11 any \"Test G\"", 0)]
		[DataRow("@ThrowError", -3)]
		[DataRow("@NonExistingAction ThisIsInvalidSyntax", -1)]
		[DataRow("@EndAll", 1)]
		[DataRow("@EndAll MatchValue", 0)]
		public void NativeEvaluator_ThrowsErrorCodesCorrectly(string buffer, int expected)
		{

			int byteCount = Encoding.Default.GetByteCount(buffer);

			fixed (byte* bufferPtr = Encoding.Default.GetBytes(buffer))
			{

				var allocErrorCode = allocate(bufferPtr, byteCount);
				Assert.AreEqual(0, allocErrorCode);

			}

			NativeEvaluationResult result = new();
			NativeEvaluationResult* resultPtr = &result;

			var errorCode = evaluate((nint)resultPtr, 1, 0, 10, 3, 0); // win 10 x64
			Assert.AreEqual(expected, errorCode);

			cleanup();

		}

		[TestMethod]
		[DataRow("-> debian \"This is the output...\"\n-> ubuntu \"...not this.\"")]
		[DataRow("@Void", true)]
		[DataRow("@Void\n", true)]
		[DataRow("@Void\n# Comment", true)]
		[DataRow("@Void\n# Comment\n", true)]
		[DataRow("@Void \n", true)]
		[DataRow("@Void ", true)]
		[DataRow("@Void Argument", true)]
		[DataRow("@EndAll\n-> linux \"Result C\"", true)]
		[DataRow("@EndAll\n-> linux \"Result C\"\n", true)]
		[DataRow("-> osx \"newlines are normalized\"\n-> osx \"are they really?\"\r\n-> ubuntu \"Ubuntu I guess\"\r\n")]
		[DataRow("-> any any any \"Being explicit is good too\"\n")]
		[DataRow("-> \"this will always return\"\n")]
		public void NativeEvaluator_SameAsManagedEvaluator(string content, bool expectNull = false)
		{

			int byteCount = Encoding.Default.GetByteCount(content);

			fixed (byte* bufferPtr = Encoding.Default.GetBytes(content))
			{

				var allocErrorCode = allocate(bufferPtr, byteCount);
				Assert.AreEqual(0, allocErrorCode);

			}

			NativeEvaluationResult result = new();
			NativeEvaluationResult* resultPtr = &result;

			var errorCode = evaluate((nint)resultPtr, 103, 1, 22, 2, 0); // ubuntu 22 (arm64)
			Assert.IsInRange(0, 1, errorCode);

			Evaluator evaluator = new(content);
			string? evaluationStr = evaluator.Evaluate(ubuntu22Arm64).MatchValue;

			if (expectNull)
			{

				Assert.IsNull(evaluationStr);
				Assert.IsTrue(result is { matchValueStart: -1, matchValueEnd: -1, wasMatchFound: 0 });
			
			}
			else
			{

				Assert.IsNotNull(evaluationStr);
				Assert.IsFalse(result.matchValueStart == -1 || result.matchValueEnd == -1 || result.wasMatchFound == 0);
				Assert.AreEqual(evaluationStr, content[result.matchValueStart..result.matchValueEnd]);

			}

			cleanup();

		}

		private static readonly HostInfo ubuntu22Arm64 = new(
			"ubuntu",
			"debian",
			"x64",
			22
		);

	}

}
