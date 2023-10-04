namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using BetConstruct;
using BetConstruct.WithData;

public static class CommonResultToBetconstructMappers
{
    public static IBetconstructResult<TData> ToBetConstructResult<TData>(this IResult result, TData response)
        => result.IsSuccess
            ? BetconstructResultFactory.Success(response)
            : BetconstructResultFactory.Failure<TData>(
                result.Error.ToBetConstructErrorCode(),
                exception: result.Exception);

    public static IBetconstructResult<TData> ToBetConstructResult<TData>(this IResult result)
        => result.IsFailure
            ? BetconstructResultFactory.Failure<TData>(result.Error.ToBetConstructErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IBetconstructResult ToBetConstructResult(this IResult result)
        => result.IsSuccess
            ? BetconstructResultFactory.Success()
            : BetconstructResultFactory.Failure(result.Error.ToBetConstructErrorCode(), result.Exception);

    private static BetconstructErrorCode ToBetConstructErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.CasinoNotFound => BetconstructErrorCode.GeneralError,
            ErrorCode.SessionNotFound => BetconstructErrorCode.TokenNotFound,
            ErrorCode.GameNotFound => BetconstructErrorCode.GameIsBlocked,
            ErrorCode.InvalidCurrency => BetconstructErrorCode.WrongCurrency,
            ErrorCode.InsufficientFunds => BetconstructErrorCode.NotEnoughMoney,
            ErrorCode.UserIsDisabled => BetconstructErrorCode.PlayerIsBlocked,
            ErrorCode.SecurityParameterIsInvalid => BetconstructErrorCode.AuthenticationFailed,
            ErrorCode.SessionExpired => BetconstructErrorCode.TokenExpired,
            ErrorCode.BadParametersInTheRequest => BetconstructErrorCode.IncorrectParametersPassed,
            ErrorCode.TransactionAlreadyExists => BetconstructErrorCode.TransactionIsAlreadyExist,
            ErrorCode.TransactionNotFound => BetconstructErrorCode.TransactionNotFound,
            ErrorCode.Unknown or _ => BetconstructErrorCode.GeneralError
        };
    }
}