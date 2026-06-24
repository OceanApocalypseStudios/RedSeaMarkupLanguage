using System.Text;

using OceanApocalypseStudios.RSML.Analyzer;
using OceanApocalypseStudios.RSML.Analyzer.Semantics;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Native;
using OceanApocalypseStudios.RSML.Native.Structures;


namespace OceanApocalypseStudios.RSML.Tests
{

	public unsafe partial class NativeTests
	{

		[Theory]
		[InlineData("-> defined == 11 any \"Test A\"")]
		[InlineData("-> any != 11 arm64 \"Test B\"")]
		[InlineData("# Test C\r\n@Void")]
		[InlineData("@Void \"Test D\"")]
		[InlineData("!> windows >= 10 arm64 \"Test E\"")]
		[InlineData("@ThrowError \"Test F1\"\n-> windows <= 7 x86 \"Test F2\"\r\n")]
		[InlineData("!> ubuntu any x86 \"Test G\"")]
		[InlineData("-> archlinux defined x64 \"Test H\"")]
		[InlineData("-> templeos any x69 \"Test I\"")] // invalid line
		public void ValidateRsml_SameAsManagedValidator(string data)
		{

			var alloc = (delegate* unmanaged[Cdecl]<byte*, int, int>)&ToolchainExports.AllocRsmlBuffer;
			var tokenize = (delegate* unmanaged[Cdecl]<nint, int>)&ToolchainExports.TokenizeRsmlLine;
			var normalize = (delegate* unmanaged[Cdecl]<nint, nint, int>)&ToolchainExports.NormalizeRsmlLine;
			var validate = (delegate* unmanaged[Cdecl]<nint, int>)&ToolchainExports.ValidateRsmlLine;
			var cleanup = (delegate* unmanaged[Cdecl]<int>)&ToolchainExports.Cleanup;

			fixed (byte* buffer = Encoding.Default.GetBytes(data))
			{

				#region Setup

				alloc(buffer, Encoding.Default.GetByteCount(data));

				NativeLine nativeLine = new()
				{
					item1 = SyntaxToken.Empty.ToNativeToken(),
					item2 = SyntaxToken.Empty.ToNativeToken(),
					item3 = SyntaxToken.Empty.ToNativeToken(),
					item4 = SyntaxToken.Empty.ToNativeToken(),
					item5 = SyntaxToken.Empty.ToNativeToken(),
					item6 = SyntaxToken.Empty.ToNativeToken(),
					item7 = SyntaxToken.Empty.ToNativeToken(),
					item8 = SyntaxToken.Empty.ToNativeToken()
				};

				NativeLine normalizedLine = new()
				{
					item1 = SyntaxToken.Empty.ToNativeToken(),
					item2 = SyntaxToken.Empty.ToNativeToken(),
					item3 = SyntaxToken.Empty.ToNativeToken(),
					item4 = SyntaxToken.Empty.ToNativeToken(),
					item5 = SyntaxToken.Empty.ToNativeToken(),
					item6 = SyntaxToken.Empty.ToNativeToken(),
					item7 = SyntaxToken.Empty.ToNativeToken(),
					item8 = SyntaxToken.Empty.ToNativeToken()
				};

				var nativeLinePtr = &nativeLine;
				var normalizedLinePtr = &normalizedLine;

				tokenize((nint)nativeLinePtr);
				normalize((nint)nativeLinePtr, (nint)normalizedLinePtr);

				#endregion

				// normalization happens here
				int errorCode = validate((nint)normalizedLinePtr);
				Assert.True(errorCode is 0 or 1); // success (valid or invalid lines)

				// managed normalization happens here - to be compared
				DualTextBuffer managedBuffer = new(data);
				var managedNormalizedLine = Lexer.TokenizeLine(managedBuffer);
				Normalizer.NormalizeLine(ref managedNormalizedLine, out _);
				bool isManagedLineValid = true;

				try
				{

					Validator.ValidateLine(managedNormalizedLine, managedBuffer);

				}
				catch (InvalidRsmlSyntax)
				{

					isManagedLineValid = false;

				}

				if (isManagedLineValid !=
					(errorCode == 0
						 ? true
						 : false))
					Assert.Fail("isManagedLineValid doesn't match boolean state of errorCode");

			}

			cleanup();

		}

	}

}
