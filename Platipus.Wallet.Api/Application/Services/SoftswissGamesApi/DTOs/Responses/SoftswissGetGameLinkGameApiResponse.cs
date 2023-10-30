namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Responses;

public sealed record SoftswissGetGameLinkGameApiResponse(
    SoftswissLaunchOptions LaunchOptions,
    string SessionId);