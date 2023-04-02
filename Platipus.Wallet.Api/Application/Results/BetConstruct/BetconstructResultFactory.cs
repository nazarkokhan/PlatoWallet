namespace Platipus.Wallet.Api.Application.Results.BetConstruct;

using WithData;

public static class BetconstructResultFactory
{
    public static BetconstructResult Success()
        => new();

    public static BetconstructResult<TData> Success<TData>(TData data)
        => new(data);

    public static BetconstructResult Failure(BetconstructErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static BetconstructResult<TData> Failure<TData>(
        BetconstructErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static BetconstructResult<TData> Failure<TData, TSourceData>(IBetconstructResult<TSourceData> result)
        => result.IsFailure
            ? Failure<TData>(result.ErrorCode, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
}