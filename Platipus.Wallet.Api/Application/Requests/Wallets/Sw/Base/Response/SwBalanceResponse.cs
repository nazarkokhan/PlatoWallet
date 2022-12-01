namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw.Base.Response;

public record SwBalanceResponse(int UserId, decimal Balance, string Currency);