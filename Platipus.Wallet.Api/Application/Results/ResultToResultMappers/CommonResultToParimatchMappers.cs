namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Parimatch;
using Parimatch.WithData;

public static class CommonResultToParimatchMappers
{
    public static IParimatchResult<TData> ToParimatchFailureResult<TData>(this IResult result)
    {
        return result.IsFailure
            ? ParimatchResultFactory.Failure<TData>(result.Error.ToParimatchErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
    }

    private static ParimatchErrorCode ToParimatchErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.SessionNotFound or ErrorCode.SessionExpired => ParimatchErrorCode.InvalidSessionKey,
            ErrorCode.InsufficientFunds => ParimatchErrorCode.InsufficientBalance,
            ErrorCode.UserIsDisabled => ParimatchErrorCode.LockedPlayer,
            ErrorCode.TransactionNotFound => ParimatchErrorCode.InvalidTransactionId,
            ErrorCode.Unknown => ParimatchErrorCode.ErrorInternal,
            _ => ParimatchErrorCode.InvalidCasinoLogic
        };
    }
}