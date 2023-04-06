namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.PariMatch.Base;

public record PariMatchPlayerInfoResponse(
    string PlayerId,
    int Balance,
    string Currency,
    string Country,
    string DisplayName);