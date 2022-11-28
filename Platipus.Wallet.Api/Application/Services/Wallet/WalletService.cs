namespace Platipus.Wallet.Api.Application.Services.Wallet;

using Domain.Entities;
using DTOs;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class WalletService : IWalletService
{
    private readonly WalletDbContext _context;

    public WalletService(WalletDbContext context)
    {
        _context = context;
    }

    public async Task<IResult<BalanceResponse>> GetBalanceAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .TagWith("GetUserBalance")
            .Where(
                u => u.UserName == request.User
                  && u.Sessions
                         .Select(s => s.Id)
                         .Contains(request.SessionId))
            .Select(
                u => new
                {
                    u.Id,
                    u.Balance,
                    u.IsDisabled,
                    Currency = u.Currency.Name
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.InvalidUser);

        if (user.IsDisabled)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.UserDisabled);

        var response = new BalanceResponse(user.Balance, user.Currency);

        return ResultFactory.Success(response);
    }

    public async Task<IResult<BetOrWinResponse>> BetAsync(
        BetRequest request,
        CancellationToken cancellationToken)
    {
        var round = await _context.Set<Round>()
            .Where(r => r.Id == request.RoundId && r.User.UserName == request.User)
            .Include(r => r.User.Currency)
            .Include(r => r.Transactions)
            .FirstOrDefaultAsync(cancellationToken);

        if (round is null)
        {
            var thisUser = await _context.Set<User>()
                .Where(u => u.UserName == request.User)
                .Include(u => u.Currency)
                .FirstAsync(cancellationToken);

            round = new Round
            {
                Id = request.RoundId,
                Finished = false,
                User = thisUser
            };
            _context.Add(round);

            await _context.SaveChangesAsync(cancellationToken);
        }

        var user = round.User;

        if (round.Transactions.Any(t => t.Id == request.TransactionId))
            return ResultFactory.Failure<BetOrWinResponse>(ErrorCode.DuplicateTransaction);

        if (round.Finished)
            return ResultFactory.Failure<BetOrWinResponse>(ErrorCode.Unknown);

        if (user.Currency.Name != request.Currency)
            return ResultFactory.Failure<BetOrWinResponse>(ErrorCode.WrongCurrency);

        user.Balance -= request.Amount;
        if (request.Finished)
            round.Finished = request.Finished;

        var transaction = new Transaction
        {
            Id = request.TransactionId,
            Amount = request.Amount,
        };

        round.Transactions.Add(transaction);

        _context.Update(round);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new BetOrWinResponse(
            user.Balance,
            user.Currency.Name,
            transaction.InternalId,
            transaction.CreatedDate);

        return ResultFactory.Success(response);
    }

    public async Task<IResult<BetOrWinResponse>> WinAsync(
        WinRequest request,
        CancellationToken cancellationToken)
    {
        var round = await _context.Set<Round>()
            .Where(
                r => r.Id == request.RoundId
                  && r.User.UserName == request.User)
            .Include(r => r.User.Currency)
            .Include(r => r.Transactions)
            .FirstOrDefaultAsync(cancellationToken);

        if (round is null || round.Transactions.Any(t => t.Id == request.TransactionId))
            return ResultFactory.Failure<BetOrWinResponse>(ErrorCode.DuplicateTransaction);

        if (round.Finished)
            return ResultFactory.Failure<BetOrWinResponse>(ErrorCode.Unknown);
        var user = round.User;
        if (user.Currency.Name != request.Currency)
            return ResultFactory.Failure<BetOrWinResponse>(ErrorCode.WrongCurrency);

        user.Balance += request.Amount;
        if (request.Finished)
            round.Finished = request.Finished;

        var transaction = new Transaction
        {
            Id = request.TransactionId,
            Amount = request.Amount
        };

        round.Transactions.Add(transaction);

        _context.Update(round);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new BetOrWinResponse(
            user.Balance,
            user.Currency.Name,
            transaction.InternalId,
            transaction.CreatedDate);

        return ResultFactory.Success(response);
    }

    public async Task<IResult<BetOrWinResponse>> RollbackAsync(
        RollbackRequest request,
        CancellationToken cancellationToken)
    {
        var round = await _context.Set<Round>()
            .Where(
                r => r.Id == request.RoundId
                  && r.User.UserName == request.User)
            .Include(r => r.User.Currency)
            .Include(r => r.Transactions)
            .FirstOrDefaultAsync(cancellationToken);

        if (round is null)
            return ResultFactory.Failure<BetOrWinResponse>(ErrorCode.BadParametersInTheRequest);

        if (round.Finished)
            return ResultFactory.Failure<BetOrWinResponse>(ErrorCode.Unknown);

        var lastTransaction = round.Transactions.MaxBy(t => t.CreatedDate);
        if (lastTransaction is null || lastTransaction.Id != request.TransactionId)
            return ResultFactory.Failure<BetOrWinResponse>(ErrorCode.TransactionDoesNotExist);

        var user = round.User;

        user.Balance += lastTransaction.Amount;
        _context.Update(user);

        round.Transactions.Remove(lastTransaction);
        _context.Update(round);

        await _context.SaveChangesAsync(cancellationToken);

        var response = new BetOrWinResponse(
            user.Balance,
            user.Currency.Name,
            lastTransaction.InternalId,
            lastTransaction.CreatedDate);

        return ResultFactory.Success(response);
    }

    public async Task<IResult<BalanceResponse>> AwardAsync(
        AwardRequest request,
        CancellationToken cancellationToken)
    {
        var award = await _context.Set<Award>()
            .Where(a => a.Id == request.AwardId)
            .Include(a => a.User)
            .Include(a => a.AwardRound!.Round)
            .FirstOrDefaultAsync(cancellationToken);

        if (award is null || award.User.UserName != request.User)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.AwardDoesNotExist);

        if (award.AwardRound is not null)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.DuplicateTransaction);

        var round = await _context.Set<Round>()
            .Where(r => r.Id == request.RoundId)
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(cancellationToken);

        if (round is not null)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.DuplicateTransaction);

        round = new Round
        {
            Id = request.RoundId,
            Finished = true,
            Transactions = new List<Transaction>
            {
                new()
                {
                    Id = request.TransactionId,
                    Amount = request.Amount
                }
            },
            AwardRound = new AwardRound {Award = award}
        };
        _context.Add(round);

        var user = award.User;

        user.Rounds.Add(round);
        user.Balance += request.Amount;
        _context.Update(user);

        await _context.SaveChangesAsync(cancellationToken);

        var result = new BalanceResponse(user.Balance, user.Currency.Name);

        return ResultFactory.Success(result);
    }
}