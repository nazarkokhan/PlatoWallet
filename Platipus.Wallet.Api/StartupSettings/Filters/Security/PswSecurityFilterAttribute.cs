namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.Psw.Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class PswSecurityFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var xRequestSign = httpContext.Request.Headers.GetXRequestSign();
        if (xRequestSign is null)
        {
            context.Result = PswResultFactory.Failure(PswErrorCode.MissingSignature).ToActionResult();
            return;
        }

        var request = context.ActionArguments.Values
            .OfType<IPswBaseRequest>()
            .Single();

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
            context.Result = PswResultFactory.Failure(PswErrorCode.SessionExpired).ToActionResult();
            return;
        }

        if (session.User.IsDisabled)
        {
            context.Result = PswResultFactory.Failure(PswErrorCode.UserDisabled).ToActionResult();
            return;
        }

        var rawRequestBytes = httpContext.GetRequestBodyBytesItem();

        var isValidSign = PswSecuritySign.IsValid(xRequestSign, rawRequestBytes, session.User.CasinoSignatureKey);

        if (!isValidSign)
        {
            context.Result = PswResultFactory.Failure(PswErrorCode.InvalidSignature).ToActionResult();
            return;
        }

        await next();
    }
}