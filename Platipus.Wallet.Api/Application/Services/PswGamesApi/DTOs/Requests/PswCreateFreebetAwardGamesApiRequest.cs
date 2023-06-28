namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Requests;

public record PswCreateFreebetAwardGamesApiRequest(
    string CasinoId,
    string User,
    string AwardId,
    string Currency,
    string[] Games,
    DateTime ValidUntil,
    int Count) : IPswGamesApiBaseRequest;