namespace Platipus.Wallet.Api.Application.Results.Common;

public enum ErrorCode
{
    Unknown = 1,

    BadParametersInTheRequest,

    CasinoNotFound,
    CasinoAlreadyExists,
    ThisProviderSupportOnlyOneCasino,

    SessionNotFound,
    SessionExpired,

    SecurityParameterIsInvalid,
    SecurityParameterIsEmpty,

    UserNotFound,
    UserIsDisabled,
    InvalidCurrency,
    TransactionAlreadyExists,
    TransactionNotFound,

    RoundAlreadyExists,
    RoundNotFound,
    RoundAlreadyFinished,

    InsufficientFunds,

    AwardNotFound,
    AwardIsAlreadyUsed,

    RequestAlreadyExists,

    UnknownGetBalanceException,
    UnknownBetException,
    UnknownWinException,
    UnknownRollbackException,
    UnknownAwardException,
    UnknownAuthorizeException,
    UnknownLogoutException,

    GameNotFound,

    ErrorMockApplianceError,


    //external
    InvalidPassword,
    GameServerApiError,
    EnvironmentDoesNotExists,
    EnvironmentAlreadyExists,
    UserAlreadyExists,

    ErrorMockAlreadyExists,
    InvalidJsonContent,
    InvalidXmlContent,

    //game api
    EmptyResponse,
    InvalidResponse
}