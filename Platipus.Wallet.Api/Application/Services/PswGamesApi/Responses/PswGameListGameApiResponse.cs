namespace Platipus.Wallet.Api.Application.Services.PswGamesApi.Responses;

using Platipus.Wallet.Api.Application.Requests.Wallets.Psw.Base.Response;

public record PswGameListGameApiResponse(List<PswGameListGameApiResponse.GameItemDto> Data) : PswBaseResponse
{
    public record GameItemDto(
        string Id,
        string GameId,
        string Name,
        string Category);
}