namespace Platipus.Wallet.Api.Application.Results.Uis;

public enum UisErrorCode
{
    InternalSystemError = -100,
    UserNotFound = -101,
    InvalidPartnered = -102,
    InvalidMd5OrHash = -103,
    InvalidIp = -104,
    InvalidAmount = -105,
    InsufficientBalance = -106,
    TransferLimit = -107,
    DuplicateRemoteTransactionId = -108,
    InsufficientBalance2 = -109,
    InvalidTransactionId = -110,
    TransactionAlreadyProcessed = -111,
    ExpiredToken = -112,

    InsufficientFunds = 300,
    OperationFailed = 301,
    UnknownTransactionIdOrWasAlreadyProcessed = 302,
    UnknownUserId = 310,
    InternalError = 399,
    InvalidToken = 400,
    InvalidHash = 500
}