namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using BetConstruct;
using BetConstruct.WithData;

public static class CommonResultToBetConstructMappers
{
    public static IBetConstructResult<TData> ToBetConstructResult<TData>(this IResult result)
        => result.IsFailure
            ? BetConstructResultFactory.Failure<TData>(result.ErrorCode.ToBetConstructErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IBetConstructResult ToBetConstructResult (this IResult result)
        => result.IsSuccess
            ? BetConstructResultFactory.Success()
            : BetConstructResultFactory.Failure(result.ErrorCode.ToBetConstructErrorCode(), result.Exception);

    private static BetConstructErrorCode ToBetConstructErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.CasinoNotFound => BetConstructErrorCode.GeneralError,
            ErrorCode.SessionNotFound => BetConstructErrorCode.TokenNotFound,
            ErrorCode.GameNotFound => BetConstructErrorCode.GameIsBlocked,
            ErrorCode.InvalidCurrency => BetConstructErrorCode.WrongCurrency,
            ErrorCode.InsufficientFunds => BetConstructErrorCode.NotEnoughMoney,
            ErrorCode.UserIsDisabled => BetConstructErrorCode.PlayerIsBlocked,
            ErrorCode.SecurityParameterIsInvalid => BetConstructErrorCode.AuthenticationFailed,
            ErrorCode.SessionExpired => BetConstructErrorCode.TokenExpired,
            ErrorCode.BadParametersInTheRequest => BetConstructErrorCode.IncorrectParametersPassed,
            ErrorCode.TransactionAlreadyExists => BetConstructErrorCode.TransactionIsAlreadyExist,
            ErrorCode.TransactionNotFound => BetConstructErrorCode.TransactionNotFound,
            ErrorCode.Unknown or _ => BetConstructErrorCode.GeneralError
        };
    }
}