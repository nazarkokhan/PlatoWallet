namespace Platipus.Wallet.Api.Application.Results.EmaraPlay;

public enum EmaraPlayErrorCode
{
    Success = 0,
    InsufficientBalance = 1,
    PlayerNotFound = 2,
    BetIsNotAllowed = 3,
    PlayerAuthenticationFailed = 4,
    InvalidHashCode = 5,
    PlayerIsFrozen = 6,
    BadParameters = 7,
    GameIsNotFoundOrDisabled = 8,
    BetLimitHasBeenReached = 10,
    DuplicatedTransaction = 11,
    InvalidTransaction = 12,
    BetRoundAlreadyStarted = 13,
    TransactionNotFound = 14,
    BetRoundAlreadyClosed = 15,
    RoundNotFound = 16,
    InvalidUserCurrency = 20,
    ProviderNotFound = 30,
    InternalServerError = 100,
    RealityCheckWarning = 210,
    UnauthorizedAccess = 401,
}