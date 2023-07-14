namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay.Data;

public sealed record UranusBalanceData(string? Currency, decimal Balance) 
    : EvoplayCommonData(Currency, Balance);