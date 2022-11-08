namespace Platipus.Wallet.Api.Application.DTOs;

public record CachedSessionDto(
    Guid SessionId,
    DateTime ExpirationDate,
    bool UserIsDisabled,
    string CasinoSignatureKey);