using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using OceanApocalypseStudios.RSML.Analyzer;
using OceanApocalypseStudios.RSML.Analyzer.Semantics;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Evaluation;
using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Machine;

using OceanApocalypseStudios.RSML.Native.Structures;


namespace OceanApocalypseStudios.RSML.Native
{

	/// <summary>
	/// C ABI exports for RSML toolchain components.
	/// </summary>
	public static unsafe class ToolchainExports
	{

		private static DualTextBuffer? buffer = null;
		private static nint lastErrorMessage = IntPtr.Zero;
		private static nint evaluationResult = IntPtr.Zero;

		#region Conversion Helpers

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static NativeRsmlToken FromManagedToNativeSyntaxToken(SyntaxToken token) => new()
		{
			kind = (byte)token.Kind,
			startIndex = token.BufferRange.Start.IsFromEnd ? (0 - token.BufferRange.Start.Value) : token.BufferRange.Start.Value,
			endIndex = token.BufferRange.End.IsFromEnd ? (0 - token.BufferRange.End.Value) : token.BufferRange.End.Value
		};

		internal static NativeRsmlLine FromManagedToNativeSyntaxLine(SyntaxLine line) => new()
		{
			item1 = FromManagedToNativeSyntaxToken(line.Item1),
			item2 = FromManagedToNativeSyntaxToken(line.Item2),
			item3 = FromManagedToNativeSyntaxToken(line.Item3),
			item4 = FromManagedToNativeSyntaxToken(line.Item4),
			item5 = FromManagedToNativeSyntaxToken(line.Item5),
			item6 = FromManagedToNativeSyntaxToken(line.Item6),
			item7 = FromManagedToNativeSyntaxToken(line.Item7),
			item8 = FromManagedToNativeSyntaxToken(line.Item8)
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static SyntaxToken FromNativeToManagedSyntaxToken(NativeRsmlToken token) => new((TokenKind)token.kind, token.startIndex, token.endIndex);
		internal static SyntaxLine FromNativeToManagedSyntaxLine(NativeRsmlLine* line) => new(
				FromNativeToManagedSyntaxToken(line->item1),
				FromNativeToManagedSyntaxToken(line->item2),
				FromNativeToManagedSyntaxToken(line->item3),
				FromNativeToManagedSyntaxToken(line->item4),
				FromNativeToManagedSyntaxToken(line->item5),
				FromNativeToManagedSyntaxToken(line->item6),
				FromNativeToManagedSyntaxToken(line->item7),
				FromNativeToManagedSyntaxToken(line->item8)
			);

		#endregion

		/// <summary>
		/// Allocates a buffer to be public to all RSML toolchain tools.
		/// </summary>
		/// <param name="input">The buffer's contents</param>
		/// <returns>
		/// <list type="bullet"><c>-3:</c> Unknown error<br /></list>
		/// <list type="bullet"><c>-2:</c> The input line is empty<br /></list>
		/// <list type="bullet"><c>-1:</c> The given pointer is null (<c>IntPtr.Zero</c>)<br /></list>
		/// <list type="bullet"><c>0:</c> Success<br /></list>
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) }, EntryPoint = "rsml_alloc_buffer")]
		public static int AllocRsmlBuffer(nint input)
		{

			try
			{

				System.Diagnostics.Debug.Assert(sizeof(NativeRsmlToken) == 12);
				System.Diagnostics.Debug.Assert(sizeof(NativeRsmlLine) == 96);

				if (input == IntPtr.Zero)
					return -1;

				string? data = Marshal.PtrToStringAuto(input);

				if (String.IsNullOrEmpty(data))
					return -2;

				buffer = new(data);

				return 0;

			}
			catch { return -3; }

		}

		/// <summary>
		/// Returns the last saved error message. Can be a null pointer (<c>IntPtr.Zero</c>).
		/// </summary>
		/// <returns>The pointer to the error message</returns>
		[UnmanagedCallersOnly(EntryPoint = "rsml_get_last_error_message")]
		public static nint GetLastErrorMessage() => lastErrorMessage;

		/// <summary>
		/// Returns the last saved evaluation result. Can be a null pointer (<c>IntPtr.Zero</c>).
		/// </summary>
		/// <returns>The pointer to the evaluation result</returns>
		[UnmanagedCallersOnly(EntryPoint = "rsml_get_last_evaluation_result")]
		public static nint GetLastEvaluationResult() => evaluationResult;		

		/// <summary>
		/// Tokenizes a line of RSML.
		/// </summary>
		/// <param name="outputLine">The tokenized line</param>
		/// <returns>
		/// <list type="bullet"><c>-5:</c> There's no allocated buffer<br /></list>
		/// <list type="bullet"><c>-4:</c> Output token count exceeds 8<br /></list>
		/// <list type="bullet"><c>-3:</c> An error occured while tokenizing the line<br /></list>
		/// <list type="bullet"><c>-2:</c> The line is empty<br /></list>
		/// <list type="bullet"><c>-1:</c> The given pointer is null (<c>IntPtr.Zero</c>)<br /></list>
		/// <list type="bullet"><c>0:</c> Success<br /></list>
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) }, EntryPoint = "rsml_tokenize_line")]
		public static int TokenizeRsmlLine(nint outputLine)
		{

			try
			{

				System.Diagnostics.Debug.Assert(sizeof(NativeRsmlToken) == 12);
				System.Diagnostics.Debug.Assert(sizeof(NativeRsmlLine) == 96);

				if (buffer is null || buffer.IsEmpty)
					return -5;

				if (outputLine == IntPtr.Zero)
					return -1;

				var line = Lexer.TokenizeLine(buffer);

				if (line.Length > 8)
					return -4;

				NativeRsmlLine* dst = (NativeRsmlLine*)outputLine.ToPointer();

				dst->item1 = FromManagedToNativeSyntaxToken(line.Item1);
				dst->item2 = FromManagedToNativeSyntaxToken(line.Item2);
				dst->item3 = FromManagedToNativeSyntaxToken(line.Item3);
				dst->item4 = FromManagedToNativeSyntaxToken(line.Item4);
				dst->item5 = FromManagedToNativeSyntaxToken(line.Item5);
				dst->item6 = FromManagedToNativeSyntaxToken(line.Item6);
				dst->item7 = FromManagedToNativeSyntaxToken(line.Item7);
				dst->item8 = FromManagedToNativeSyntaxToken(line.Item8);

				return 0;

			}
			catch { return -3; }

		}

		/// <summary>
		/// Normalizes a line of RSML.
		/// </summary>
		/// <param name="inputLine">The line to normalize</param>
		/// <param name="outputLine">The normalized line</param>
		/// <returns>
		/// <list type="bullet"><c>-4:</c> Output token count exceeds 8<br /></list>
		/// <list type="bullet"><c>-3:</c> An error occured while normalizing the input line<br /></list>
		/// <list type="bullet"><c>-2:</c> The input line is empty<br /></list>
		/// <list type="bullet"><c>-1:</c> At least one of the given pointers is null (<c>IntPtr.Zero</c>)<br /></list>
		/// <list type="bullet"><c>0:</c> Success<br /></list>
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) }, EntryPoint = "rsml_normalize_line")]
		public static int NormalizeRsmlLine(nint inputLine, nint outputLine)
		{

			try
			{

				System.Diagnostics.Debug.Assert(sizeof(NativeRsmlToken) == 12);
				System.Diagnostics.Debug.Assert(sizeof(NativeRsmlLine) == 96);

				if (inputLine == IntPtr.Zero || outputLine == IntPtr.Zero)
					return -1;

				NativeRsmlLine* src = (NativeRsmlLine*)inputLine.ToPointer();
				var line = FromNativeToManagedSyntaxLine(src);

				if (line.IsEmpty)
					return -2;

				Normalizer.NormalizeLine(ref line, out int tokenCount);

				if (tokenCount > 8)
					return -4;

				NativeRsmlLine* dst = (NativeRsmlLine*)outputLine.ToPointer();
				*dst = *src;

				dst->item1 = FromManagedToNativeSyntaxToken(line.Item1);
				dst->item2 = FromManagedToNativeSyntaxToken(line.Item2);
				dst->item3 = FromManagedToNativeSyntaxToken(line.Item3);
				dst->item4 = FromManagedToNativeSyntaxToken(line.Item4);
				dst->item5 = FromManagedToNativeSyntaxToken(line.Item5);
				dst->item6 = FromManagedToNativeSyntaxToken(line.Item6);
				dst->item7 = FromManagedToNativeSyntaxToken(line.Item7);
				dst->item8 = FromManagedToNativeSyntaxToken(line.Item8);

				return 0;

			}
			catch { return -3; }

		}

		/// <summary>
		/// Validates a line of RSML.
		/// </summary>
		/// <param name="inputLine">The line to validate</param>
		/// <returns>
		/// <list type="bullet"><c>-4:</c> There's no allocated buffer or allocated buffer is empty<br /></list>
		/// <list type="bullet"><c>-3:</c> Unknown error<br /></list>
		/// <list type="bullet"><c>-2:</c> The input line is empty<br /></list>
		/// <list type="bullet"><c>-1:</c> The given pointer is null (<c>IntPtr.Zero</c>)<br /></list>
		/// <list type="bullet"><c>0:</c> Success and line is valid<br /></list>
		/// <list type="bullet"><c>1:</c> Success but line is invalid (error message is saved to internal data)<br /></list>
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) }, EntryPoint = "rsml_validate_line")]
		public static nint ValidateRsmlLine(nint inputLine)
		{

			try
			{

				System.Diagnostics.Debug.Assert(sizeof(NativeRsmlToken) == 12);
				System.Diagnostics.Debug.Assert(sizeof(NativeRsmlLine) == 96);

				if (buffer is null || buffer.IsEmpty)
					return -4;

				if (inputLine == IntPtr.Zero)
					return -1;

				NativeRsmlLine* src = (NativeRsmlLine*)inputLine.ToPointer();
				var line = FromNativeToManagedSyntaxLine(src);

				if (line.IsEmpty)
					return -2;

				Validator.ValidateLine(line, buffer);


				return 0;

			}
			catch (InvalidRsmlSyntax ex)
			{

				try
				{

					if (lastErrorMessage != IntPtr.Zero)
						Marshal.FreeHGlobal(lastErrorMessage);

					lastErrorMessage = Marshal.StringToHGlobalAuto(ex.Message);

				}
				catch { return -3; }

				return 1;

			}
			catch { return -3; }

		}

		/// <summary>
		/// Evaluates a RSML document given a machine.
		/// </summary>
		/// <param name="systemName">The machine's system name - use a nullptr to leave it undefined</param>
		/// <param name="distroName">The machine's Linux distro name - use only if Linux; nullptr for undefined</param>
		/// <param name="distroFamily">The machine's Linux distro family - use only if Linux; nullptr for undefined</param>
		/// <param name="systemVersion">The machine's system version - use <c>-1</c> for undefined</param>
		/// <param name="processorArchitecture">The machine's processor architecture - nullptr if undefined</param>
		/// <returns>
		/// <c>-4:</c> Input buffer is empty or unassigned
		/// <c>-3:</c> An unknown error occured
		/// <c>-2:</c> An error occured while assigning the result (evaluation might or might have not found matches)
		/// <c>-1:</c> An error occured while evaluating the document OR the result buffer is too small to hold the result<br />
		/// <c>0:</c> A match was found (result assigned)<br />
		/// <c>1:</c> No matches were found (result unassigned)<br />
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) }, EntryPoint = "rsml_evaluate_document")]
		public static int EvaluateRsmlDocument(nint resultBuffer, int systemOrDistroName, int distroFamilyNameOrNull, int systemOrDistroMajorVersion, int processorArchitecture)
		{

			#region Converting values to LocalMachine-compatible

			string? actualSystemName = systemOrDistroName switch
			{

				0 => null,
				1 => "windows",
				2 => "osx",
				3 => "freebsd",
				201 => "debian",
				202 => "fedora",
				203 => "ubuntu",
				204 => "archlinux",
				_ => "UNKNOWN"

			};

			string? actualDistroFamily = (systemOrDistroName is >= 201 and <= 204) ? distroFamilyNameOrNull switch
			{

				0 => null,
				201 => "debian",
				202 => "fedora",
				203 => "ubuntu",
				204 => "archlinux",
				_ => "UNKNOWN"

			} : null;

			string? actualProcessorArchitecture = processorArchitecture switch
			{

				0 => null,
				1 => "arm32", // also known as simply "ARM"
				2 => "arm64",
				3 => "x64",
				4 => "x86",
				5 => "loongarch64",
				_ => "UNKNOWN"

			};

			LocalMachine actualMachine = LocalMachine.Merge(LocalMachine.CurrentMachine, (systemOrDistroName is >= 201 and <= 204) ? LocalMachine.Linux(actualSystemName, actualDistroFamily, actualProcessorArchitecture, systemOrDistroMajorVersion) : new(actualSystemName, actualProcessorArchitecture, systemOrDistroMajorVersion));

			#endregion


			// fixme: fix this method
			// todo: ensure each error has its own return code instead of a "-1 catch all"

			// xxx: check if buffer is allocated
			// xxx: check if the resultBuffer is not IntPtr.Zero
			Evaluator evaluator = new(span);
			EvaluationResult result;

			try
			{
				result = evaluator.Evaluate()
			}
			catch (InvalidRsmlSyntax ex)
			{

				try
				{

					if (lastErrorMessage != IntPtr.Zero)
						Marshal.FreeHGlobal(lastErrorMessage);

			byte[] asBytes = Encoding.UTF8.GetBytes(result.MatchValue!);

				}
				catch { return -3; }

				return 1;

			}
			catch { return -3; }

		}

		// todo: add the Lexer "tokens-to-RSML" method here

	}

}
