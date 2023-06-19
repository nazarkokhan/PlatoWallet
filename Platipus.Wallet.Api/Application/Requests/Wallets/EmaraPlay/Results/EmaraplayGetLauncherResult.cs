using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;

public sealed record EmaraplayGetLauncherResult(Uri Url) : IEmaraPlayBaseResponse;