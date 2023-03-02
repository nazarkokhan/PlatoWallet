namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

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

    private static SoftswissErrorCode ToSoftswissErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.InsufficientFunds => SoftswissErrorCode.PlayerHasNotEnoughFundsToProcessAnAction,
            ErrorCode.UserNotFound => SoftswissErrorCode.PlayerIsInvalid,
            // ErrorCode. => SoftswissErrorCode.PlayerReachedCustomizedBetLimit,
            // ErrorCode. => SoftswissErrorCode.BetExceededMaxBetLimit,
            // ErrorCode. => SoftswissErrorCode.GameIsForbiddenToThePlayer,
            ErrorCode.UserIsDisabled => SoftswissErrorCode.PlayerIsDisabled,
            // ErrorCode. => SoftswissErrorCode.GameIsNotAvailableInPlayerCountry,
            // ErrorCode. => SoftswissErrorCode.CurrencyIsNotAllowedForThePlayer,
            // ErrorCode. => SoftswissErrorCode.ForbiddenToChangeAlreadySetField,
            ErrorCode.BadParametersInTheRequest => SoftswissErrorCode.BadRequest,
            ErrorCode.SessionNotFound or ErrorCode.SecurityParameterIsEmpty or ErrorCode.SecurityParameterIsInvalid =>
                SoftswissErrorCode.Forbidden,
            ErrorCode.TransactionNotFound or ErrorCode.RoundNotFound => SoftswissErrorCode.NotFound,
            ErrorCode.GameNotFound => SoftswissErrorCode.GameIsNotAvailableToYourCasino,
            ErrorCode.AwardNotFound => SoftswissErrorCode.FreeSpinsAreNotAvailableForYourCasino,
            // ErrorCode. => SoftswissErrorCode.UnknownErrorInExternalService,
            // ErrorCode. => SoftswissErrorCode.ServiceIsUnavailable,
            // ErrorCode. => SoftswissErrorCode.RequestTimedOut,
            // ErrorCode. => SoftswissErrorCode.GameProviderDoesntProvideFreeSpins,
            ErrorCode.UnknownAwardException => SoftswissErrorCode.ImpossibleToIssueFreeSpinsInRequestedGame,
            // ErrorCode. => SoftswissErrorCode.YouShouldProvideAtLeastOneGameToIssueFreeSpins,
            ErrorCode.SessionExpired => SoftswissErrorCode.BadExpirationDate,
            // ErrorCode. => SoftswissErrorCode.CantChangeIssueStateFromItsCurrentToRequested,
            // ErrorCode. => SoftswissErrorCode.YouCantChangeIssueStateWhenIssueStatusIsNotSynced,
            // ErrorCode. => SoftswissErrorCode.CantIssueFreeSpinsForDifferentGameProviders,
            // ErrorCode. => SoftswissErrorCode.InvalidFreeSpinsIssue,
            // ErrorCode. => SoftswissErrorCode.FreeSpinsIssueHasAlreadyExpired,
            // ErrorCode. => SoftswissErrorCode.FreeSpinsIssueCantBeCanceled,
            // ErrorCode. => SoftswissErrorCode.RequestedLiveGameIsNotAvailableRightNow,
            ErrorCode.Unknown or _ => SoftswissErrorCode.UnknownError,
        };
    }
}