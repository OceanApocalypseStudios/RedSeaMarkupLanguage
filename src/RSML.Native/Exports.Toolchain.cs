using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using OceanApocalypseStudios.RSML.Analyzer.Semantics;
using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Evaluation;
using OceanApocalypseStudios.RSML.Exceptions;
using OceanApocalypseStudios.RSML.Host;
using OceanApocalypseStudios.RSML.Native.Structures;


namespace OceanApocalypseStudios.RSML.Native
{

	// toolchain exports
	public static unsafe partial class Exports
	{

		/// <summary>
		/// Evaluates a RSML document given a host.
		/// </summary>
		/// <param name="outputResultPtr">A pointer to the <see cref="NativeEvaluationResult" /> instance this method will write to</param>
		/// <param name="systemOrDistroName">
		/// The host's system name (or distro name if Linux) - use a nullptr to leave it
		/// undefined
		/// </param>
		/// <param name="distroFamily">The host's Linux distro family - use only if Linux; nullptr for undefined or not Linux</param>
		/// <param name="systemOrDistroMajorVersion">
		/// The host's system version (or distro version if Linux) - use <c>-1</c> for
		/// undefined
		/// </param>
		/// <param name="processorArchitecture">The host's processor architecture - nullptr if undefined</param>
		/// <param name="currentHostFallback">Whether the current host should be used as fallback</param>
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
			int processorArchitecture,
			byte currentHostFallback
		)
		{
			try
			{

				#region Converting given values to Host-compatible values

				string? actualSystemName = systemOrDistroName switch
				{

					0   => null,
					1   => "windows",
					2   => "osx",
					3   => "linux",
					4   => "freebsd",
					101 => "debian",
					102 => "fedora",
					103 => "ubuntu",
					104 => "archlinux",
					_   => "UNKNOWN"

				};

				string? actualDistroFamily = systemOrDistroName is (>= 101 and <= 104) or 3
												 ? distroFamily switch
												 {

													 0   => null,
													 1 => "debian",
													 2 => "fedora",
													 3 => "ubuntu",
													 4 => "archlinux",
													 _   => "UNKNOWN"

												 }
												 : null;

				string? actualProcessorArchitecture = processorArchitecture switch
				{

					0 => null,
					1 => "arm32", // arm
					2 => "arm64",
					3 => "x64",
					4 => "x86",
					5 => "loongarch64",
					_ => "UNKNOWN"

				};

				HostInfo customHost =
					systemOrDistroName is (>= 101 and <= 104) or 3
						? new(
							actualSystemName,
							actualDistroFamily,
							actualProcessorArchitecture,
							systemOrDistroMajorVersion
						)
						: new(
							actualSystemName,
							actualProcessorArchitecture,
							systemOrDistroMajorVersion
						);

				HostInfo actualHost =
					currentHostFallback == 1
						? HostInfo.MergeWithFallback(customHost, HostInfo.CurrentHost)
						: customHost;

				#endregion

				if (buffer?.IsEmpty is true or null)
					return -7;

				if (outputResultPtr == IntPtr.Zero)
					return -8;

				EvaluationResult result = null!;

				var content = buffer.ReadUntil((_, _) => false); // this reads everything

				Evaluator evaluator = new(content);

				try
				{
					result = evaluator.Evaluate(actualHost);
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


				*(NativeEvaluationResult*)outputResultPtr.ToPointer() = new(result, buffer);

				return result.WasMatchFound ? 0 : 1;

			}
			catch
			{
				return -10;
			}

		}

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

				var line = (*(NativeLine*)inputLinePtr.ToPointer()).ToLine();

				if (line.IsEmpty)
					return -2;

				Normalizer.NormalizeLine(ref line, out int tokenCount);

				if (tokenCount > 8)
					return -4;

				var dst = *(NativeLine*)outputLinePtr.ToPointer() = (NativeLine)line;

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

				*(NativeLine*)outputLinePtr.ToPointer() = (NativeLine)line;

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

				var line = (*(NativeLine*)inputLinePtr.ToPointer()).ToLine();

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

	}

}
