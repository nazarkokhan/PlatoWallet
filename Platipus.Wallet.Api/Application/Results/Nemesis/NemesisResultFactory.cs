namespace Platipus.Wallet.Api.Application.Results.Nemesis;

using WithData;

public static class NemesisResultFactory
{
    public static NemesisResult Success() => new();

    public static NemesisResult<TData> Success<TData>(TData data) => new(data);

    public static NemesisResult Failure(NemesisErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static NemesisResult<TData> Failure<TData>(
        NemesisErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static NemesisResult<TData> Failure<TData, TSourceData>(INemesisResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.Error, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}