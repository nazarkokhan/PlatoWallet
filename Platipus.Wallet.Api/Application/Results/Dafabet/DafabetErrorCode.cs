namespace Platipus.Wallet.Api.Application.Results.Dafabet;

public enum DafabetErrorCode
{
    Success = 0,
    SystemError = 100,
    InvalidHash = 101,
    MissingRequiredParameters = 102,
    InvalidToken = 103,
    InvalidGameCode = 104,
    PlayerNotFound = 105,
    RoundNotFound = 106,
    TransactionNotFound = 107,
    InsufficientFunds = 108,
}