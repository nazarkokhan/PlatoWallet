namespace Platipus.Wallet.Api.Application.Services.Wallet;

using Domain.Entities;
using Domain.Entities.Enums;
using DTOs;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public sealed class WalletService : IWalletService
{
    private readonly ILogger<WalletService> _logger;
    private readonly WalletDbContext _context;

    public WalletService(ILogger<WalletService> logger, WalletDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IResult<WalletGetBalanceResponse>> GetBalanceAsync(
        string sessionId,
        bool searchByUsername = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Set<User>()
                .TagWith("GetBalance")
                .Where(
                    u => searchByUsername
                        ? u.Username == sessionId
                        : u.Sessions.Any(s => s.Id == sessionId))
                .Select(
                    u => new WalletGetBalanceResponse(
                        u.Id,
                        u.Username,
                        u.Balance,
                        u.CurrencyId,
                        u.CasinoId,
                        u.IsDisabled))
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<WalletGetBalanceResponse>(ErrorCode.UserNotFound);

            return user.IsDisabled ? 
                ResultFactory.Failure<WalletGetBalanceResponse>(ErrorCode.UserIsDisabled) : 
                ResultFactory.Success(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Common wallet service GetBalance unknown exception");
            return ResultFactory.Failure<WalletGetBalanceResponse>(ErrorCode.UnknownGetBalanceException, e);
        }
    }

    public async Task<IResult<WalletBetWinRollbackResponse>> BetAsync(
        string sessionId,
        string roundId,
        string transactionId,
        decimal amount,
        string? currency = null,
        bool roundFinished = false,
        bool searchByUsername = false,
        string? provider = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // await using var dbTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var transactionAlreadyExists = await _context.Set<Transaction>()
                .TagWith("Bet")
                .Where(t => t.Id == transactionId)
                .AnyAsync(cancellationToken);

            if (transactionAlreadyExists)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.TransactionAlreadyExists);

            var user = await _context.Set<User>()
                .TagWith("Bet")
                .Where(
                    u => searchByUsername
                        ? u.Username == sessionId
                        : u.Sessions.Any(s => s.Id == sessionId))
                .Include(u => u.Rounds.Where(r => r.Id == roundId))
                .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.UserNotFound);

            if (user.IsDisabled)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.UserIsDisabled);

            var round = user.Rounds.FirstOrDefault();

            if (round is null)
            {
                var roundAlreadyExists = await _context.Set<Round>()
                    .TagWith("Bet")
                    .Where(t => t.Id == roundId)
                    .AnyAsync(cancellationToken);

                if (roundAlreadyExists)
                    return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.RoundAlreadyExists);

                round = new Round(roundId) { UserId = user.Id };
                _context.Add(round);
            }

            if (round.Finished)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.RoundAlreadyFinished);

            if (currency is not null && currency != user.CurrencyId)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.InvalidCurrency);

            user.Balance -= amount;
            if (roundFinished)
            {
                round.Finish();
                _context.Update(round);
            }

            var transaction = new Transaction(transactionId, amount, TransactionType.Bet) { RoundId = round.Id };

            _context.Add(transaction);

            await _context.SaveChangesAsync(cancellationToken);

            // await dbTransaction.CommitAsync(cancellationToken);

            var response = new WalletBetWinRollbackResponse(
                user.Id,
                user.Username,
                user.Balance,
                user.CurrencyId,
                new WalletBetWinRollbackResponse.TransactionDto(transaction.Id, transaction.InternalId, transaction.CreatedDate),
                new WalletBetWinRollbackResponse.RoundDto(round.Id, round.InternalId, round.CreatedDate));

            return ResultFactory.Success(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Common wallet service Bet unknown exception");
            return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.UnknownBetException, e);
        }
    }

    public async Task<IResult<WalletBetWinRollbackResponse>> WinAsync(
        string sessionId,
        string roundId,
        string transactionId,
        decimal amount,
        bool roundFinished = true,
        string? currency = null,
        bool searchByUsername = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // var dbTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var transactionAlreadyExists = await _context.Set<Transaction>()
                .TagWith("Win")
                .Where(t => t.Id == transactionId)
                .AnyAsync(cancellationToken);

            if (transactionAlreadyExists)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.TransactionAlreadyExists);

            var user = await _context.Set<User>()
                .TagWith("Win")
                .Where(
                    u => searchByUsername
                        ? u.Username == sessionId
                        : u.Sessions.Any(s => s.Id == sessionId))
                .Include(u => u.Rounds.Where(r => r.Id == roundId))
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.UserNotFound);

            if (user.IsDisabled)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.UserIsDisabled);

            var round = user.Rounds.FirstOrDefault();
            if (round is null)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.RoundNotFound);

            if (round.Finished)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.RoundAlreadyFinished);

            if (currency is not null && currency != user.CurrencyId)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.InvalidCurrency);

            user.Balance += amount;

            var transaction = new Transaction(transactionId, amount, TransactionType.Win) { RoundId = round.Id };

            _context.Add(transaction);
            round.Transactions.Add(transaction);

            await _context.SaveChangesAsync(cancellationToken);

            // await dbTransaction.CommitAsync(cancellationToken);

            var response = new WalletBetWinRollbackResponse(
                user.Id,
                user.Username,
                user.Balance,
                user.CurrencyId,
                new WalletBetWinRollbackResponse.TransactionDto(transaction.Id, transaction.InternalId, transaction.CreatedDate),
                new WalletBetWinRollbackResponse.RoundDto(round.Id, round.InternalId, round.CreatedDate));

            return ResultFactory.Success(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Common wallet service Win unknown exception");
            return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.UnknownWinException, e);
        }
    }

    public async Task<IResult<WalletBetWinRollbackResponse>> RollbackAsync(
        string sessionId,
        string transactionId,
        string? roundId = null,
        bool searchByUsername = false,
        decimal? amount = null,
        string? clientId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // var dbTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var user = await _context.Set<User>()
                .TagWith("Rollback")
                .Where(
                    u => searchByUsername
                        ? u.Username == sessionId
                        : u.Sessions.Any(s => s.Id == sessionId))
                .Include(
                    u => u.Rounds.Where(
                        r => roundId != null
                            ? r.Id == roundId
                            : r.Transactions.Any(t => t.Id == transactionId)))
                .ThenInclude(r => r.Transactions.Where(t => t.Id == transactionId))
                .AsSingleQuery()
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.UserNotFound);

            if (user.IsDisabled)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.UserIsDisabled);

            var round = user.Rounds.FirstOrDefault();
            if (round is null)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.RoundNotFound);

            if (round.Finished)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.RoundAlreadyFinished);

            var transaction = round.Transactions.FirstOrDefault();
            if (transaction is null || transaction.IsCanceled)
                return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.TransactionNotFound);

            transaction.Cancel();
            _context.Update(transaction);

            if (amount is null)
            {
                user.Balance += transaction.Amount;
            }
            else
            {
                user.Balance += (decimal)amount;
            }
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            // await dbTransaction.CommitAsync(cancellationToken);

            var response = new WalletBetWinRollbackResponse(
                user.Id,
                user.Username,
                user.Balance,
                user.CurrencyId,
                new WalletBetWinRollbackResponse.TransactionDto(transaction.Id, transaction.InternalId, transaction.CreatedDate),
                new WalletBetWinRollbackResponse.RoundDto(round.Id, round.InternalId, round.CreatedDate));

            return ResultFactory.Success(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Common wallet service Rollback unknown exception");
            return ResultFactory.Failure<WalletBetWinRollbackResponse>(ErrorCode.UnknownRollbackException, e);
        }
    }

    public async Task<IResult<WalletGetBalanceResponse>> AwardAsync(
        string sessionId,
        string roundId,
        string transactionId,
        decimal amount,
        string awardId,
        string? currency = null,
        bool searchByUsername = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // var dbTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var transactionAlreadyExists = await _context.Set<Transaction>()
                .TagWith("Award")
                .Where(t => t.Id == transactionId)
                .AnyAsync(cancellationToken);

            if (transactionAlreadyExists)
                return ResultFactory.Failure<WalletGetBalanceResponse>(ErrorCode.TransactionAlreadyExists);

            var roundAlreadyExists = await _context.Set<Round>()
                .TagWith("Award")
                .Where(t => t.Id == roundId)
                .AnyAsync(cancellationToken);

            if (roundAlreadyExists)
                return ResultFactory.Failure<WalletGetBalanceResponse>(ErrorCode.RoundAlreadyExists);

            var user = await _context.Set<User>()
                .TagWith("Award")
                .Where(
                    u => searchByUsername
                        ? u.Username == sessionId
                        : u.Sessions.Any(s => s.Id == sessionId))
                .Include(u => u.Awards.Where(r => r.Id == awardId))
                .ThenInclude(r => r.AwardRound)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<WalletGetBalanceResponse>(ErrorCode.UserNotFound);

            if (user.IsDisabled)
                return ResultFactory.Failure<WalletGetBalanceResponse>(ErrorCode.UserIsDisabled);

            var award = user.Awards.FirstOrDefault();
            if (award is null)
                return ResultFactory.Failure<WalletGetBalanceResponse>(ErrorCode.AwardNotFound);
            if (award.ValidUntil < DateTime.UtcNow)
                return ResultFactory.Failure<WalletGetBalanceResponse>(ErrorCode.AwardExpired);
            if (award.AwardRound is not null)
                return ResultFactory.Failure<WalletGetBalanceResponse>(ErrorCode.AwardIsAlreadyUsed);

            var transaction = new Transaction(transactionId, amount, TransactionType.Award);
            _context.Add(transaction);

            var awardRound = new AwardRound { AwardId = awardId };
            _context.Add(awardRound);

            var round = new Round(roundId)
            {
                Finished = true,
                Transactions = new List<Transaction> { transaction },
                AwardRound = awardRound,
                UserId = user.Id
            };
            _context.Add(round);

            user.Balance += amount;
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);
            // await dbTransaction.CommitAsync(cancellationToken);

            var response = new WalletGetBalanceResponse(
                user.Id,
                user.Username,
                user.Balance,
                user.CurrencyId,
                user.CasinoId,
                user.IsDisabled);

            return ResultFactory.Success(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Common wallet service Award unknown exception");
            return ResultFactory.Failure<WalletGetBalanceResponse>(ErrorCode.UnknownAwardException, e);
        }
    }

    public async Task<IResult<WalletGetEnvironmentResponse>> GetEnvironmentAsync(
        string environment, CancellationToken cancellationToken = default)
    {
        try
        {
            var contextResponse = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == environment)
                .FirstOrDefaultAsync(cancellationToken);

            if (contextResponse is null)
                return ResultFactory.Failure<WalletGetEnvironmentResponse>(
                    ErrorCode.EnvironmentNotFound);
            
            var finalResponse = new WalletGetEnvironmentResponse(contextResponse.BaseUrl);

            return ResultFactory.Success(finalResponse);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Common wallet service Environment unknown exception");
            return ResultFactory.Failure<WalletGetEnvironmentResponse>(ErrorCode.UnknownEnvironmentException);
        }
    }
}