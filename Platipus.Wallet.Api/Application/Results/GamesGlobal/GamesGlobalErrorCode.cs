namespace Platipus.Wallet.Api.Application.Results.GamesGlobal;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum GamesGlobalErrorCode
{
    UnknownServerError,
    PacketFormatError,
    GamingDatabaseError,
    ConfigDatabaseError,
    MissingServer,
    Configuration,
    InvalidAPICredentials,
    FailureToUpdateExternalBalance,
    ExternalBalanceError,
    GamingDatabaseTimeout,
    CacheError,
}