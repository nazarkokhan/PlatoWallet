namespace Platipus.Wallet.Api.Application.Services.PswGamesApi.DTOs.Responses;

using Requests.Wallets.Psw.Base.Response;

public record GetLaunchUrlResponseDto(string LaunchUrl) : PswBaseResponse;