using System;


namespace OceanApocalypseStudios.RSML.Sources
{

	/// <summary>
	/// A data source for RSML's toolchain members.
	/// </summary>
	public interface ISource : IDisposable
	{

		/// <summary>
		/// Converts an index into a location.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="span">The output <see cref="SourceLocation"/>.</param>
		/// <remarks>
		/// In most implementations, this method is extremely expensive performance-wise as it requires
		/// calculating all newlines up until <paramref name="index"/>.
		/// </remarks>
		/// <returns>True if the conversion was successful.</returns>
		bool TryGetSourceLocation(int index, out SourceLocation span);

	}

}
