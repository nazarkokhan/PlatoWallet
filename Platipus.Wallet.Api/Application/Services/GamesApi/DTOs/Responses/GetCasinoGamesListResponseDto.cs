namespace Platipus.Wallet.Api.Application.Services.GamesApi.DTOs.Responses;

using Requests.Wallets.Psw.Base.Response;

public record GetCasinoGamesListResponseDto(List<GetGameDto> Data) : PswBaseResponse;