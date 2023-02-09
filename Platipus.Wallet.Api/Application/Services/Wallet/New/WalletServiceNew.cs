namespace Platipus.Wallet.Api.Application.Services.Wallet.New;

using Platipus.Wallet.Api.Application.Services.Wallet.DTOs;

public interface IWalletServiceNew
{
    Task<IResult<BalanceResponse>> GetBalanceAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);

    Task<IResult<BetOrWinResponse>> BetAsync(
        Guid sessionId,
        string roundId,
        string transactionId,
        decimal amount,
        bool? finished = null,
        string? currency = null,
        CancellationToken cancellationToken = default);

    Task<IResult<BetOrWinResponse>> WinAsync(
        Guid sessionId,
        string roundId,
        string transactionId,
        decimal amount,
        bool? finished = null,
        string? currency = null,
        CancellationToken cancellationToken = default);

    Task<IResult<BetOrWinResponse>> RollbackAsync(
        Guid sessionId,
        string roundId,
        string transactionId,
        CancellationToken cancellationToken = default);

    Task<IResult<BalanceResponse>> AwardAsync(
        Guid sessionId,
        string roundId,
        string transactionId,
        decimal amount,
        string awardId,
        string? currency = null,
        CancellationToken cancellationToken = default);
}

public class WalletServiceNew : IWalletServiceNew
{
    public Task<IResult<BalanceResponse>> GetBalanceAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return null;
    }

    public Task<IResult<BetOrWinResponse>> BetAsync(
        Guid sessionId,
        string roundId,
        string transactionId,
        decimal amount,
        bool? finished = null,
        string? currency = null,
        CancellationToken cancellationToken = default)
    {
        return null;
    }

    public Task<IResult<BetOrWinResponse>> WinAsync(
        Guid sessionId,
        string roundId,
        string transactionId,
        decimal amount,
        bool? finished = null,
        string? currency = null,
        CancellationToken cancellationToken = default)
    {
        return null;
    }

    public Task<IResult<BetOrWinResponse>> RollbackAsync(
        Guid sessionId,
        string roundId,
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        return null;
    }

    public Task<IResult<BalanceResponse>> AwardAsync(
        Guid sessionId,
        string roundId,
        string transactionId,
        decimal amount,
        string awardId,
        string? currency = null,
        CancellationToken cancellationToken = default)
    {
        return null;
    }
}