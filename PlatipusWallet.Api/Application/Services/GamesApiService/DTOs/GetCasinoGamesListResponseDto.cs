namespace PlatipusWallet.Api.Application.Services.GamesApiService.DTOs;

using Requests.Base.Responses;

public record GetCasinoGamesListResponseDto(List<GetGameDto> Data) : BaseResponse;