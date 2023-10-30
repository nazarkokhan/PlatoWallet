namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Requests;

public record SoftswissRoundDetailsGameApiResponse(
    string ProviderIdentifier,
    string CasinoIdentifier,
    string UserId,
    bool? Finished);