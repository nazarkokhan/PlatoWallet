namespace Platipus.Wallet.Api.Application.Requests.Wallets.PariMatch.Base;

public record ParimatchErrorResponse(string Code, string Message, DateTimeOffset At);