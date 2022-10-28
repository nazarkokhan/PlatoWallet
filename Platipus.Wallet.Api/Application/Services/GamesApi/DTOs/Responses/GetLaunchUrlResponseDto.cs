namespace Platipus.Wallet.Api.Application.Services.GamesApi.DTOs.Responses;

using Requests.Base.Responses;

public record GetLaunchUrlResponseDto(string LaunchUrl) : BaseResponse;