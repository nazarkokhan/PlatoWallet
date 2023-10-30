namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.SoftswissGamesApi.DTOs.Responses;

public sealed record SoftswissGetGameLinkGameApiResponse(
    SoftswissLaunchOptions LaunchOptions,
    string SessionId);