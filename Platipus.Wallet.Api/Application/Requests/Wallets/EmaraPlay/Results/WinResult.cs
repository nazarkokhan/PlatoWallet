namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;

public sealed record WinResult(
    string Currency, string Balance, string Transaction, 
    string TxId, string? Bonus = null, string? Promo = null);