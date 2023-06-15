using Platipus.Wallet.Api.Application.Results.AtlasPlatform.WithData;

namespace Platipus.Wallet.Api.Application.Results.AtlasPlatform;

public static class AtlasPlatformResultFactory
{
    public static AtlasPlatformResult Success() => new();

    public static AtlasPlatformResult<TData> Success<TData>(TData data) => new(data);

    public static AtlasPlatformResult Failure(
        AtlasPlatformErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static AtlasPlatformResult Failure(IAtlasPlatformResult result)
        => result.IsFailure
            ? Failure(result.Error, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static AtlasPlatformResult<TData> Failure<TData>(
        AtlasPlatformErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static AtlasPlatformResult<TData> Failure<TData, TSourceData>(
        IAtlasPlatformResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.Error, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}