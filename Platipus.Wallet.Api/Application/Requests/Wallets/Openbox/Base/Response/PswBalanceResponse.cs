namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox.Base.Response;

public record OpenboxBalanceResponse(long Balance);

public record OpenboxTokenResponse(Guid Token);