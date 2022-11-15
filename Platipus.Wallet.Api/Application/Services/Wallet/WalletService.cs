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
                s => new
                {
                    s.Id,
                    s.Balance,
                    s.IsDisabled
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.InvalidUser);

        if (user.IsDisabled)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.UserDisabled);

        var response = new BalanceResponse(user.Balance);

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
            var user = await _context.Set<User>()
                .Where(u => u.UserName == request.User)
                .Include(u => u.Currency)
                .FirstAsync(cancellationToken);

            round = new Round
            {
                Id = request.RoundId,
                Finished = false,
                User = user
            };
            _context.Add(round);

            await _context.SaveChangesAsync(cancellationToken);
        }

        if (round.Transactions.Any(t => t.Id == request.TransactionId))
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.DuplicateTransaction);

        if (round.Finished)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.Unknown);

        if (round.User.Currency.Name != request.Currency)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.WrongCurrency);

        round.User.Balance -= request.Amount;
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

        var response = new BalanceResponse(round.User.Balance);

        return ResultFactory.Success(response);
    }

    public async Task<IResult<BalanceResponse>> WinAsync(
        WinRequest request,
        CancellationToken cancellationToken)
    {
        var round = await _context.Set<Round>()
            .Where(r => r.Id == request.RoundId && r.User.UserName == request.User)
            .Include(r => r.User.Currency)
            .Include(r => r.Transactions)
            .FirstOrDefaultAsync(cancellationToken);

        if (round is null || round.Transactions.Any(t => t.Id == request.TransactionId))
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.DuplicateTransaction);

        if (round.Finished)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.Unknown);

        if (round.User.Currency.Name != request.Currency)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.WrongCurrency);

        round.User.Balance += request.Amount;
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

        var response = new BalanceResponse(round.User.Balance);

        return ResultFactory.Success(response);
    }

    public async Task<IResult<BalanceResponse>> RollbackAsync(
        RollbackRequest request,
        CancellationToken cancellationToken)
    {
        var round = await _context.Set<Round>()
            .Where(r => r.Id == request.RoundId && r.User.UserName == request.User)
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
        if (user.Currency.Name != request.Currency)
            return ResultFactory.Failure<BalanceResponse>(ErrorCode.WrongCurrency);

        user.Balance += request.Amount;
        _context.Update(user);

        round.Transactions.Remove(lastTransaction);
        _context.Update(round);

        await _context.SaveChangesAsync(cancellationToken);

        var response = new BalanceResponse(user.Balance);

        return ResultFactory.Success(response);
    }
}