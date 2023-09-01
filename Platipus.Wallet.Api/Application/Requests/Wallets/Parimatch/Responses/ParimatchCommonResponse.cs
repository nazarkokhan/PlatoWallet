namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch.Responses;

public record ParimatchCommonResponse(
    string PlayerId,
    long Balance,
    string Currency,
    string DisplayName,
    string Country = "UA");