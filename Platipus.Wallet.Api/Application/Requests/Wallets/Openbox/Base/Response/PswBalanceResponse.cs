namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox.Base.Response;

public record OpenboxBalanceResponse(decimal Balance);

public record OpenboxTokenResponse(Guid Token);