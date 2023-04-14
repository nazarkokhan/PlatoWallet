namespace Platipus.Wallet.Api.Application.Results.Dafabet;

public static class DafabetResultFactory
{
    public static DafabetResult Success()
        => new();

    public static DafabetResult<TData> Success<TData>(TData data)
        => new(data);

    public static DafabetResult Failure(DafabetErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static DafabetResult<T> Failure<T>(
        DafabetErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static DafabetResult<TData> Failure<TData, TSourceData>(IDafabetResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.Error, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}