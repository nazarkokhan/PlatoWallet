namespace Platipus.Wallet.Api.Application.Responses.AtlasPlatform;

using Requests.Wallets.Atlas.Base;

public sealed record AtlasCommonResponse(
    string Currency, 
    int Balance, 
    string ClientId) : IAtlasCommonResponse;