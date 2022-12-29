namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Everymatrix;
using Everymatrix.WithData;

public static class CommonResultToEverymatrixMappers
{
    public static IEverymatrixResult<TData> ToEverymatrixResult<TData>(this IResult result)
        => result.IsFailure
            ? EverymatrixResultFactory.Failure<TData>(result.ErrorCode.ToEverymatrixErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IEverymatrixResult ToEverymatrixResult(this IResult result)
        => result.IsSuccess
            ? EverymatrixResultFactory.Success()
            : EverymatrixResultFactory.Failure(result.ErrorCode.ToEverymatrixErrorCode(), result.Exception);

    public static EverymatrixErrorCode ToEverymatrixErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.NotEnoughMoney => EverymatrixErrorCode.InsufficientFunds,
            ErrorCode.UserDisabled => EverymatrixErrorCode.UserIsBlocked,
            ErrorCode.InvalidSignature or ErrorCode.InvalidSign => EverymatrixErrorCode.InvalidHash,
            ErrorCode.MissingSignature or ErrorCode.SessionExpired => EverymatrixErrorCode.TokenNotFound,
            ErrorCode.BetLimitReached => EverymatrixErrorCode.DoubleTransaction,
            ErrorCode.InvalidUser => EverymatrixErrorCode.UserIsBlocked,
            // ErrorCode.InvalidCasinoId => expr,
            // ErrorCode.InvalidGame => expr,
            // ErrorCode.InvalidExpirationDate => expr,
            // ErrorCode.WrongCurrency => EverymatrixErrorCode.CurrencyDoesntMatch,
            // ErrorCode.DuplicateTransaction => expr,
            // ErrorCode.TransactionDoesNotExist => expr,
            ErrorCode.Unknown or _ => EverymatrixErrorCode.UnknownError,
        };
    }
}