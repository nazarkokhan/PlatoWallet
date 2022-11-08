namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet.Base.Response;

public record DatabetBalanceResponse(
    string PlayerId,
    string Currency,
    decimal Balance) : DatabetBaseResponse;