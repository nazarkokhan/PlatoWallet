namespace Platipus.Wallet.Api.Application.Services.NemesisGamesApi.Responses;

using JetBrains.Annotations;

[PublicAPI]
public record NemesisErrorGameApiResponse(string Error, string Description);