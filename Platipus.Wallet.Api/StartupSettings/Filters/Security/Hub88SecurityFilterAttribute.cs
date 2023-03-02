namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.Hub88.Base;
using Application.Results.Hub88;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class Hub88SecurityFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
            .OfType<IHub88BaseRequest>()
            .Single();

        var httpContext = context.HttpContext;
        var cancellationToken = httpContext.RequestAborted;
        var services = httpContext.RequestServices;
        var dbContext = services.GetRequiredService<WalletDbContext>();

        var requestExist = await dbContext.Set<Request>()
            .Where(r => r.Id == request.RequestUuid)
            .AnyAsync(cancellationToken);

        if (requestExist)
        {
            context.Result = Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_WRONG_SYNTAX).ToActionResult();
            return;
        }

        var xRequestSign = httpContext.Request.Headers.GetXHub88Signature();
        if (xRequestSign is null)
        {
            context.Result = Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_INVALID_SIGNATURE).ToActionResult();
            return;
        }

        var session = await dbContext.Set<Session>()
            .Where(c => c.Id == request.Token)
            .Select(
                s => new
                {
                    s.Id,
                    s.ExpirationDate,
                    UserId = s.User.Id,
                    UserIsDisabled = s.User.IsDisabled,
                    UserCasinoSignatureKey = s.User.Casino.SignatureKey
                })
            .FirstOrDefaultAsync(httpContext.RequestAborted);

        if (session is null)
        {
            context.Result = Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_INVALID_TOKEN).ToActionResult();
            return;
        }

        if (session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_TOKEN_EXPIRED).ToActionResult();
            return;
        }

        if (session.UserIsDisabled)
        {
            context.Result = Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_USER_DISABLED).ToActionResult();
            return;
        }

        var rawRequestBytes = httpContext.GetRequestBodyBytesItem();

        var isValidSign = Hub88SecuritySign.IsValid(xRequestSign, rawRequestBytes, session.UserCasinoSignatureKey);

        if (!isValidSign)
        {
            context.Result = Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_INVALID_SIGNATURE).ToActionResult();
            return;
        }

        var requestEntity = new Request(request.RequestUuid) { UserId = session.UserId };
        dbContext.Add(requestEntity);

        await dbContext.SaveChangesAsync(cancellationToken);

        await next();
    }
}