using System.Text;

using OceanApocalypseStudios.RSML.Evaluation;
using OceanApocalypseStudios.RSML.Host;
using OceanApocalypseStudios.RSML.Native.Structures;


namespace OceanApocalypseStudios.RSML.Tests
{

	public unsafe partial class NativeTests
	{

		[Fact]
		public void NativeEvaluator_AnyWorksEvenIfUnknown()
		{

			string buffer = "-> osx any any \"Output!\"";
			int byteCount = Encoding.Default.GetByteCount(buffer);

			fixed (byte* bufferPtr = Encoding.Default.GetBytes(buffer))
			{

				var allocErrorCode = allocate(bufferPtr, byteCount);
				Assert.Equal(0, allocErrorCode);

			}

			NativeEvaluationResult result = new();
			NativeEvaluationResult* resultPtr = &result;

			var errorCode = evaluate((nint)resultPtr, 2, 0, -1, 0, 0); // osx, no version, no arch

			Assert.Equal(0, errorCode);
			Assert.Equal<byte>(1, result.wasMatchFound);
			Assert.Equal("Output!", buffer[result.matchValueStart..result.matchValueEnd]);

			cleanup();

		}

		[Fact]
		public void NativeEvaluator_ComparatorPlusWildcard_InvalidSyntax()
		{

			string buffer = "-> windows == defined defined \"Output!\"";
			int byteCount = Encoding.Default.GetByteCount(buffer);

			fixed (byte* bufferPtr = Encoding.Default.GetBytes(buffer))
			{

				var allocErrorCode = allocate(bufferPtr, byteCount);
				Assert.Equal(0, allocErrorCode);

			}

			NativeEvaluationResult result = new();
			NativeEvaluationResult* resultPtr = &result;

			var errorCode = evaluate((nint)resultPtr, 1, 0, 10, 3, 0); // win 10 x64

			Assert.Equal(-1, errorCode);
			Assert.Equal<byte>(0, result.wasMatchFound);

			cleanup();

		}

		[Theory]
		[InlineData("==", "Result A")]
		[InlineData(">=", "Result A")]
		[InlineData("<=", "Result A")]
		[InlineData(">", "Result B")]
		[InlineData("<", "Result B")]
		[InlineData("!=", "Result B")]
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
				Assert.Equal(0, allocErrorCode);

			}

			NativeEvaluationResult result = new();
			NativeEvaluationResult* resultPtr = &result;

			var errorCode = evaluate((nint)resultPtr, 1, 0, 10, 3, 0); // win 10 x64

			Assert.Equal(0, errorCode);
			Assert.Equal<byte>(1, result.wasMatchFound);
			Assert.Equal(expected, buffer[result.matchValueStart..result.matchValueEnd]);

			cleanup();

		}

		[Theory]
		[InlineData("-> windows 10 x64 \"Test A\"", 0)]
		[InlineData("-> windows 10 x86 \"Test B\"", 1)]
		[InlineData("-> windows <= 7 defined \"Test C\"", 1)]
		[InlineData("!> \"Test D\"", -5)]
		[InlineData("-> ubuntu defined any \"Test E\"", 1)]
		[InlineData("-> bindows =! -5 64arm \"Test F\"", -1)]
		[InlineData("-> defined < 11 any \"Test G\"", 0)]
		[InlineData("@ThrowError", -3)]
		[InlineData("@NonExistingAction ThisIsInvalidSyntax", -1)]
		[InlineData("@EndAll", 1)]
		[InlineData("@EndAll MatchValue", 0)]
		public void NativeEvaluator_ThrowsErrorCodesCorrectly(string buffer, int expected)
		{

			int byteCount = Encoding.Default.GetByteCount(buffer);

			fixed (byte* bufferPtr = Encoding.Default.GetBytes(buffer))
			{

				var allocErrorCode = allocate(bufferPtr, byteCount);
				Assert.Equal(0, allocErrorCode);

			}

			NativeEvaluationResult result = new();
			NativeEvaluationResult* resultPtr = &result;

			var errorCode = evaluate((nint)resultPtr, 1, 0, 10, 3, 0); // win 10 x64
			Assert.Equal(expected, errorCode);

			cleanup();

		}

		[Theory]
		[InlineData("-> debian \"This is the output...\"\n-> ubuntu \"...not this.\"")]
		[InlineData("@Void", true)]
		[InlineData("@Void\n", true)]
		[InlineData("@Void\n# Comment", true)]
		[InlineData("@Void\n# Comment\n", true)]
		[InlineData("@Void \n", true)]
		[InlineData("@Void ", true)]
		[InlineData("@Void Argument", true)]
		[InlineData("@EndAll\n-> linux \"Result C\"", true)]
		[InlineData("@EndAll\n-> linux \"Result C\"\n", true)]
		[InlineData("-> osx \"newlines are normalized\"\n-> osx \"are they really?\"\r\n-> ubuntu \"Ubuntu I guess\"\r\n")]
		[InlineData("-> any any any \"Being explicit is good too\"\n")]
		[InlineData("-> \"this will always return\"\n")]
		public void NativeEvaluator_SameAsManagedEvaluator(string content, bool expectNull = false)
		{

			int byteCount = Encoding.Default.GetByteCount(content);

			fixed (byte* bufferPtr = Encoding.Default.GetBytes(content))
			{

				var allocErrorCode = allocate(bufferPtr, byteCount);
				Assert.Equal(0, allocErrorCode);

			}

			NativeEvaluationResult result = new();
			NativeEvaluationResult* resultPtr = &result;

			var errorCode = evaluate((nint)resultPtr, 103, 1, 22, 2, 0); // ubuntu 22 (arm64)
			Assert.InRange(errorCode, 0, 1);

			Evaluator evaluator = new(content);
			string? evaluationStr = evaluator.Evaluate(ubuntu22Arm64).MatchValue;

			if (expectNull)
			{

				Assert.Null(evaluationStr);
				Assert.True(result is { matchValueStart: -1, matchValueEnd: -1, wasMatchFound: 0 });
			
			}
			else
			{

				Assert.NotNull(evaluationStr);
				Assert.False(result.matchValueStart == -1 || result.matchValueEnd == -1 || result.wasMatchFound == 0);
				Assert.Equal(evaluationStr, content[result.matchValueStart..result.matchValueEnd]);

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
