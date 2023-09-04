namespace Platipus.Wallet.Api.Application.Results.Parimatch;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum ParimatchErrorCode
{
    InvalidSessionKey,
    InsufficientBalance,
    LockedPlayer,
    InvalidCasinoLogic,
    InvalidTransactionId,
    RequestTimeout,
    ErrorInternal,
    IntegrationHubFailure,
}