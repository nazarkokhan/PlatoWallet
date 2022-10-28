namespace Platipus.Wallet.Api.StartupSettings.Middlewares.DTOs;

public record CachedSessionDto(
    Guid SessionId,
    DateTime ExpirationDate,
    bool UserIsDisabled,
    string CasinoSignatureKey);