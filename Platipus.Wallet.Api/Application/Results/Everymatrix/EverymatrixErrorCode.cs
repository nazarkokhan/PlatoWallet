namespace Platipus.Wallet.Api.Application.Results.Everymatrix;

public enum EverymatrixErrorCode
{
    UnknownError = 101,
    TokenNotFound = 102,
    UserIsBlocked = 103,
    InsufficientFunds = 105,
    VendorAccountNotActive = 106,
    IpIsNotAllowed = 107,
    CurrencyDoesntMatch = 108,
    TransactionNotFound = 109,
    DoubleTransaction = 110,
    InvalidHash = 111,
    CasinoLossLimit = 112
}