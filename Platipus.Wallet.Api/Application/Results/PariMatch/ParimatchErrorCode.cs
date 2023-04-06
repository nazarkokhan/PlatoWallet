namespace Platipus.Wallet.Api.Application.Results.PariMatch;

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