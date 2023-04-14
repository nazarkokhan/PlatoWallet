namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Hub88;
using Hub88.WithData;

public static class CommonResultToHub88Mappers
{
    public static IHub88Result<TData> ToHub88Result<TData>(this IResult result)
        => result.IsFailure
            ? Hub88ResultFactory.Failure<TData>(result.Error.ToHub88ErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IHub88Result ToHub88Result(this IResult result)
        => result.IsSuccess
            ? Hub88ResultFactory.Success()
            : Hub88ResultFactory.Failure(result.Error.ToHub88ErrorCode(), result.Exception);

    private static Hub88ErrorCode ToHub88ErrorCode(this ErrorCode source)
    {
        return source switch
        {
            // ErrorCode. => Hub88ErrorCode.RS_OK,
            ErrorCode.CasinoNotFound => Hub88ErrorCode.RS_ERROR_INVALID_PARTNER,
            ErrorCode.SessionNotFound => Hub88ErrorCode.RS_ERROR_INVALID_TOKEN,
            ErrorCode.GameNotFound => Hub88ErrorCode.RS_ERROR_INVALID_GAME,
            ErrorCode.InvalidCurrency => Hub88ErrorCode.RS_ERROR_WRONG_CURRENCY,
            ErrorCode.InsufficientFunds => Hub88ErrorCode.RS_ERROR_NOT_ENOUGH_MONEY,
            ErrorCode.UserIsDisabled => Hub88ErrorCode.RS_ERROR_USER_DISABLED,
            ErrorCode.SecurityParameterIsInvalid => Hub88ErrorCode.RS_ERROR_INVALID_SIGNATURE,
            ErrorCode.SessionExpired => Hub88ErrorCode.RS_ERROR_TOKEN_EXPIRED,
            ErrorCode.BadParametersInTheRequest => Hub88ErrorCode.RS_ERROR_WRONG_SYNTAX,
            // ErrorCode. => Hub88ErrorCode.RS_ERROR_WRONG_TYPES,
            ErrorCode.TransactionAlreadyExists => Hub88ErrorCode.RS_ERROR_DUPLICATE_TRANSACTION,
            ErrorCode.TransactionNotFound => Hub88ErrorCode.RS_ERROR_TRANSACTION_DOES_NOT_EXIST,
            ErrorCode.Unknown or _ => Hub88ErrorCode.RS_ERROR_UNKNOWN
        };
    }
}