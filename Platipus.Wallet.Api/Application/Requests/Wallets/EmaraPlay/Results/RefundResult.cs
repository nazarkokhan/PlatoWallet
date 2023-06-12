namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;

public sealed record RefundResult(
    string Currency, string Balance, string Transaction, 
    string TxId, string? Bonus = null, string? Promo = null);