namespace Platipus.Wallet.Api.Application.Services.PswGameApi.Requests;

public record PswGameBuyGameApiRequest(
    string CasinoId,
    string User,
    string Currency,
    string Game,
    string BuyOption,
    string BetLevel) : IPswGameApiBaseRequest;