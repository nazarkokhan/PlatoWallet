namespace Platipus.Wallet.Api.Application.Results.Sw;

using WithData;

public static class SwResultFactory
{
    public static SwResult Success()
        => new();

    public static SwResult<TData> Success<TData>(TData data)
        => new(data);

    public static SwResult Failure(SwErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static SwResult<TData> Failure<TData>(
        SwErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static SwResult<TData> Failure<TData, TSourceData>(ISwResult<TSourceData> pswResult)
        => pswResult.IsFailure
            ? Failure<TData>(pswResult.Error, pswResult.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(pswResult));
}