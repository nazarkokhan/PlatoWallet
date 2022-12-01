namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Hub88;
using Hub88.WithData;

public static class CommonResultToHub88Mappers
{
    public static IHub88Result<TData> ToHub88Result<TData>(this IResult result)
        => result.IsFailure
            ? Hub88ResultFactory.Failure<TData>(result.ErrorCode.ToHub88ErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IHub88Result ToHub88Result(this IResult result)
        => result.IsSuccess
            ? Hub88ResultFactory.Success()
            : Hub88ResultFactory.Failure(result.ErrorCode.ToHub88ErrorCode(), result.Exception);

    public static Hub88ErrorCode ToHub88ErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.NotEnoughMoney => Hub88ErrorCode.RS_ERROR_NOT_ENOUGH_MONEY,
            ErrorCode.UserDisabled => Hub88ErrorCode.RS_ERROR_USER_DISABLED,
            ErrorCode.SessionExpired => Hub88ErrorCode.RS_ERROR_TOKEN_EXPIRED,
            ErrorCode.MissingSignature or ErrorCode.InvalidSignature => Hub88ErrorCode.RS_ERROR_INVALID_SIGNATURE,
            ErrorCode.RoomIsWrongOrEmpty => Hub88ErrorCode.RS_ERROR_INVALID_GAME,
            ErrorCode.BadParametersInTheRequest => Hub88ErrorCode.RS_ERROR_WRONG_SYNTAX,
            ErrorCode.InvalidCasinoId => Hub88ErrorCode.RS_ERROR_INVALID_PARTNER,
            ErrorCode.InvalidGame => Hub88ErrorCode.RS_ERROR_INVALID_GAME,
            ErrorCode.InvalidExpirationDate => Hub88ErrorCode.RS_ERROR_TOKEN_EXPIRED,
            ErrorCode.WrongCurrency => Hub88ErrorCode.RS_ERROR_WRONG_CURRENCY,
            ErrorCode.DuplicateTransaction => Hub88ErrorCode.RS_ERROR_DUPLICATE_TRANSACTION,
            ErrorCode.TransactionDoesNotExist => Hub88ErrorCode.RS_ERROR_TRANSACTION_DOES_NOT_EXIST,
            ErrorCode.InvalidSign => Hub88ErrorCode.RS_ERROR_INVALID_TOKEN,
            ErrorCode.Unknown or _ => Hub88ErrorCode.RS_ERROR_UNKNOWN
        };
    }
}