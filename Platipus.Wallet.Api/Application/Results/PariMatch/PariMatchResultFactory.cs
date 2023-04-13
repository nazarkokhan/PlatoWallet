namespace Platipus.Wallet.Api.Application.Results.PariMatch;

using WithData;

public static class PariMatchResultFactory
{
    public static PariMatchResult Success()
        => new();

    public static PariMatchResult<TData> Success<TData>(TData data)
        => new(data);

    public static PariMatchResult Failure(PariMatchErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static PariMatchResult<TData> Failure<TData>(
        PariMatchErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static PariMatchResult<TData> Failure<TData, TSourceData>(IPariMatchResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.Error, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}