namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Evoplay;
using Evoplay.WithData;

public static class CommonResultToEvoplayMappers
{
    public static IEvoplayResult<TData> ToEvoplayFailureResult<TData>(this IResult result)
        => result.IsFailure
            ? EvoplayResultFactory.Failure<TData>(result.Error.ToEvoplayErrorCode(), exception: result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IEvoplayResult ToEvoplayResult(this IResult result)
        => result.IsSuccess
            ? EvoplayResultFactory.Success()
            : EvoplayResultFactory.Failure(result.Error.ToEvoplayErrorCode(), "0", "EUR",
                exception: result.Exception);

    public static IEvoplayResult<TData> ToEvoplayResult<TData>(
        this IResult result, TData response)
        => result.IsSuccess
            ? EvoplayResultFactory.Success(response)
            : EvoplayResultFactory.Failure<TData>(result.Error.ToEvoplayErrorCode(), 
                exception: result.Exception);

    private static EvoplayErrorCode ToEvoplayErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.CasinoNotFound => EvoplayErrorCode.E_PROVIDER_NOT_FOUND,
            ErrorCode.TransactionAlreadyExists => EvoplayErrorCode.E_INVALID_TRANSACTION_ID,
            ErrorCode.UserNotFound => EvoplayErrorCode.E_INVALID_USER_ID,
            ErrorCode.UserIsDisabled => EvoplayErrorCode.E_PLAYER_IS_LOCKED,
            ErrorCode.RoundAlreadyExists => EvoplayErrorCode.E_UNEXPECTED_LOGIC,
            ErrorCode.RoundAlreadyFinished => EvoplayErrorCode.E_UNEXPECTED_LOGIC,
            ErrorCode.InvalidCurrency => EvoplayErrorCode.E_UNEXPECTED_LOGIC,
            ErrorCode.RoundNotFound => EvoplayErrorCode.E_UNEXPECTED_LOGIC,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }
}