namespace PlatipusWallet.Api.Results.Common.Result.Factories;

using WithData;

public static class DatabetResultFactory
{
    public static DatabetResult Success() => new();

    public static DatabetResult<TData> Success<TData>(TData data) => new(data);

    public static DatabetResult Failure(DatabetErrorCode errorCode) => new(errorCode);

    public static DatabetResult<T> Failure<T>(
        DatabetErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);
}