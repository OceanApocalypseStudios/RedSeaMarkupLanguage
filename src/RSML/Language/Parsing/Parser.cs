using System;

namespace OceanApocalypseStudios.RSML.Language.Parsing;

/// <summary>
/// The base type that deals with parsing tokens and turning them into an organized tree.
/// </summary>
public abstract class Parser : IParser
{
	private bool isDisposed;

	/// <inheritdoc/>
	public ToolchainConfiguration Configuration { get; protected set; }

	/// <inheritdoc/>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <inheritdoc/>
	public virtual void Inject(ToolchainConfiguration configuration) => throw new NotImplementedException();

	/// <summary>
	/// Disposes of both managed and unmanaged resources.
	/// </summary>
	/// <param name="disposing">When set to <c>false</c>, disposes of unmanaged resources only.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (isDisposed)
			return;

		if (disposing)
		{ }

		isDisposed = true;
	}
}
