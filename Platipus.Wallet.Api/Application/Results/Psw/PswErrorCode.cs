namespace Platipus.Wallet.Api.Application.Results.Psw;

public enum PswErrorCode
{
    NotEnoughMoney = 1,
    UserDisabled = 2,
    SessionExpired = 3,
    InvalidSignature = 4,
    MissingSignature = 5,
    RoomIsWrongOrEmpty = 6,
    BadParametersInTheRequest = 7,
    CommandIsWrong = 8,
    EmptySessionId = 9,
    BetLimitReached = 10,
    InvalidUser = 11,
    InvalidCasinoId = 12,
    InvalidGame = 13,
    InvalidExpirationDate = 14,
    WrongCurrency = 15,
    DuplicateTransaction = 16,
    TransactionDoesNotExist = 17,
    DuplicateAward = 18,
    AwardIsAlreadyCanceled = 19,
    AwardDoesNotExist = 20,
    Unknown = 100,
    InvalidHash = 101,
    CouldNotTryToMockSessionError = 1000,
    Duplication = 1001
}