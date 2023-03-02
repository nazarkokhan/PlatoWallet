namespace Platipus.Wallet.Api.Application.Services.Wallet.DTOs;

public record WalletBetWinRollbackResponse(
    int UserId,
    string Username,
    decimal Balance,
    string Currency,
    WalletBetWinRollbackResponse.TransactionDto Transaction,
    WalletBetWinRollbackResponse.RoundDto Round)
{
    public record TransactionDto(string Id, string InternalId, DateTime CreatedDate);

    public record RoundDto(string Id, string InternalId, DateTime CreatedDate);
}