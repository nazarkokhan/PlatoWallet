namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Everymatrix;
using Everymatrix.WithData;

public static class CommonResultToEverymatrixMappers
{
    public static IEverymatrixResult<TData> ToEverymatrixResult<TData>(this IResult result, TData response)
        => result.IsSuccess
            ? EverymatrixResultFactory.Success(response)
            : EverymatrixResultFactory.Failure<TData>(
                result.Error.ToEverymatrixErrorCode(),
                exception: result.Exception);

    public static IEverymatrixResult<TData> ToEverymatrixResult<TData>(this IResult result)
        => result.IsFailure
            ? EverymatrixResultFactory.Failure<TData>(result.Error.ToEverymatrixErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IEverymatrixResult ToEverymatrixResult(this IResult result)
        => result.IsSuccess
            ? EverymatrixResultFactory.Success()
            : EverymatrixResultFactory.Failure(result.Error.ToEverymatrixErrorCode(), result.Exception);

    private static EverymatrixErrorCode ToEverymatrixErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.SessionNotFound => EverymatrixErrorCode.TokenNotFound,
            ErrorCode.UserIsDisabled => EverymatrixErrorCode.UserIsBlocked,
            ErrorCode.InsufficientFunds => EverymatrixErrorCode.InsufficientFunds,
            ErrorCode.UserNotFound => EverymatrixErrorCode.VendorAccountNotActive,
            // ErrorCode. => EverymatrixErrorCode.IpIsNotAllowed,
            ErrorCode.InvalidCurrency => EverymatrixErrorCode.CurrencyDoesntMatch,
            ErrorCode.TransactionNotFound => EverymatrixErrorCode.TransactionNotFound,
            ErrorCode.TransactionAlreadyExists => EverymatrixErrorCode.DoubleTransaction,
            ErrorCode.SecurityParameterIsInvalid => EverymatrixErrorCode.InvalidHash,
            ErrorCode.CasinoNotFound => EverymatrixErrorCode.CasinoLossLimit,
            ErrorCode.Unknown or _ => EverymatrixErrorCode.UnknownError
        };
    }
}