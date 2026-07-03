#if NET10_0_OR_GREATER

using System;


namespace OceanApocalypseStudios.RSML.Sources
{

	/// <summary>
	/// A data source that supports async mechanics for RSML's toolchain members.
	/// </summary>
	public interface IAsyncSource : IAsyncDisposable;

}

#endif
