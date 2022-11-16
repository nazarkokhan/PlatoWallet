namespace Platipus.Wallet.Api.Application.Services.Wallet;

using Domain.Entities;
using DTOs;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Common;

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

    public async Task<IResult<BalanceResponse>> BetAsync(
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
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.DuplicateTransaction);

        if (round.Finished)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.Unknown);

        if (user.Currency.Name != request.Currency)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.WrongCurrency);

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

        var response = new BalanceResponse(user.Balance, user.Currency.Name);

        return ResultFactory.Success(response);
    }

    public async Task<IResult<BalanceResponse>> WinAsync(
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
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.DuplicateTransaction);

        if (round.Finished)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.Unknown);
        var user = round.User;
        if (user.Currency.Name != request.Currency)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.WrongCurrency);

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

        var response = new BalanceResponse(user.Balance, user.Currency.Name);

        return ResultFactory.Success(response);
    }

    public async Task<IResult<BalanceResponse>> RollbackAsync(
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
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.BadParametersInTheRequest);

        if (round.Finished)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.Unknown);

        var lastTransaction = round.Transactions.MaxBy(t => t.CreatedDate);
        if (lastTransaction is null || lastTransaction.Id != request.TransactionId)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.TransactionDoesNotExist);

        var user = round.User;

        user.Balance += lastTransaction.Amount;
        _context.Update(user);

        round.Transactions.Remove(lastTransaction);
        _context.Update(round);

        await _context.SaveChangesAsync(cancellationToken);

        var response = new BalanceResponse(user.Balance, user.Currency.Name);

        return ResultFactory.Success(response);
    }
}