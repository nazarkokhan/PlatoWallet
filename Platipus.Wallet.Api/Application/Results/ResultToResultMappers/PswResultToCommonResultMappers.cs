namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

public static class PswResultToCommonResultMappers
{
    public static IResult<TData> ToHub88Result<TData>(this IPswResult result)
        => result.IsFailure
            ? ResultFactory.Failure<TData>(result.ErrorCode.ToCommonErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IResult ToHub88Result(this IPswResult result)
        => result.IsSuccess
            ? ResultFactory.Success()
            : ResultFactory.Failure(result.ErrorCode.ToCommonErrorCode(), result.Exception);

    public static ErrorCode ToCommonErrorCode(this PswErrorCode source)
    {
        return source switch
        {
            PswErrorCode.NotEnoughMoney => ErrorCode.NotEnoughMoney,
            PswErrorCode.UserDisabled => ErrorCode.UserDisabled,
            PswErrorCode.SessionExpired => ErrorCode.SessionExpired,
            PswErrorCode.InvalidSignature => ErrorCode.InvalidSignature,
            PswErrorCode.MissingSignature => ErrorCode.MissingSignature,
            PswErrorCode.RoomIsWrongOrEmpty => ErrorCode.RoomIsWrongOrEmpty,
            PswErrorCode.BadParametersInTheRequest => ErrorCode.BadParametersInTheRequest,
            PswErrorCode.CommandIsWrong => ErrorCode.CommandIsWrong,
            PswErrorCode.EmptySessionId => ErrorCode.EmptySessionId,
            PswErrorCode.BetLimitReached => ErrorCode.BetLimitReached,
            PswErrorCode.InvalidUser => ErrorCode.InvalidUser,
            PswErrorCode.InvalidCasinoId => ErrorCode.InvalidCasinoId,
            PswErrorCode.InvalidGame => ErrorCode.InvalidGame,
            PswErrorCode.InvalidExpirationDate => ErrorCode.InvalidExpirationDate,
            PswErrorCode.WrongCurrency => ErrorCode.WrongCurrency,
            PswErrorCode.DuplicateTransaction => ErrorCode.DuplicateTransaction,
            PswErrorCode.TransactionDoesNotExist => ErrorCode.TransactionDoesNotExist,
            PswErrorCode.DuplicateAward => ErrorCode.DuplicateAward,
            PswErrorCode.AwardIsAlreadyCanceled => ErrorCode.AwardIsAlreadyCanceled,
            PswErrorCode.AwardDoesNotExist => ErrorCode.AwardDoesNotExist,
            PswErrorCode.InvalidHash => ErrorCode.InvalidSign,
            PswErrorCode.CouldNotTryToMockSessionError => ErrorCode.CouldNotTryToMockSessionError,
            PswErrorCode.Duplication => ErrorCode.Duplication,
            PswErrorCode.Unknown or _ => ErrorCode.Unknown,
        };
    }
}