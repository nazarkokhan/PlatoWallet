namespace Platipus.Wallet.Api.Application.Services.PswGamesApi.Requests;

public record PswFreebetAwardGameApiRequest(
    string CasinoId,
    string User,
    string AwardId,
    string Currency,
    string[] Games,
    DateTime ValidUntil,
    int Count) : IPswGameApiBaseRequest;