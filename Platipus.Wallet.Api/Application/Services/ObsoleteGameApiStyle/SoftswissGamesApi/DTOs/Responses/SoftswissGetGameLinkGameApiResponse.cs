namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.SoftswissGamesApi.DTOs.Responses;

public record SoftswissGetGameLinkGameApiResponse(
    SoftswissLaunchOptions LaunchOptions,
    string SessionId);