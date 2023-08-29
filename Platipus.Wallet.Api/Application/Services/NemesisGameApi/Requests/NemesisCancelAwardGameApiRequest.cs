namespace Platipus.Wallet.Api.Application.Services.NemesisGameApi.Requests;

using JetBrains.Annotations;

[PublicAPI]
public record NemesisCancelAwardGameApiRequest(
    string PartnerId,
    string UserId,
    string Currency,
    string BonusCode);