namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

public static class CommonResultToPswMappers
{
    public static IPswResult<TData> ToPswResult<TData>(this IResult result)
        => result.IsFailure
            ? PswResultFactory.Failure<TData>(result.ErrorCode.ToCommonErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IPswResult ToPswResult(this IResult result)
        => result.IsSuccess
            ? PswResultFactory.Success()
            : PswResultFactory.Failure(result.ErrorCode.ToCommonErrorCode(), result.Exception);

    private static PswErrorCode ToCommonErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.InsufficientFunds => PswErrorCode.NotEnoughMoney,
            ErrorCode.UserIsDisabled => PswErrorCode.UserDisabled,
            ErrorCode.SessionExpired => PswErrorCode.SessionExpired,
            ErrorCode.SecurityParameterIsInvalid => PswErrorCode.InvalidSignature,
            ErrorCode.SecurityParameterIsEmpty => PswErrorCode.MissingSignature,
            ErrorCode.BadParametersInTheRequest => PswErrorCode.BadParametersInTheRequest,
            ErrorCode.SessionNotFound => PswErrorCode.EmptySessionId,
            ErrorCode.UserNotFound => PswErrorCode.InvalidUser,
            ErrorCode.CasinoNotFound => PswErrorCode.InvalidCasinoId,
            ErrorCode.GameNotFound => PswErrorCode.InvalidGame,
            ErrorCode.InvalidCurrency => PswErrorCode.WrongCurrency,
            ErrorCode.TransactionAlreadyExists => PswErrorCode.DuplicateTransaction,
            ErrorCode.TransactionNotFound => PswErrorCode.TransactionDoesNotExist,
            ErrorCode.AwardIsAlreadyUsed => PswErrorCode.AwardIsAlreadyCanceled,
            ErrorCode.AwardNotFound => PswErrorCode.AwardDoesNotExist,
            ErrorCode.ErrorMockApplianceError => PswErrorCode.CouldNotTryToMockSessionError,
            ErrorCode.RequestAlreadyExists => PswErrorCode.Duplication,
            ErrorCode.Unknown or _ => PswErrorCode.Unknown
        };
    }
}