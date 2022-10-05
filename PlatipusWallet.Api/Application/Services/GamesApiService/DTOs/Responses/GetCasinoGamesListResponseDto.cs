namespace PlatipusWallet.Api.Application.Services.GamesApiService.DTOs.Responses;

using PlatipusWallet.Api.Application.Requests.Base.Responses;

public record GetCasinoGamesListResponseDto(List<GetGameDto> Data) : BaseResponse;