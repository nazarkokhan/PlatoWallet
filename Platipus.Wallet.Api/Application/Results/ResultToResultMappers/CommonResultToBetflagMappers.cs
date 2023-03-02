namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Betflag;
using Betflag.WithData;

public static class CommonResultToBetflagMappers
{
    public static IBetflagResult<TData> ToBetflagResult<TData>(this IResult result)
        => result.IsFailure
            ? BetflagResultFactory.Failure<TData>(result.ErrorCode.ToBetflagErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IBetflagResult ToBetflagResult(this IResult result)
        => result.IsSuccess
            ? BetflagResultFactory.Success()
            : BetflagResultFactory.Failure(result.ErrorCode.ToBetflagErrorCode(), result.Exception);

    private static BetflagErrorCode ToBetflagErrorCode(this ErrorCode source)
    {
        return source switch
        {
            // ErrorCode. => BetflagErrorCode.SUCCSESS,
            ErrorCode.SessionNotFound => BetflagErrorCode.InvalidToken,
            ErrorCode.UnknownBetException => BetflagErrorCode.BetFail2,
            ErrorCode.InsufficientFunds => BetflagErrorCode.InsufficientFunds,
            ErrorCode.CasinoNotFound => BetflagErrorCode.WalletNotFound,
            ErrorCode.BadParametersInTheRequest => BetflagErrorCode.InvalidParameter,
            ErrorCode.TransactionNotFound => BetflagErrorCode.BetNotFound,
            // ErrorCode. => BetflagErrorCode.ProviderOnMaintenance,
            ErrorCode.SessionExpired => BetflagErrorCode.SessionExpired,
            ErrorCode.UserIsDisabled => BetflagErrorCode.ClientBlocked,
            // ErrorCode. => BetflagErrorCode.BetWithCancel,
            // ErrorCode. => BetflagErrorCode.WinWithoutBet,
            ErrorCode.UnknownWinException => BetflagErrorCode.WinWithBetError,
            // ErrorCode. => BetflagErrorCode.WinWithBetCancelled,
            // ErrorCode. => BetflagErrorCode.CancelReferBetNotExists,
            // ErrorCode. => BetflagErrorCode.CancelIncorrectAmount,
            // ErrorCode. => BetflagErrorCode.CancelReferBetInTimeout,
            ErrorCode.UnknownRollbackException => BetflagErrorCode.CancelReferBetInError,
            // ErrorCode. => BetflagErrorCode.CancelWithWin,
            ErrorCode.RoundNotFound => BetflagErrorCode.RoundEndBetNotExists,
            // ErrorCode. => BetflagErrorCode.RoundEndBetInError,
            ErrorCode.UnknownLogoutException => BetflagErrorCode.SessionCloseInconsistentValues,
            // ErrorCode. => BetflagErrorCode.SessionOpenErrorOpenTicket,
            // ErrorCode. => BetflagErrorCode.WalletException,
            // ErrorCode. => BetflagErrorCode.TemporanyError,
            // ErrorCode. => BetflagErrorCode.Exception,
            ErrorCode.Unknown or _ => BetflagErrorCode.GeneralError
        };
    }
}