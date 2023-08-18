namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using System.Text.Json;
using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.BetConstruct.Base;
using Application.Requests.Wallets.BetConstruct.Base.Response;
using Application.Results.BetConstruct;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class BetconstructSecurityFilter : IAsyncActionFilter
{
    private readonly WalletDbContext _dbContext;

    public BetconstructSecurityFilter(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
            .OfType<IBetconstructBoxRequest<IBetconstructRequest>>()
            .Single();

        var httpContext = context.HttpContext;

        var session = await _dbContext
            .Set<Session>()
            .Where(s => s.Id == request.Data.Token)
            .Select(
                s => new
                {
                    s.ExpirationDate,
                    CasinoSignatureKey = s.User.Casino.SignatureKey,
                    IsTemporary = s.IsTemporaryToken
                })
            .FirstOrDefaultAsync();

        if (session is null || (session.IsTemporary && session.ExpirationDate < DateTime.UtcNow))
        {
            context.Result = BetconstructResultFactory.Failure<BetconstructErrorResponse>(BetconstructErrorCode.TokenNotFound)
                .ToActionResult();
            return;
        }

        var dataJsonString = JsonDocument.Parse(httpContext.GetRequestBodyBytesItem())
            .RootElement
            .GetProperty("data")
            .GetRawText();

        var isHashValid = BetConstructSecurityHash.IsValid(
            request.Hash,
            request.Time,
            dataJsonString,
            session.CasinoSignatureKey);

        if (!isHashValid)
        {
            context.Result = BetconstructResultFactory
                .Failure<BetconstructErrorResponse>(BetconstructErrorCode.AuthenticationFailed)
                .ToActionResult();
            return;
        }

        await next();
    }
}