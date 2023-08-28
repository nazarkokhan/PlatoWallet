namespace Platipus.Wallet.Api.Application.Services.PswGameApi.Responses;

public record PswErrorGameApiResponse(
    string Status,
    int Error,
    string Description);