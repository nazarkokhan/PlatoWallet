namespace Platipus.Wallet.Api.StartupSettings.Filters.TODO;

using System.Globalization;
using Application.Requests.Wallets.Everymatrix.Base;
using Application.Requests.Wallets.Everymatrix.Base.Response;
using Application.Results.Everymatrix;
using Domain.Entities;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class EveryMatrixVerifySignatureFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var methodName = context.RouteData.Values["action"].ToString();
        if (methodName == "Reconciliation" || methodName == "Authenticate")
        {
            await next();
            return;
        }

        var request = context.ActionArguments
            .Select(a => a.Value as IEveryMatrixBaseRequest)
            .SingleOrDefault(r => r is not null);

        var dbContext = httpContext?.RequestServices.GetService<WalletDbContext>();

        var session = await dbContext
            .Set<Session>()
            .Where(s => s.Id == new Guid(request.Token))
            .FirstOrDefaultAsync();

        var user = await dbContext
            .Set<User>()
            .Where(u => u.Id == session.UserId)
            .FirstOrDefaultAsync();

        var password = user.Password;

        var dateTime = DateTime.UtcNow.ToString("yyyy:MM:dd:HH", CultureInfo.InvariantCulture);

        var stringToVerify = $"NameOfMethod({methodName})Time({dateTime})password({password})";

        var isHashValid = EverymatrixSecuritySign.IsValidSign(request.Hash, stringToVerify);

        if (!isHashValid)
        {
            context.Result = EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.InvalidHash)
                .ToActionResult();
        }

        await next();
    }
}