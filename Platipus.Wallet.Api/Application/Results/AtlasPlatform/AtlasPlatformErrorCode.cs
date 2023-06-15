namespace Platipus.Wallet.Api.Application.Results.AtlasPlatform;

public enum AtlasPlatformErrorCode
{
    HashValidationFailed = 4001,
    MethodNotAllowed = 4003,
    RequiredHeaderHashNotPresent = 4005,
    InvalidHashFormat = 4006,
    InternalError = 5001,
    SessionValidationFailed = 7001,
    ProviderNotConfigured = 7002,
    CurrencyMismatchException = 7003,
    GameRoundNotPreviouslyCreated = 7004,
    TransactionAlreadyProcessed = 7005,
    BetTransactionNotFound = 7006,
    InsufficientFunds = 7007,
    TransactionAlreadyRefunded = 7008,
    GameRoundIdNotUnique = 7009,
    GameLaunchError = 7015,
    RegisterFreeSpinsError = 7016,
    AssignFreeSpinsError = 7017
}