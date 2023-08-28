namespace Platipus.Wallet.Api.Application.Services.NemesisGamesApi.Requests;

using JetBrains.Annotations;

[PublicAPI]
public record NemesisCancelAwardGameApiRequest(
    string PartnerId,
    string UserId,
    string Currency,
    string BonusCode);