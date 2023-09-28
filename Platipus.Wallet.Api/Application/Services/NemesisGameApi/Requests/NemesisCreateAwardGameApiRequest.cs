namespace Platipus.Wallet.Api.Application.Services.NemesisGameApi.Requests;

using JetBrains.Annotations;

[PublicAPI]
public record NemesisCreateAwardGameApiRequest(
    string PartnerId,
    string UserId,
    string Currency,
    NemesisCreateAwardGameApiRequest.GameItem[] Games,
    string BonusCode,
    int RoundsCount,
    long? ExpirationTimestamp,
    long StartTimestamp)
{
    public record GameItem(string Id);
}