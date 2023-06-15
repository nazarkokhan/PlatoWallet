namespace Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Models;

public sealed record AtlasPlatformGameModel(
    string GameId, string GameName, string GameTypeId, 
    bool DemoGameAvailable, bool JackpotAvailable, bool IsFreeSpinAvailable,
    bool IsDesktop, bool IsMobile);