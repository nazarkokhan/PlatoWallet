namespace Platipus.Wallet.Api.StartupSettings.Filters.Security.Sweepium;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;
using Platipus.Wallet.Api.Application.Responses.Sweepium.Base;
using Platipus.Wallet.Api.Application.Results.Sweepium;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.Extensions.SecuritySign.Sweepium;
using Domain.Entities;
using Infrastructure.Persistence;

public sealed class SweepiumSecurityFilter : IAsyncActionFilter
{
    private readonly WalletDbContext _dbContext;

    public SweepiumSecurityFilter(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
            .OfType<ISweepiumBoxRequest<ISweepiumRequest>>()
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
            context.Result = SweepiumResultFactory.Failure<SweepiumErrorResponse>(SweepiumErrorCode.Token_Not_Found)
                .ToActionResult();
            return;
        }

        var dataJsonString = JsonDocument.Parse(httpContext.GetRequestBodyBytesItem())
            .RootElement
            .GetProperty("data")
            .GetRawText();

        var isHashValid = SweepiumSecurityHash.IsValid(
            request.Hash,
            request.Time,
            dataJsonString,
            session.CasinoSignatureKey);

        if (!isHashValid)
        {
            context.Result = SweepiumResultFactory
                .Failure<SweepiumErrorResponse>(SweepiumErrorCode.Authentication_Failed)
                .ToActionResult();
            return;
        }

        await next();
    }
}