namespace Platipus.Wallet.Api.Application.Services.Wallet;

using DTOs;

public interface IWalletService
{
    Task<IResult<WalletGetBalanceResponse>> GetBalanceAsync(
        string sessionId,
        bool searchByUsername = false,
        CancellationToken cancellationToken = default);

    Task<IResult<WalletBetWinRollbackResponse>> BetAsync(
        string sessionId,
        string roundId,
        string transactionId,
        decimal amount,
        string? currency = null,
        bool roundFinished = false,
        bool searchByUsername = false,
        string? provider = null,
        CancellationToken cancellationToken = default);

    Task<IResult<WalletBetWinRollbackResponse>> WinAsync(
        string sessionId,
        string roundId,
        string transactionId,
        decimal amount,
        bool roundFinished = true,
        string? currency = null,
        bool searchByUsername = false,
        CancellationToken cancellationToken = default);

    Task<IResult<WalletBetWinRollbackResponse>> RollbackAsync(
        string sessionId,
        string transactionId,
        string? roundId = null,
        bool searchByUsername = false,
        CancellationToken cancellationToken = default);

    Task<IResult<WalletGetBalanceResponse>> AwardAsync(
        string sessionId,
        string roundId,
        string transactionId,
        decimal amount,
        string awardId,
        string? currency = null,
        bool searchByUsername = false,
        CancellationToken cancellationToken = default);
}