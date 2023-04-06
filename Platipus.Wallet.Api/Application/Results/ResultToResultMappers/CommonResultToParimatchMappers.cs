namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using PariMatch;
using PariMatch.WithData;

public static class CommonResultToParimatchMappers
{
    public static IParimatchResult<TData> ToParimatchResult<TData>(this IResult result)
        => result.IsFailure
            ? ParimatchResultFactory.Failure<TData>(result.ErrorCode.ToParimatchErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IParimatchResult ToReevoResult(this IResult result)
        => result.IsSuccess
            ? ParimatchResultFactory.Success()
            : ParimatchResultFactory.Failure(result.ErrorCode.ToParimatchErrorCode(), result.Exception);

    private static ParimatchErrorCode ToParimatchErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.SessionNotFound => ParimatchErrorCode.InvalidSessionKey,
            ErrorCode.UserIsDisabled => ParimatchErrorCode.LockedPlayer,
            ErrorCode.InsufficientFunds => ParimatchErrorCode.InsufficientBalance,
            ErrorCode.TransactionNotFound => ParimatchErrorCode.InvalidTransactionId,
            ErrorCode.Unknown or _ => ParimatchErrorCode.ErrorInternal
        };
    }
}