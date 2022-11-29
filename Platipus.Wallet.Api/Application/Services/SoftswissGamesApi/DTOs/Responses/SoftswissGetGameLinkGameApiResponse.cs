namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Responses;

public record SoftswissGetGameLinkGameApiResponse(
    SoftswissLaunchOptions LaunchOptions,
    string SessionId);