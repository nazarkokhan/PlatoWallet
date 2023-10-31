using Platipus.Wallet.Api.Application.Results.Sweepium;
using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;

namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

public static class CommonResultToSweepiumMappers
{
    public static ISweepiumResult<TData> ToSweepiumErrorResult<TData>(this IResult result)
    {
        throw new NotImplementedException();
    }

    public static ISweepiumResult<TData> ToSweepiumResult<TData>(this IResult result, TData response)
    {
        throw new NotImplementedException();
    }

    private static SweepiumErrorCode ToSweepiumErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.InsufficientFunds => SweepiumErrorCode.Insufficient_Funds,
            ErrorCode.SessionExpired => SweepiumErrorCode.Session_Not_Enabled,
            ErrorCode.SessionNotFound => SweepiumErrorCode.Session_Not_Found,
            ErrorCode.GameNotFound => SweepiumErrorCode.Wrong_Game_ID,
            ErrorCode.TransactionAlreadyExists => SweepiumErrorCode.Transaction_AlreadyExists,
            ErrorCode.TransactionNotFound => SweepiumErrorCode.Transaction_Not_Found,
            ErrorCode.UserIsDisabled => SweepiumErrorCode.Player_Is_Blocked,
            ErrorCode.InvalidCurrency => SweepiumErrorCode.Wrong_Currency,
            ErrorCode.CasinoNotFound => SweepiumErrorCode.Incorrect_Parameters_Passed,
            ErrorCode.UserNotFound => SweepiumErrorCode.Incorrect_Parameters_Passed,
            ErrorCode.BadParametersInTheRequest => SweepiumErrorCode.Incorrect_Parameters_Passed,
            ErrorCode.InvalidPassword => SweepiumErrorCode.Authentication_Failed,
            ErrorCode.UnknownBetException => SweepiumErrorCode.Wrong_Bet_Amount,
            ErrorCode.UnknownAwardException => SweepiumErrorCode.Wrong_Win_Amount,
            _ => SweepiumErrorCode.General_Error
        };
    }
}