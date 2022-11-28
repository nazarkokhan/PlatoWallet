namespace Platipus.Wallet.Api.Application.Results.Hub88.WithData.Mappers;

public static class CommonResultToSoftswissMappers
{
    public static ISoftswissResult<TData> ToSoftswissResult<TData>(this IResult result, long? balance = null)
        => result.IsFailure
            ? SoftswissResultFactory.Failure<TData>(result.ErrorCode.ToSoftswissErrorCode(), balance, result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static ISoftswissResult ToSoftswissResult(this IResult result, long? balance = null)
        => result.IsSuccess
            ? SoftswissResultFactory.Success()
            : SoftswissResultFactory.Failure(result.ErrorCode.ToSoftswissErrorCode(), balance, result.Exception);

    public static SoftswissErrorCode ToSoftswissErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.NotEnoughMoney => SoftswissErrorCode.BadRequest,
            ErrorCode.UserDisabled => SoftswissErrorCode.PlayerIsDisabled,
            ErrorCode.SessionExpired => SoftswissErrorCode.Forbidden,
            ErrorCode.MissingSignature or ErrorCode.InvalidSignature => SoftswissErrorCode.Forbidden,
            ErrorCode.RoomIsWrongOrEmpty => SoftswissErrorCode.GameIsForbiddenToThePlayer,
            ErrorCode.BadParametersInTheRequest => SoftswissErrorCode.BadRequest,
            ErrorCode.InvalidCasinoId => SoftswissErrorCode.BadRequest,
            ErrorCode.InvalidGame => SoftswissErrorCode.GameIsForbiddenToThePlayer,
            ErrorCode.InvalidExpirationDate => SoftswissErrorCode.Forbidden,
            ErrorCode.WrongCurrency => SoftswissErrorCode.CurrencyIsNotAllowedForThePlayer,
            ErrorCode.DuplicateTransaction => SoftswissErrorCode.BadRequest,
            ErrorCode.TransactionDoesNotExist => SoftswissErrorCode.BadRequest,
            ErrorCode.InvalidSign => SoftswissErrorCode.Forbidden,
            ErrorCode.Unknown or _ => SoftswissErrorCode.UnknownError
        };
    }
}