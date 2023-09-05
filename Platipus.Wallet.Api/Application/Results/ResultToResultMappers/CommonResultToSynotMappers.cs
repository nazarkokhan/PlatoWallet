namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Synot;
using Synot.WithData;

public static class CommonResultToSynotMappers
{
    public static ISynotResult<TData> ToSynotFailureResult<TData>(this IResult result)
        => result.IsFailure
            ? SynotResultFactory.Failure<TData>(result.Error.ToSynotError(), exception: result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static ISynotResult<TData> ToSynotResult<TData>(this IResult result, TData response)
        => result.IsSuccess
            ? SynotResultFactory.Success(response)
            : SynotResultFactory.Failure<TData>(
                result.Error.ToSynotError(),
                exception: result.Exception);

    private static SynotError ToSynotError(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.CasinoNotFound => SynotError.CASINO_CLOSED,
            ErrorCode.SessionNotFound or ErrorCode.SessionAlreadyExists or ErrorCode.SessionExpired => SynotError.INVALID_TOKEN,
            ErrorCode.InvalidCurrency => SynotError.INVALID_CURRENCY,
            ErrorCode.RoundNotFound or ErrorCode.RoundAlreadyExists => SynotError.INVALID_GAME_ROUND,
            ErrorCode.RoundAlreadyFinished => SynotError.GAME_ROUND_CLOSED,
            ErrorCode.GameNotFound => SynotError.INVALID_GAME,
            ErrorCode.InsufficientFunds => SynotError.INSUFFICIENT_FUNDS,
            _ => SynotError.UNSPECIFIED
        };
    }
}