namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.PariMatch.Base;

public record ParimatchErrorResponse(string Code, string Message, DateTimeOffset At);