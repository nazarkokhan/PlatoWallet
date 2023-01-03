namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Application.Requests.Wallets.Everymatrix.Base;
using Application.Results.Everymatrix;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class EveryMatrixHashFilterAttribute : ActionFilterAttribute
{

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var request = context.ActionArguments
            .Select(a =>a.Value as IEveryMatrixBaseRequest)
            .SingleOrDefault(r =>r is not null);

        var session = await httpContext.RequestServices.GetService<WalletDbContext>()
            .Set<Session>()
            .Where(s => s.Id == request.Token)
            .FirstOrDefaultAsync();

        var user = await httpContext.RequestServices.GetService<WalletDbContext>()
            .Set<User>()
            .Where(u => u.Id == session.UserId)
            .FirstOrDefaultAsync();

        var password = user.Password;

        var methodName = context.RouteData.Values["action"].ToString();

        var dateTime = DateTime.UtcNow.ToString("yyyy:MM:dd:HH", CultureInfo.InvariantCulture);

        var md5Hash = MD5.Create()
            .ComputeHash(Encoding.UTF8.GetBytes($"NameOfMethod({methodName})Time({dateTime})password({password})"));

        var validHash = Convert.ToHexString(md5Hash);

        var hashToVerify = context.ActionArguments
            .FirstOrDefault(a => a.Key == "hash");

        var isHashValid = validHash.Equals(hashToVerify);

        if (isHashValid)
        {
            context.Result = EverymatrixResultFactory.Failure(EverymatrixErrorCode.InvalidHash).ToActionResult();
        }

        await next();
    }
}