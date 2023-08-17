namespace Platipus.Wallet.Api.Application.Services.PswGamesApi.DTOs.Responses;

public record PswErrorGameApiResponse(
    string Status,
    int Error,
    string Description);