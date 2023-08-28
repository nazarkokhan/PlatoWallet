namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.SoftswissGamesApi.DTOs.Requests;

public record SoftswissRoundDetailsGameApiRequest(
    string CasinoId,
    string RoundId,
    string RoundType,
    string Provider) : ISoftswissGameApiBaseRequest;