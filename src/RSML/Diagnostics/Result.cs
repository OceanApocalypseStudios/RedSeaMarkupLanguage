namespace OceanApocalypseStudios.RSML.Diagnostics;

/// <summary>
/// An operation's result.
/// </summary>
/// <typeparam name="TValue">The type of the return value when successful.</typeparam>
public readonly struct Result<TValue>
{
	/// <summary>
	/// The return value. Might be null or an arbitrary default if
	/// <see cref="IsSuccessful"/> is <c>false</c>.
	/// </summary>
#if NET8_0_OR_GREATER
	[MemberNotNullWhen(true, nameof(Value))]
#endif
	public readonly TValue? Value { get; }

	/// <summary>
	/// The diagnostic that serves as the error value when an error occurs.
	/// Might be null or an arbitrary default if <see cref="IsError"/> is <c>true</c>.
	/// </summary>
	public readonly Diagnostic Error { get; }

	/// <summary>
	/// Set to <c>true</c> when the operation is successful. Otherwise, it's set to <c>false</c>.
	/// It also indicates whether <see cref="Value"/> can be safely accessed or not (safe to access if
	/// <see cref="IsSuccessful"/> is <c>true</c>).
	/// </summary>
	/// <remarks>
	/// <see cref="IsSuccessful"/> and <see cref="IsError"/> are mutually exclusive conditions. When one is true,
	/// the other is false. This means you can use any of the two to evaluate whether the operation was successful
	/// and if it's safe to access <see cref="Value"/>.
	/// </remarks>
	public readonly bool IsSuccessful { get; }

	/// <summary>
	/// Set to <c>false</c> when the operation is successful. Otherwise, it's set to <c>true</c>.
	/// It also indicates whether <see cref="Value"/> can be safely accessed or not (safe to access if
	/// <see cref="IsError"/> is <c>false</c>).
	/// </summary>
	/// <remarks>
	/// <see cref="IsSuccessful"/> and <see cref="IsError"/> are mutually exclusive conditions. When one is true,
	/// the other is false. This means you can use any of the two to evaluate whether the operation was successful
	/// and if it's safe to access <see cref="Value"/>.
	/// </remarks>
	public readonly bool IsError => !IsSuccessful;

	private Result(TValue value)
	{
		Value = value;
		Error = default;
		IsSuccessful = true;
	}

	private Result(Diagnostic error)
	{
		Value = default;
		Error = error;
		IsSuccessful = false;
	}

	/// <summary>
	/// Creates a return result with a successful outcome.
	/// </summary>
	/// <param name="value">The return value.</param>
	/// <returns>The result.</returns>
	public static Result<TValue> Success(TValue value) => new(value);

	/// <summary>
	/// Creates a return result with an unsuccessful outcome.
	/// </summary>
	/// <param name="error">The error.</param>
	/// <returns>The result.</returns>
	public static Result<TValue> Fail(Diagnostic error) => new(error);
}
