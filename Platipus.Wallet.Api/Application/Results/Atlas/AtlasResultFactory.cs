namespace Platipus.Wallet.Api.Application.Results.Atlas;

using WithData;

public static class AtlasResultFactory
{
    public static AtlasResult Success() => new();

    public static AtlasResult<TData> Success<TData>(TData data) => new(data);

    public static AtlasResult Failure(
        AtlasErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static AtlasResult Failure(IAtlasResult result)
        => result.IsFailure
            ? Failure(result.Error, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static AtlasResult<TData> Failure<TData>(
        AtlasErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static AtlasResult<TData> Failure<TData, TSourceData>(
        IAtlasResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.Error, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}