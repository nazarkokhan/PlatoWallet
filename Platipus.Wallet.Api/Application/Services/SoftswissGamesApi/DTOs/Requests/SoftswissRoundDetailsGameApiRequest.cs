namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Requests;

public record SoftswissRoundDetailsGameApiRequest(
    string CasinoId,
    string RoundId,
    string RoundType,
    string Provider) : ISoftswissGameApiBaseRequest;