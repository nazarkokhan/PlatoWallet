namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Data;

public sealed record UranusBalanceData(string? Currency, decimal Balance) 
    : UranusCommonData(Currency, Balance);