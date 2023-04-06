namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.BetConstruct.Base.Response;
using Application.Requests.Wallets.TODO.EmaraPlay.Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Extensions;

public class EmaraPlayVerifyHashFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
            .OfType<IEmaraPlayBaseRequest>()
            .Single();

        var httpContext = context.HttpContext;

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var session = await dbContext
            .Set<Session>()
            .Where(s => s.Id == request.Token)
            .Select(
                s => new
                {
                    s.ExpirationDate,
                    CasinoSignatureKey = s.User.Casino.SignatureKey,
                })
            .FirstOrDefaultAsync();

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = EmaraPlayResultFactory
                .Failure<BetconstructErrorResponse>(EmaraPlayErrorCode.PlayerAuthenticationFailed)
                .ToActionResult();
            return;
        }

        string authHeader = context.HttpContext.Request.Headers["Authorization"]!;

        authHeader = authHeader.Replace("Bearer ", "");

        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();

        var result = EmaraplaySecurityHash.IsValid(authHeader, requestBytesToValidate, session.CasinoSignatureKey);

        if (!result)
        {
            context.Result = EmaraPlayResultFactory.Failure(EmaraPlayErrorCode.InvalidHashCode).ToActionResult();
        }

        await next();
    }
}