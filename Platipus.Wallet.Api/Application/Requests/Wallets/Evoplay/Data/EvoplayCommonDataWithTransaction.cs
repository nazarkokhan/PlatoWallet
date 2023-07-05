namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay.Data;

public sealed record EvoplayCommonDataWithTransaction(
    string? Currency, 
    decimal Balance,
    string ProcessedTransactionId) : EvoplayCommonData(Currency, Balance);