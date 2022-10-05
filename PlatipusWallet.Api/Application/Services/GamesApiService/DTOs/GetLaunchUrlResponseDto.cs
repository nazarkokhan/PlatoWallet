namespace PlatipusWallet.Api.Application.Services.GamesApiService.DTOs;

using Requests.Base.Responses;

public record GetLaunchUrlResponseDto(string LaunchUrl) : BaseResponse;