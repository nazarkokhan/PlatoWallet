namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Evenbet;
using Evenbet.WithData;

public static class CommonResultToEvenbetMappers
{
    public static IEvenbetResult<TData> ToEvenbetFailureResult<TData>(this IResult result)
        => result.IsFailure
            ? EvenbetResultFactory.Failure<TData>(result.Error.ToEvenbetErrorCode(), exception: result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IEvenbetResult ToEvenbetResult(this IResult result)
        => result.IsSuccess
            ? EvenbetResultFactory.Success()
            : EvenbetResultFactory.Failure(
                result.Error.ToEvenbetErrorCode(),
                0,
                DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                exception: result.Exception);

    public static IEvenbetResult<TData> ToEvenbetResult<TData>(this IResult result, TData response)
        => result.IsSuccess
            ? EvenbetResultFactory.Success(response)
            : EvenbetResultFactory.Failure<TData>(
                result.Error.ToEvenbetErrorCode(),
                exception: result.Exception);

    public static EvenbetErrorCode ToEvenbetErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.CasinoNotFound => EvenbetErrorCode.INVALID_PARAMETER,
            ErrorCode.TransactionAlreadyExists => EvenbetErrorCode.TRANSACTION_ALREADY_SETTLED,
            ErrorCode.TransactionNotFound => EvenbetErrorCode.UNKNOWN_TRANSACTION_ID,
            ErrorCode.InsufficientFunds => EvenbetErrorCode.INSUFFICIENT_FUNDS,
            _ => EvenbetErrorCode.GENERAL_ERROR
        };
    }
}