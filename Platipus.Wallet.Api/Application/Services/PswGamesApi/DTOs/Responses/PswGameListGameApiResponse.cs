namespace Platipus.Wallet.Api.Application.Services.PswGamesApi.DTOs.Responses;

using Application.Requests.Wallets.Psw.Base.Response;

public record PswGameListGameApiResponse(List<PswGameListGameApiResponse.GameItemDto> Data) : PswBaseResponse
{
    public record GameItemDto(
        string Id,
        string GameId,
        string Name,
        string Category);
}