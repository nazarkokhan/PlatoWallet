using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;

public sealed record EmaraPlayAuthenticateResult(
    string User,
    string Currency,
    string Username,
    decimal Balance,
    string? Bonus = null,
    string? Country = null,
    string? Jurisdiction = null) : IEmaraPlayBaseResponse;