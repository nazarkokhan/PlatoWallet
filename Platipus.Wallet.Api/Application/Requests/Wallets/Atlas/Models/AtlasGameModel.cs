﻿namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas.Models;

public sealed record AtlasGameModel(
    string GameId, 
    string GameName, 
    string GameTypeId, 
    bool DemoGameAvailable, 
    bool JackpotAvailable, 
    bool FreeSpinAvailable,
    bool IsDesktop, 
    bool IsMobile);