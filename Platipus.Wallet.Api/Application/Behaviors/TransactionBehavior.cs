namespace Platipus.Wallet.Api.Application.Behaviors;

using System.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Infrastructure.Persistence;
using Results.Base;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<IBaseResult>, IBaseRequest
    where TResponse : class, IBaseResult
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly WalletDbContext _dbContext;

    public TransactionBehavior(
        ILogger<TransactionBehavior<TRequest, TResponse>> logger,
        WalletDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(
            IsolationLevel.RepeatableRead,
            cancellationToken);

        var result = await next();

        if (result.IsFailure)
        {
            _logger.LogWarning("Rolling back transaction {TransactionId}", transaction.TransactionId);
            await transaction.RollbackAsync(cancellationToken);
        }
        else
            await transaction.CommitAsync(cancellationToken);

        return result;
    }
}