namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using PariMatch;
using PariMatch.WithData;

public static class CommonResultToParimatchMappers
{
    public static IPariMatchResult<TData> ToParimatchResult<TData>(this IResult result)
        => result.IsFailure
            ? PariMatchResultFactory.Failure<TData>(result.ErrorCode.ToParimatchErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IPariMatchResult ToReevoResult(this IResult result)
        => result.IsSuccess
            ? PariMatchResultFactory.Success()
            : PariMatchResultFactory.Failure(result.ErrorCode.ToParimatchErrorCode(), result.Exception);

    private static PariMatchErrorCode ToParimatchErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.SessionNotFound => PariMatchErrorCode.InvalidSessionKey,
            ErrorCode.UserIsDisabled => PariMatchErrorCode.LockedPlayer,
            ErrorCode.InsufficientFunds => PariMatchErrorCode.InsufficientBalance,
            ErrorCode.TransactionNotFound => PariMatchErrorCode.InvalidTransactionId,
            ErrorCode.Unknown or _ => PariMatchErrorCode.ErrorInternal
        };
    }
}