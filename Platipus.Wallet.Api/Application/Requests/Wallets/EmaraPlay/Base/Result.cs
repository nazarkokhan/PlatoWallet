namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

public record Result(
    string? User = null,
    string? Username = null,
    string? Currency = null,
    string? Balance = null,
    string? Bonus = null,
    string? Country = null,
    string? Jurisdiction = null);