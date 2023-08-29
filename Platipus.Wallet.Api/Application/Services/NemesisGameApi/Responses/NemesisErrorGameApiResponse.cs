namespace Platipus.Wallet.Api.Application.Services.NemesisGameApi.Responses;

using JetBrains.Annotations;

[PublicAPI]
public record NemesisErrorGameApiResponse(string Error, string Description);