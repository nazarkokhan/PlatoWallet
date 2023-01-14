namespace Platipus.Wallet.Api.Application.Results.BetConstruct;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum BetConstructErrorCode
{
    WrongBetAmount = 4,
    WrongGameID = 7,
    AuthenticationFailed = 8,
    NotEnoughMoney = 21,
    PlayerIsBlocked = 29,
    WrongCurrency = 34,
    WrongWinAmount = 63,
    GameIsBlocked = 84,
    TransactionIsAlreadyExist = 104,
    TheTransactionIsAlreadyCanceled = 105,
    TokenExpired = 106,
    TransactionNotFound = 107,
    TokenNotFound = 114,
    GeneralError = 130,
    IncorrectParametersPassed = 200
}