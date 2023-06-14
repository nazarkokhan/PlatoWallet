using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;

public sealed record EmaraPlayGetLauncherUrlResponse(
    int Error, string Description, LauncherResult Result);