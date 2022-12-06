namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using ISoftBet;
using ISoftBet.WithData;

public static class CommonResultToSoftBetMappers
{
    public static ISoftBetResult<TData> ToSoftBetResult<TData>(this IResult result)
        => result.IsFailure
            ? SoftBetResultFactory.Failure<TData>(result.ErrorCode.ToSoftBetErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static ISoftBetResult ToSoftBetResult(this IResult result)
        => result.IsSuccess
            ? SoftBetResultFactory.Success()
            : SoftBetResultFactory.Failure(result.ErrorCode.ToSoftBetErrorCode(), result.Exception);

    public static SoftBetErrorMessage ToSoftBetErrorCode(this ErrorCode source)
    {
        return source switch
        {
            // ErrorCode.NotEnoughMoney => SoftBetErrorCode.RS_ERROR_NOT_ENOUGH_MONEY,
            // ErrorCode.UserDisabled => SoftBetErrorCode.RS_ERROR_USER_DISABLED,
            // ErrorCode.SessionExpired => SoftBetErrorCode.RS_ERROR_TOKEN_EXPIRED,
            // ErrorCode.MissingSignature or ErrorCode.InvalidSignature => SoftBetErrorCode.RS_ERROR_INVALID_SIGNATURE,
            // ErrorCode.RoomIsWrongOrEmpty => SoftBetErrorCode.RS_ERROR_INVALID_GAME,
            // ErrorCode.BadParametersInTheRequest => SoftBetErrorCode.RS_ERROR_WRONG_SYNTAX,
            // ErrorCode.InvalidCasinoId => SoftBetErrorCode.RS_ERROR_INVALID_PARTNER,
            // ErrorCode.InvalidGame => SoftBetErrorCode.RS_ERROR_INVALID_GAME,
            // ErrorCode.InvalidExpirationDate => SoftBetErrorCode.RS_ERROR_TOKEN_EXPIRED,
            // ErrorCode.WrongCurrency => SoftBetErrorCode.RS_ERROR_WRONG_CURRENCY,
            // ErrorCode.DuplicateTransaction => SoftBetErrorCode.RS_ERROR_DUPLICATE_TRANSACTION,
            // ErrorCode.TransactionDoesNotExist => SoftBetErrorCode.RS_ERROR_TRANSACTION_DOES_NOT_EXIST,
            // ErrorCode.InvalidSign => SoftBetErrorCode.RS_ERROR_INVALID_TOKEN,
            ErrorCode.Unknown or _ => SoftBetErrorMessage.GeneralRequestError
        };
    }
}