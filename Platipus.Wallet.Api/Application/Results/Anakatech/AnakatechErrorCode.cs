namespace Platipus.Wallet.Api.Application.Results.Anakatech;

using System.ComponentModel;

public enum AnakatechErrorCode
{
    [Description("GA_501")]
    InternalError,

    [Description("GA_502")]
    InvalidArgument,

    [Description("GA_503")]
    InvalidSecret,

    [Description("GA_504")]
    UntrustedApi,

    [Description("GA_601")]
    InvalidPlayerIdOrSessionId,

    [Description("GA_602")]
    InsufficientBalance,

    [Description("GA_603")]
    PlayerLimitsExceeded,

    [Description("GA_604")]
    CurrencyNotPermitted,

    [Description("GA_605")]
    RoundDoesNotExist,

    [Description("GA_606")]
    RoundWasAlreadyClosed,
}