namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base.Response;

public record BetconstructPlayResponse(
    string TransactionId,
    decimal Balance) : BetconstructBaseResponse;