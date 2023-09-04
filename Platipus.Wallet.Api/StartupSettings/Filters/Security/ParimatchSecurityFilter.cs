namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Application.Requests.Wallets.Parimatch.Base;
using Application.Results.Parimatch;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public sealed class ParimatchSecurityFilter : IAsyncActionFilter
{
    private readonly WalletDbContext _dbContext;

    public ParimatchSecurityFilter(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var xHubConsumer = context.HttpContext.Request.Headers.GetXHubConsumer();
        if (xHubConsumer is null)
        {
            context.Result = ParimatchResultFactory.Failure(ParimatchErrorCode.IntegrationHubFailure).ToActionResult();
            return;
        }

        var request = context.ActionArguments.Values
           .OfType<IParimatchRequest>()
           .Single();

        if (request is IParimatchSessionRequest parimatchSessionRequest)
        {
            var session = await _dbContext
               .Set<Session>()
               .Where(s => s.Id == parimatchSessionRequest.SessionToken)
               .Select(
                    s => new
                    {
                        s.Id,
                        s.ExpirationDate,
                        User = new
                        {
                            s.User.IsDisabled,
                            Casino = new
                            {
                                Id = s.User.CasinoId,
                                s.User.Casino.InternalId
                            }
                        },
                        IsTemporary = s.IsTemporaryToken
                    })
               .FirstOrDefaultAsync();

            if (session is null || session.ExpirationDate < DateTime.UtcNow)
            {
                context.Result = ParimatchResultFactory.Failure(ParimatchErrorCode.InvalidSessionKey).ToActionResult();
                return;
            }
            if (request.Cid != session.User.Casino.Id || xHubConsumer != session.User.Casino.InternalId.ToString())
            {
                context.Result = ParimatchResultFactory.Failure(ParimatchErrorCode.IntegrationHubFailure).ToActionResult();
                return;
            }
            if (session.User.IsDisabled)
            {
                context.Result = ParimatchResultFactory.Failure(ParimatchErrorCode.LockedPlayer).ToActionResult();
                return;
            }
        }
        else
        {
            var parimatchPlayerIdRequest = (IParimatchPlayerIdRequest)request;

            var user = await _dbContext
               .Set<User>()
               .Where(s => s.Username == parimatchPlayerIdRequest.PlayerId)
               .Select(
                    u => new
                    {
                        u.Username,
                        u.IsDisabled,
                        Casino = new
                        {
                            Id = u.CasinoId,
                            u.Casino.InternalId
                        }
                    })
               .FirstOrDefaultAsync();

            if (user is null)
            {
                context.Result = ParimatchResultFactory.Failure(ParimatchErrorCode.InvalidSessionKey).ToActionResult();
                return;
            }
            if (request.Cid != user.Casino.Id || xHubConsumer != user.Casino.InternalId.ToString())
            {
                context.Result = ParimatchResultFactory.Failure(ParimatchErrorCode.IntegrationHubFailure).ToActionResult();
                return;
            }
            if (user.IsDisabled)
            {
                context.Result = ParimatchResultFactory.Failure(ParimatchErrorCode.LockedPlayer).ToActionResult();
                return;
            }
        }

        await next();
    }
}