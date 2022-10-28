namespace Platipus.Wallet.Api.Application.Services.GamesApi.DTOs.Responses;

using Requests.Base.Responses;

public record GetCasinoGamesListResponseDto(List<GetGameDto> Data) : BaseResponse;