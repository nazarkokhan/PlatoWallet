namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Results.EmaraPlay;
using Results.EmaraPlay.WithData;

public static class CommonResultToEmaraplayMappers
{
    public static IEmaraPlayResult<TData> ToEmaraplayResult<TData>(this IResult result)
        => result.IsFailure
            ? EmaraPlayResultFactory.Failure<TData>(result.ErrorCode.ToEmaraplayErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IEmaraPlayResult ToEmaraplayResult(this IResult result)
        => result.IsSuccess
            ? EmaraPlayResultFactory.Success()
            : EmaraPlayResultFactory.Failure(result.ErrorCode.ToEmaraplayErrorCode(), result.Exception);

    private static EmaraPlayErrorCode ToEmaraplayErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.CasinoNotFound => EmaraPlayErrorCode.ProviderNotFound,
            ErrorCode.SessionNotFound => EmaraPlayErrorCode.PlayerAuthenticationFailed,
            ErrorCode.GameNotFound => EmaraPlayErrorCode.GameIsNotFoundOrDisabled,
            ErrorCode.InvalidCurrency => EmaraPlayErrorCode.InvalidUserCurrency,
            ErrorCode.InsufficientFunds => EmaraPlayErrorCode.InsufficientBalance,
            ErrorCode.UserIsDisabled => EmaraPlayErrorCode.PlayerNotFound,
            ErrorCode.SecurityParameterIsInvalid => EmaraPlayErrorCode.PlayerAuthenticationFailed,
            ErrorCode.SessionExpired => EmaraPlayErrorCode.PlayerAuthenticationFailed,
            ErrorCode.BadParametersInTheRequest => EmaraPlayErrorCode.BadParameters,
            ErrorCode.TransactionAlreadyExists => EmaraPlayErrorCode.DuplicatedTransaction,
            ErrorCode.TransactionNotFound => EmaraPlayErrorCode.TransactionNotFound,
            ErrorCode.RoundAlreadyFinished => EmaraPlayErrorCode.BetRoundAlreadyClosed,
            ErrorCode.Unknown or _ => EmaraPlayErrorCode.InternalServerError
        };
    }
}