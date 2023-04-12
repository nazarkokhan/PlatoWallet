namespace Platipus.Wallet.Api.Application.Requests.Wallets.PariMatch.Base;

public record ParimatchPlayerInfoResponse(
    string PlayerId,
    int Balance,
    string Currency,
    string Country,
    string DisplayName);