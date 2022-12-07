namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet.Base.Response;

public record DafabetBalanceResponse(
    string PlayerId,
    string Currency,
    decimal Balance) : DafabetBaseResponse;