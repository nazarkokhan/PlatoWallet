namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Requests;

public record PswGameBuyGamesApiRequest(
    string CasinoId,
    string User,
    string Currency,
    string Game,
    string BuyOption,
    string BetLevel) : IPswGamesApiBaseRequest;