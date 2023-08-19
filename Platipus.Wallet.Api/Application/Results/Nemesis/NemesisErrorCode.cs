namespace Platipus.Wallet.Api.Application.Results.Nemesis;

public enum NemesisErrorCode
{
    InappropriateArgument,
    Internal,
    NotImplemented,
    SessionExpired,
    SessionNotFound,
    SessionNotActivated,
    SessionIsDeactivated,
    UserNotFound,
    UserMismatched,
    GameNotFound,
    GameMismatched,
    ProviderNotFound,
    ProductTypeNotFound,
    ProductTypeNotSupported,
    InsufficientFunds,
    TokenIsInvalid,
    RoundIsClosed,
    CurrencyMismatched,
    AmountMismatched,
    NegativeAmount,
    IpAddressUnknown,
    SessionMismatched,
    ApiDeprecated,
}