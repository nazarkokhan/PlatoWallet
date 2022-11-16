namespace Platipus.Wallet.Api.Application.DTOs;

public record CachedSessionDto(
    Guid SessionId,
    DateTime ExpirationDate,
    Guid UserId,
    bool UserIsDisabled,
    string CasinoSignatureKey);