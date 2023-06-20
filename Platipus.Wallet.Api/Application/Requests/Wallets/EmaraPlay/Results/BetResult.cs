namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;

using Base;

public sealed record BetResult(
    string Currency, decimal Balance, string Transaction, string TxId, 
    string? Bonus = null, string? Promo = null) : IEmaraPlayBaseResponse;