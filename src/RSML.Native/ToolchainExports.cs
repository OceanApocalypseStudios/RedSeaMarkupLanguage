using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

		internal static DualTextBuffer? buffer = null;
		internal static nint lastErrorMessage = IntPtr.Zero;

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
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) }, EntryPoint = "rsml_get_last_error_message")]
		public static nint GetLastErrorMessage() => lastErrorMessage;	

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
		/// <param name="output">The result details</param>
		/// <param name="systemOrDistroName">The machine's system name (or distro name if Linux) - use a nullptr to leave it undefined</param>
		/// <param name="distroFamilyNameOrNull">The machine's Linux distro family - use only if Linux; nullptr for undefined or not Linux</param>
		/// <param name="systemOrDistroMajorVersion">The machine's system version (or distro version if Linux) - use <c>-1</c> for undefined</param>
		/// <param name="processorArchitecture">The machine's processor architecture - nullptr if undefined</param>
		/// <returns>
		/// <list type="bullet"><c>-10:</c> Unknown error during the whole process</list>
		/// <list type="bullet"><c>-9:</c> Unknown error during evaluation</list>
		/// <list type="bullet"><c>-8:</c> No pointer was given for the result details</list>
		/// <list type="bullet"><c>-7:</c> There's no allocated buffer or allocated buffer is empty</list>
		/// <list type="bullet"><c>-6:</c> User Raised Exception (failed to register error message)</list>
		/// <list type="bullet"><c>-5:</c> User Raised Exception</list>
		/// <list type="bullet"><c>-4:</c> Action Error (failed to register error message)</list>
		/// <list type="bullet"><c>-3:</c> Action Error</list>
		/// <list type="bullet"><c>-2:</c> Invalid RSML Syntax (failed to register error message)</list>
		/// <list type="bullet"><c>-1:</c> Invalid RSML Syntax</list>
		/// <list type="bullet"><c>0:</c> A match was found</list>
		/// <list type="bullet"><c>1:</c> No matches were found</list>
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) }, EntryPoint = "rsml_evaluate_document")]
		public static int EvaluateRsmlDocument(nint output, int systemOrDistroName, int distroFamilyNameOrNull, int systemOrDistroMajorVersion, int processorArchitecture)
		{
			try
			{

				#region Converting values to LocalMachine-compatible

				string? actualSystemName = systemOrDistroName switch
				{

					0 => null,
					1 => "windows",
					2 => "osx",
					3 => "freebsd",
					4 => "linux",
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
					1 => "arm32",
					2 => "arm64",
					3 => "x64",
					4 => "x86",
					5 => "loongarch64",
					_ => "UNKNOWN"

				};

				LocalMachine actualMachine = LocalMachine.MergeWithFallback(LocalMachine.CurrentMachine, (systemOrDistroName is >= 201 and <= 204) ? LocalMachine.Linux(actualSystemName, actualDistroFamily, actualProcessorArchitecture, systemOrDistroMajorVersion) : new(actualSystemName, actualProcessorArchitecture, systemOrDistroMajorVersion));

				#endregion

				// fixme: fix this method

				if (buffer is null || buffer.IsEmpty)
					return -7;

				if (output == IntPtr.Zero)
					return -8;

				EvaluationResult result = null!;
				ReadOnlyMemory<char> content = buffer.ReadUntil((_, _) => false); // this reads everything
				Evaluator evaluator = new(content);

				try
				{
					result = evaluator.Evaluate(actualMachine);
				}
				catch (InvalidRsmlSyntax irs) // yeah it's the fucking IRS
				{

					try
					{

						if (lastErrorMessage != IntPtr.Zero)
							Marshal.FreeHGlobal(lastErrorMessage);

						lastErrorMessage = Marshal.StringToHGlobalAuto(irs.Message);

					}
					catch { return -2; }

					return -1;

				}
				catch (ActionErrorException aee)
				{

					try
					{

						if (lastErrorMessage != IntPtr.Zero)
							Marshal.FreeHGlobal(lastErrorMessage);

						lastErrorMessage = Marshal.StringToHGlobalAuto(aee.Message);

					}
					catch { return -4; }

					return -3;

				}
				catch (UserRaisedException ure)
				{

					try
					{

						if (lastErrorMessage != IntPtr.Zero)
							Marshal.FreeHGlobal(lastErrorMessage);

						lastErrorMessage = Marshal.StringToHGlobalAuto(ure.Message);

					}
					catch { return -6; }

					return -5;

				}
				catch { return -9; }

				NativeEvaluationResult* dst = (NativeEvaluationResult*)output.ToPointer();

				int startIndex = result.WasMatchFound ? content.Span.IndexOf(result.MatchValue!) : -1;

				dst->wasMatchFound = (byte)(result.WasMatchFound ? 1 : 0);
				dst->matchValueStart = startIndex;
				dst->matchValueEnd = result.WasMatchFound ? (startIndex + result.MatchValue!.Length) : -1;

				return result.WasMatchFound ? 0 : 1;

			}
			catch { return -10; }

		}

	}

}
