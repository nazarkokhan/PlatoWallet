namespace Platipus.Wallet.Api.StartupSettings.Filters.Security.Evoplay;

using Api.Extensions;
using Api.Extensions.SecuritySign.Evoplay;
using Application.Requests.Wallets.Evoplay.Base;
using Application.Results.Evoplay;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public sealed class EvoplaySecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, 
        ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
            .OfType<IEvoplayRequest>()
            .Single();
        
        var httpContext = context.HttpContext;

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();
        IQueryable<Session> query = dbContext.Set<Session>();
        query = !string.IsNullOrEmpty(request.SessionToken) 
            ? query.Where(s => s.Id == request.SessionToken) 
            : query.Where(s => s.UserId.ToString() == request.PlayerId);

        var session = await query
            .Select(
                s => new
                {
                    s.ExpirationDate,
                    CasinoSignatureKey = s.User.Casino.SignatureKey,
                    UsedId = s.User.Id,
                    UserPassword = s.User.Password
                })
            .FirstOrDefaultAsync();

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = EvoplayResultFactory
                .Failure<EvoplayCommonErrorResponse>(EvoplayErrorCode.E_SESSION_TOKEN_INVALID_OR_EXPIRED)
                .ToActionResult();
            return;
        }

        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();
        string? authHeaderValue = context.HttpContext.Request.Headers["X-Signature"];
        if (authHeaderValue is null)
        {
            context.Result = EvoplayResultFactory.Failure<EvoplayCommonErrorResponse>(
                EvoplayErrorCode.E_PLAYER_SESSION_NOT_FOUND).ToActionResult();
            return;
        }
        var result = EvoplaySecurityHash.IsValid(
            authHeaderValue, requestBytesToValidate, session.CasinoSignatureKey);

        if (!result)
        {
            context.Result = EvoplayResultFactory.Failure<EvoplayCommonErrorResponse>(
                EvoplayErrorCode.E_SESSION_TOKEN_INVALID_OR_EXPIRED).ToActionResult();
            return;
        }

        await next();
    }
}