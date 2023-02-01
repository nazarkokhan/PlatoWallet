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

    public static BetflagErrorCode ToBetflagErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.NotEnoughMoney => BetflagErrorCode.InsufficientFunds,
            ErrorCode.SessionExpired => BetflagErrorCode.SessionExpired,
            ErrorCode.MissingSignature or ErrorCode.InvalidSignature => BetflagErrorCode.InvalidToken,
            ErrorCode.BadParametersInTheRequest => BetflagErrorCode.InvalidParameter,
            ErrorCode.DuplicateTransaction => BetflagErrorCode.BetFail2,
            ErrorCode.TransactionDoesNotExist => BetflagErrorCode.RoundEndBetNotExists,
            ErrorCode.InvalidSign => BetflagErrorCode.InvalidToken,
            ErrorCode.Unknown or _ => BetflagErrorCode.GeneralError
        };
    }
}