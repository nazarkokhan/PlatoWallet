namespace Platipus.Wallet.Api.Application.Results.Softswiss;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum SoftswissErrorCode
{
//1xx
    PlayerHasNotEnoughFundsToProcessAnAction = 100,
    PlayerIsInvalid = 101,
    PlayerReachedCustomizedBetLimit = 105,
    BetExceededMaxBetLimit = 106,
    GameIsForbiddenToThePlayer = 107,
    PlayerIsDisabled = 110,
    GameIsNotAvailableInPlayerCountry = 153,
    CurrencyIsNotAllowedForThePlayer = 154,
    ForbiddenToChangeAlreadySetField = 155,

//4xx
    BadRequest = 400,
    Forbidden = 403,
    NotFound = 404,
    GameIsNotAvailableToYourCasino = 405,
    FreeSpinsAreNotAvailableForYourCasino = 406,

//5xx
    UnknownError = 500,
    UnknownErrorInExternalService = 502,
    ServiceIsUnavailable = 503,
    RequestTimedOut = 504,

//6xx
    GameProviderDoesntProvideFreeSpins = 600,
    ImpossibleToIssueFreeSpinsInRequestedGame = 601,

    YouShouldProvideAtLeastOneGameToIssueFreeSpins = 602,
    BadExpirationDate = 603,
    CantChangeIssueStateFromItsCurrentToRequested = 605,
    YouCantChangeIssueStateWhenIssueStatusIsNotSynced = 606,
    CantIssueFreeSpinsForDifferentGameProviders = 607,
    InvalidFreeSpinsIssue = 610,
    FreeSpinsIssueHasAlreadyExpired = 611,
    FreeSpinsIssueCantBeCanceled = 620,

//7xx
    RequestedLiveGameIsNotAvailableRightNow = 700,
}