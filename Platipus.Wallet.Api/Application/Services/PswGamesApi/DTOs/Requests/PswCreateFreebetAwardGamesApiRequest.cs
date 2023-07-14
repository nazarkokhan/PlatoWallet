namespace Platipus.Wallet.Api.Application.Services.PswGamesApi.DTOs.Requests;

public record PswCreateFreebetAwardGamesApiRequest(
    string CasinoId,
    string User,
    string AwardId,
    string Currency,
    string[] Games,
    DateTime ValidUntil,
    int Count) : IPswGamesApiBaseRequest;