namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Base;
using Application.Requests.Wallets.Softswiss;
using Application.Requests.Wallets.Softswiss.Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class SoftswissSecurityFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var xRequestSign = httpContext.Request.Headers.GetXRequestSign();
        if (xRequestSign is null)
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.Forbidden).ToActionResult();
            return;
        }

        var request = context.ActionArguments.Values
            .OfType<ISoftswissBaseRequest>()
            .FirstOrDefault();

        if (request is null)
        {
            await next();
            return;
        }

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var session = await dbContext.Set<Session>()
            .Where(s => s.Id == request.SessionId)
            .Select(
                s => new
                {
                    s.Id,
                    s.ExpirationDate,
                    User = new
                    {
                        s.User.IsDisabled,
                        CasinoSignatureKey = s.User.Casino.SignatureKey
                    }
                })
            .FirstOrDefaultAsync(httpContext.RequestAborted);

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.Forbidden).ToActionResult();
            return;
        }

        if (session.User.IsDisabled)
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.PlayerIsDisabled).ToActionResult();
            return;
        }

        var rawRequestBytes = httpContext.GetRequestBodyBytesItem();

        var isValidSign = SoftswissSecurityHash.IsValid(xRequestSign, rawRequestBytes, session.User.CasinoSignatureKey);

        if (!isValidSign)
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.Forbidden).ToActionResult();
            return;
        }

        await next();
    }
}