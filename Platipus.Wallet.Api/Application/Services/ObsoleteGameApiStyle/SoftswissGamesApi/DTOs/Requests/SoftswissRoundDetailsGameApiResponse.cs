namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.SoftswissGamesApi.DTOs.Requests;

public record SoftswissRoundDetailsGameApiResponse(
    string ProviderIdentifier,
    string CasinoIdentifier,
    string UserId,
    bool? Finished);