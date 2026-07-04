using System;

using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Exceptions
{

	/// <summary>
	/// An error that has happened in the RSML toolchain and comes from a source.
	/// </summary>
	/// <param name="span">The span the error relates to.</param>
	/// <param name="message">A brief error message detailing why it has happened.</param>
	/// <param name="severity">The error's severity.</param>
	public struct SourceError(SourceSpan span, string message, Severity severity) : IEquatable<SourceError>
	{

		/// <summary>
		/// The span the error relates to.
		/// </summary>
		public readonly SourceSpan Span => span;

		/// <summary>
		/// A brief error message detailing why it has happened.
		/// </summary>
		public readonly string Message => message;

		/// <summary>
		/// The error's severity.
		/// </summary>
		public readonly Severity Severity => severity;

		/// <inheritdoc/>
		public override readonly bool Equals(object? obj) => obj is SourceError error && Equals(error);
		
		/// <inheritdoc/>
		public readonly bool Equals(SourceError other) => Message == other.Message && Severity == other.Severity && Span.Equals(other.Span);

		/// <summary>
		/// Checks if two <see cref="SourceError"/>s are equal to each other.
		/// </summary>
		/// <returns>True if equals.</returns>
		public static bool operator ==(SourceError left, SourceError right) => left.Equals(right);

		/// <summary>
		/// Checks if two <see cref="SourceError"/>s are different from each other.
		/// </summary>
		/// <returns>True if different.</returns>
		public static bool operator !=(SourceError left, SourceError right) => !left.Equals(right);

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{

			unchecked
			{

				int hashCode = InternalUtils.HashCodeSeed * InternalUtils.HashCodeMultiplier + Span.GetHashCode();
				hashCode = hashCode * InternalUtils.HashCodeMultiplier + Message.GetHashCode();
				return hashCode * InternalUtils.HashCodeMultiplier + Severity.GetHashCode();

			}

		}

	}

}
