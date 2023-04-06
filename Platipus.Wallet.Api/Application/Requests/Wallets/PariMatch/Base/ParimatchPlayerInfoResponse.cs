namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.PariMatch.Base;

public record ParimatchPlayerInfoResponse(
    string PlayerId,
    int Balance,
    string Currency,
    string Country,
    string DisplayName);