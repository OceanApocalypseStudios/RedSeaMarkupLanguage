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

		/// <summary>
		/// Allocates a buffer to be public to all RSML toolchain tools.
		/// </summary>
		/// <param name="content">The buffer's contents</param>
		/// <param name="byteCount">The amount of bytes the content has</param>
		/// <returns>
		/// <list type="bullet"><c>-3:</c> Unknown error<br /></list>
		/// <list type="bullet"><c>-2:</c> The given amount of bytes is less than 0<br /></list>
		/// <list type="bullet"><c>-1:</c> The given pointer is null or the input buffer is null<br /></list>
		/// <list type="bullet"><c>0:</c> Success<br /></list>
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ], EntryPoint = "rsml_alloc_buffer")]
		public static int AllocRsmlBuffer(
			byte* content,
			int byteCount
		)
		{

			try
			{

				string data = Encoding.Default.GetString(content, byteCount);

				if (data == "")
					throw new ArgumentNullException(null, "String is empty");

				buffer = new(data);

				return 0;

			}
			catch (ArgumentNullException ane)
			{

				if (lastErrorMessage != IntPtr.Zero)
					Marshal.FreeHGlobal(lastErrorMessage);

				lastErrorMessage = Marshal.StringToHGlobalAuto(ane.Message);

				return -1;

			}
			catch (ArgumentOutOfRangeException aoo)
			{

				if (lastErrorMessage != IntPtr.Zero)
					Marshal.FreeHGlobal(lastErrorMessage);

				lastErrorMessage = Marshal.StringToHGlobalAuto(aoo.Message);

				return -2;

			}
			catch (Exception ex)
			{

				if (lastErrorMessage != IntPtr.Zero)
					Marshal.FreeHGlobal(lastErrorMessage);

				lastErrorMessage = Marshal.StringToHGlobalAuto(ex.Message);

				return -3;

			}

		}

		/// <summary>
		/// Destroys memory that is no longer necessary but still in use by RSML.
		/// </summary>
		/// <returns><c>0</c> if successful; <c>-1</c> if not successful</returns>
		[UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ], EntryPoint = "rsml_cleanup")]
		public static int Cleanup()
		{

			try
			{

				if (lastErrorMessage != IntPtr.Zero)
				{

					Marshal.FreeHGlobal(lastErrorMessage);
					lastErrorMessage = IntPtr.Zero;

				}

				buffer = null;

				return 0;

			}
			catch (Exception)
			{
				return -1;
			}

		}

		/// <summary>
		/// Evaluates a RSML document given a machine.
		/// </summary>
		/// <param name="outputResultPtr">A pointer to the <see cref="NativeEvaluationResult" /> instance this method will write to</param>
		/// <param name="systemOrDistroName">
		/// The machine's system name (or distro name if Linux) - use a nullptr to leave it
		/// undefined
		/// </param>
		/// <param name="distroFamily">The machine's Linux distro family - use only if Linux; nullptr for undefined or not Linux</param>
		/// <param name="systemOrDistroMajorVersion">
		/// The machine's system version (or distro version if Linux) - use <c>-1</c> for
		/// undefined
		/// </param>
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
		[UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ], EntryPoint = "rsml_evaluate_document")]
		public static int EvaluateRsmlDocument(
			nint outputResultPtr,
			int systemOrDistroName,
			int distroFamily,
			int systemOrDistroMajorVersion,
			int processorArchitecture
		)
		{
			try
			{

				#region Converting values to LocalMachine-compatible

				string? actualSystemName = systemOrDistroName switch
				{

					0   => null,
					1   => "windows",
					2   => "osx",
					3   => "freebsd",
					4   => "linux",
					201 => "debian",
					202 => "fedora",
					203 => "ubuntu",
					204 => "archlinux",
					_   => "UNKNOWN"

				};

				string? actualDistroFamily = systemOrDistroName is >= 201 and <= 204
												 ? distroFamily switch
												 {

													 0   => null,
													 201 => "debian",
													 202 => "fedora",
													 203 => "ubuntu",
													 204 => "archlinux",
													 _   => "UNKNOWN"

												 }
												 : null;

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

				var actualMachine = LocalMachine.MergeWithFallback(
					LocalMachine.CurrentMachine, systemOrDistroName is >= 201 and <= 204
													 ? LocalMachine.Linux(
														 actualSystemName, actualDistroFamily, actualProcessorArchitecture, systemOrDistroMajorVersion
													 )
													 : new(actualSystemName, actualProcessorArchitecture, systemOrDistroMajorVersion)
				);

				#endregion

				if (buffer?.IsEmpty is true or null)
					return -7;

				if (outputResultPtr == IntPtr.Zero)
					return -8;

				EvaluationResult result = null!;

				var content = buffer.ReadUntil((
												   _,
												   _
											   ) => false
				); // this reads everything

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
					catch
					{
						return -2;
					}

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
					catch
					{
						return -4;
					}

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
					catch
					{
						return -6;
					}

					return -5;

				}
				catch
				{
					return -9;
				}

				var dst = (NativeEvaluationResult*)outputResultPtr.ToPointer();

				int startIndex = result.WasMatchFound
									 ? content.Span.IndexOf(result.MatchValue!)
									 : -1;

				dst->wasMatchFound = (byte)(result.WasMatchFound
												? 1
												: 0);

				dst->matchValueStart = startIndex;

				dst->matchValueEnd = result.WasMatchFound
										 ? startIndex + result.MatchValue!.Length
										 : -1;

				return result.WasMatchFound
						   ? 0
						   : 1;

			}
			catch
			{
				return -10;
			}

		}

		/// <summary>
		/// Returns the last saved error message. Can be a null pointer (<c>IntPtr.Zero</c>).
		/// </summary>
		/// <returns>The pointer to the error message</returns>
		[UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ], EntryPoint = "rsml_get_last_error_message")]
		public static nint GetLastErrorMessage() => lastErrorMessage;

		/// <summary>
		/// Normalizes a line of RSML.
		/// </summary>
		/// <param name="inputLinePtr">
		/// A pointer to the <see cref="NativeLine" /> instance this method will normalize (without writing to input)
		/// </param>
		/// <param name="outputLinePtr">A pointer to the <see cref="NativeLine" /> instance this method will write to</param>
		/// <returns>
		/// <list type="bullet">
		/// <c>-5:</c> An error occured while normalizing the input line (failed to save error message)<br />
		/// </list>
		/// <list type="bullet"><c>-4:</c> Output token count exceeds 8<br /></list>
		/// <list type="bullet"><c>-3:</c> An error occured while normalizing the input line (error message was saved)<br /></list>
		/// <list type="bullet"><c>-2:</c> The input line is empty<br /></list>
		/// <list type="bullet"><c>-1:</c> At least one of the given pointers is null (<c>IntPtr.Zero</c>)<br /></list>
		/// <list type="bullet"><c>0:</c> Success<br /></list>
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ], EntryPoint = "rsml_normalize_line")]
		public static int NormalizeRsmlLine(
			nint inputLinePtr,
			nint outputLinePtr
		)
		{

			try
			{

				if (inputLinePtr == IntPtr.Zero || outputLinePtr == IntPtr.Zero)
					return -1;

				var src = (NativeLine*)inputLinePtr.ToPointer();
				var line = SyntaxExtensions.PtrToLine(src);

				if (line.IsEmpty)
					return -2;

				Normalizer.NormalizeLine(ref line, out int tokenCount);

				if (tokenCount > 8)
					return -4;

				var dst = (NativeLine*)outputLinePtr.ToPointer();
				line.CopyToNative(dst);

				return 0;

			}
			catch (Exception ex)
			{

				try
				{

					if (lastErrorMessage != IntPtr.Zero)
						Marshal.FreeHGlobal(lastErrorMessage);

					lastErrorMessage = Marshal.StringToHGlobalAuto(ex.Message);

				}
				catch
				{
					return -5;
				}

				return -3;

			}

		}

		/// <summary>
		/// Tokenizes a line of RSML.
		/// </summary>
		/// <param name="outputLinePtr">A pointer to the <see cref="NativeLine" /> instance this method will write to</param>
		/// <returns>
		/// <list type="bullet"><c>-6:</c> Output token count exceeds 8 (failed to save error message)<br /></list>
		/// <list type="bullet"><c>-5:</c> Output token count exceeds 8 (error message was saved)<br /></list>
		/// <list type="bullet"><c>-4:</c> An error occured while tokenizing the line (failed to save error message)<br /></list>
		/// <list type="bullet"><c>-3:</c> An error occured while tokenizing the line (error message was saved)<br /></list>
		/// <list type="bullet"><c>-2:</c> There's no allocated buffer or it is empty<br /></list>
		/// <list type="bullet"><c>-1:</c> The given pointer is null (<c>IntPtr.Zero</c>)<br /></list>
		/// <list type="bullet"><c>0:</c> Success<br /></list>
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ], EntryPoint = "rsml_tokenize_line")]
		public static int TokenizeRsmlLine(nint outputLinePtr)
		{

			try
			{

				if (buffer?.IsEmpty is true or null)
					return -2;

				if (outputLinePtr == IntPtr.Zero)
					return -1;

				var line = Lexer.TokenizeLine(buffer);

				var dst = (NativeLine*)outputLinePtr.ToPointer();
				line.CopyToNative(dst);

				return 0;

			}
			catch (ArgumentOutOfRangeException aoo)
			{

				try
				{

					if (lastErrorMessage != IntPtr.Zero)
						Marshal.FreeHGlobal(lastErrorMessage);

					lastErrorMessage = Marshal.StringToHGlobalAuto(aoo.Message);

				}
				catch
				{
					return -6;
				}

				return -5;

			}
			catch (Exception ex)
			{

				try
				{

					if (lastErrorMessage != IntPtr.Zero)
						Marshal.FreeHGlobal(lastErrorMessage);

					lastErrorMessage = Marshal.StringToHGlobalAuto(ex.Message);

				}
				catch
				{
					return -4;
				}

				return -3;

			}

		}

		/// <summary>
		/// Validates a line of RSML.
		/// </summary>
		/// <param name="inputLinePtr">A pointer to the <see cref="NativeLine" /> instance this method will validate</param>
		/// <returns>
		/// <list type="bullet"><c>-6:</c> Unknown error (failed to save error message)<br /></list>
		/// <list type="bullet"><c>-5:</c> Unknown error (error message was saved)<br /></list>
		/// <list type="bullet"><c>-4:</c> Success but line is invalid (failed to save error message)<br /></list>
		/// <list type="bullet"><c>-3:</c> There's no allocated buffer or allocated buffer is empty<br /></list>
		/// <list type="bullet"><c>-2:</c> The input line is empty<br /></list>
		/// <list type="bullet"><c>-1:</c> The given pointer is null (<c>IntPtr.Zero</c>)<br /></list>
		/// <list type="bullet"><c>0:</c> Success and line is valid<br /></list>
		/// <list type="bullet"><c>1:</c> Success but line is invalid (error message was saved)<br /></list>
		/// </returns>
		[UnmanagedCallersOnly(CallConvs = [ typeof(CallConvCdecl) ], EntryPoint = "rsml_validate_line")]
		public static int ValidateRsmlLine(nint inputLinePtr)
		{

			try
			{

				if (buffer?.IsEmpty is true or null)
					return -3;

				if (inputLinePtr == IntPtr.Zero)
					return -1;

				var src = (NativeLine*)inputLinePtr.ToPointer();
				var line = SyntaxExtensions.PtrToLine(src);

				if (line.IsEmpty)
					return -2;

				Validator.ValidateLine(line, buffer);


				return 0;

			}
			catch (InvalidRsmlSyntax irs) // the fucking IRS again
			{

				try
				{

					if (lastErrorMessage != IntPtr.Zero)
						Marshal.FreeHGlobal(lastErrorMessage);

					lastErrorMessage = Marshal.StringToHGlobalAuto(irs.Message);

				}
				catch
				{
					return -4;
				}

				return 1;

			}
			catch (Exception ex)
			{

				try
				{

					if (lastErrorMessage != IntPtr.Zero)
						Marshal.FreeHGlobal(lastErrorMessage);

					lastErrorMessage = Marshal.StringToHGlobalAuto(ex.Message);

				}
				catch
				{
					return -6;
				}

				return -5;

			}

		}

		internal static DualTextBuffer? buffer;

		internal static nint lastErrorMessage = IntPtr.Zero;

	}

}
