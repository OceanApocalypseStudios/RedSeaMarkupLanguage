namespace OceanApocalypseStudios.RSML.Diagnostics.ErrorCodes;

internal static class InternalErrorCodes
{
	public static readonly ErrorCode EmptyBuffer = new(ErrorCategory.Internal, 1);
	public static readonly ErrorCode IndexOutOfRange = new(ErrorCategory.Internal, 2);
	public static readonly ErrorCode LineNumberOutOfRange = new(ErrorCategory.Internal, 3);
	public static readonly ErrorCode NullCheckFailed = new(ErrorCategory.Internal, 4);
	public static readonly ErrorCode UnhandledException = new(ErrorCategory.Internal, 5);
}
