namespace Platipus.Wallet.Api.Application.Results.Uis;

using WithData;

public static class UisResultFactory
{
    public static UisResult Success() => new();

    public static UisResult<TData> Success<TData>(TData data) => new(data);

    public static UisResult Failure(UisErrorCode errorCode, Exception? exception = null) => new(errorCode, exception);

    public static UisResult<TData> Failure<TData>(
        UisErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static UisResult<TData> Failure<TData, TSourceData>(IUisResult<TSourceData> pswResult)
        => pswResult.IsFailure
            ? Failure<TData>(pswResult.Error, pswResult.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(pswResult));
}