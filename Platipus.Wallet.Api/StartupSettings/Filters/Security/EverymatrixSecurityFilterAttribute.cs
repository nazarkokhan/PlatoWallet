namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Application.Requests.Wallets.Everymatrix.Base;
using Application.Requests.Wallets.Everymatrix.Base.Response;
using Application.Results.Everymatrix;
using Domain.Entities;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

public class EverymatrixSecurityFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var baseRequest = context.ActionArguments.Values
            .OfType<IEveryMatrixBaseRequest>()
            .Single();

        if (baseRequest is not IEveryMatrixRequest request)
        {
            await next();
            return;
        }

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var session = await dbContext
            .Set<Session>()
            .Where(s => s.Id == request.Token)
            .Select(
                s => new
                {
                    s.Id,
                    s.ExpirationDate,
                    s.IsTemporaryToken,
                    CasinoSignatureKey = s.User.Casino.SignatureKey,
                })
            .FirstOrDefaultAsync();

        if (session is null || session.ExpirationDate < DateTime.UtcNow || session.IsTemporaryToken)
        {
            context.Result = EverymatrixResultFactory.Failure<EverymatrixBalanceResponse>(EverymatrixErrorCode.TokenNotFound)
                .ToActionResult();
            return;
        }

        var requestRoute = context.ActionDescriptor.EndpointMetadata
            .OfType<HttpMethodAttribute>()
            .Single()
            .Template!;

        var isHashValid = EverymatrixSecurityHash.IsValidSign(request.Hash, requestRoute, session.CasinoSignatureKey);

        if (!isHashValid)
        {
            context.Result = EverymatrixResultFactory.Failure<EverymatrixBalanceResponse>(EverymatrixErrorCode.InvalidHash)
                .ToActionResult();
            return;
        }

        await next();
    }
}