namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Requests;

using PswGamesApi.DTOs.Requests;

public record SoftswissRoundDetailsGameApiRequest(
    string CasinoId,
    string RoundId,
    string RoundType,
    string Provider) : IPswGamesApiBaseRequest;