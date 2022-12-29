namespace Platipus.Wallet.Api.Application.Results.Everymatrix;

using WithData;

public static class EverymatrixResultFactory
{
    public static EverymatrixResult Success() => new();

    public static EverymatrixResult<TData> Success<TData>(TData data) => new(data);

    public static EverymatrixResult Failure(EverymatrixErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static EverymatrixResult<TData> Failure<TData>(
        EverymatrixErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static EverymatrixResult<TData> Failure<TData, TSourceData>(IEverymatrixResult<TSourceData> pswResult)
        => pswResult.IsFailure
            ? Failure<TData>(pswResult.ErrorCode, pswResult.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(pswResult));
}