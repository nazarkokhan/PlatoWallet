namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Extensions;
using Application.Requests.Wallets.Sw.Base;
using Application.Results.Sw;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class SwSecurityFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var request = context.ActionArguments.Values
           .OfType<ISwBaseRequest>()
           .Single();

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var session = await dbContext.Set<Session>()
           .Where(s => s.Id == request.Token)
           .Select(
                s => new
                {
                    s.Id,
                    s.ExpirationDate,
                    User = new
                    {
                        s.User.IsDisabled,
                        CasinoSignatureKey = s.User.Casino.SignatureKey
                    },
                    IsTemporary = s.IsTemporaryToken
                })
           .FirstOrDefaultAsync(httpContext.RequestAborted);

        if (session is null || (session.IsTemporary && session.ExpirationDate < DateTime.UtcNow))
        {
            context.Result = SwResultFactory.Failure(SwErrorCode.ExpiredToken).ToActionResult();
            return;
        }

        if (session.User.IsDisabled)
        {
            context.Result = SwResultFactory.Failure(SwErrorCode.UserNotFound).ToActionResult();
            return;
        }

        var isValidSign = request switch
        {
            ISwMd5AmountRequest md5Request => md5Request.Map(
                r => SwSecurityMd5.IsValidSign(
                    r.Md5,
                    r.ProviderId,
                    r.UserId,
                    session.User.CasinoSignatureKey,
                    r.Amount)),
            ISwMd5Request md5Request => md5Request.Map(
                r => SwSecurityMd5.IsValidSign(
                    r.Md5,
                    r.ProviderId,
                    r.UserId,
                    session.User.CasinoSignatureKey)),
            ISwHashRequest hashRequest => hashRequest.Map(
                r => SwSecurityHash.IsValid(
                    r.Hash,
                    r.ProviderId,
                    r.UserId,
                    session.User.CasinoSignatureKey)),
            _ => false
        };

        if (!isValidSign)
        {
            context.Result = SwResultFactory.Failure(SwErrorCode.InvalidMd5OrHash).ToActionResult();
            return;
        }

        await next();
    }
}