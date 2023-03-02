namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.Uis;
using Application.Requests.Wallets.Uis.Base;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class UisSecurityFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
            .OfType<IUisRequest>()
            .Single();

        string userIdOrToken;
        var isAuthRequest = false;
        if (request is UisAuthenticateRequest authenticateRequest)
        {
            userIdOrToken = authenticateRequest.Token;
            isAuthRequest = true;
        }
        else
        {
            var userIdRequest = (IUisUserIdRequest)request;
            userIdOrToken = userIdRequest.UserId;
        }

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<WalletDbContext>();
        var user = await dbContext.Set<User>()
            .Where(
                u => isAuthRequest
                    ? u.Sessions.Any(s => s.Id == userIdOrToken)
                    : u.Username == userIdOrToken)
            .Select(
                u => new
                {
                    u.IsDisabled,
                    Casino = new
                    {
                        u.CasinoId,
                        u.Casino.Provider,
                        u.Casino.SignatureKey
                    }
                })
            .FirstOrDefaultAsync(context.HttpContext.RequestAborted);

        if (user is null || user.IsDisabled || user.Casino.Provider is not CasinoProvider.Uis)
        {
            context.Result = DafabetResultFactory.Failure(DafabetErrorCode.PlayerNotFound).ToActionResult();
            return;
        }

        var isValidSecurity = request.Hash is null //TODO correct?
                           || UisSecurityHash.IsValid(request.Hash, request.GetSource(), user.Casino.SignatureKey);

        if (!isValidSecurity)
        {
            context.Result = DafabetResultFactory.Failure(DafabetErrorCode.InvalidHash).ToActionResult();
            return;
        }

        await next();
    }
}