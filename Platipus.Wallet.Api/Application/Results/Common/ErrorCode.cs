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
    SessionAlreadyExists,
    SessionAlreadyNotTemporary,

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

    AwardDoesNotBelongToThisUser,
    AwardNotFound,
    AwardIsAlreadyUsed,
    AwardExpired,
    AwardAlreadyExists,
    AwardInvalidExpirationDate,

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

    ValidationError,

    //external
    InvalidPassword,
    GameServerApiError,
    EnvironmentNotFound,
    EnvironmentAlreadyExists,
    UserAlreadyExists,

    ErrorMockAlreadyExists,
    InvalidJsonContent,
    InvalidXmlContent,
    MaxErrorMockTimeoutIs3Minutes,

    //game api
    UnknownHttpClientError,
    EmptyExternalResponse,
    InvalidExternalResponse,
    
    
    UnknownEnvironmentException,
    InvalidHttpStatusCode
}