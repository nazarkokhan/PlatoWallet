namespace Platipus.Wallet.Api.Application.Services.PswGamesApi.Responses;

public record PswErrorGameApiResponse(
    string Status,
    int Error,
    string Description);