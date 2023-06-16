using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.StartupSettings.Filters.Security.AtlasPlatform;

public sealed class AtlasPlatformBasicSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        byte[] bytes;
        string authHeader = context.HttpContext.Request.Headers["Authorization"]!;
        var encryptedClientData = authHeader.Replace("Basic", "");
        if (string.IsNullOrWhiteSpace(encryptedClientData))
        {
            context.Result = AtlasPlatformResultFactory.Failure<AtlasPlatformErrorResponse>(
                AtlasPlatformErrorCode.RequiredHeaderHashNotPresent).ToActionResult();
            return;
        }
        try
        {
            bytes = Convert.FromBase64String(encryptedClientData);
        }
        catch (Exception e)
        {
            context.Result = AtlasPlatformResultFactory.Failure<AtlasPlatformErrorResponse>(
                AtlasPlatformErrorCode.InvalidHashFormat).ToActionResult();
            return;
        }
        var decryptedData = Encoding.UTF8.GetString(bytes);
        var values = decryptedData.Split(':');
        if (values.Length is not 2)
        {
            AtlasPlatformResultFactory.Failure<AtlasPlatformErrorResponse>(
                AtlasPlatformErrorCode.InvalidHashFormat);
        }

        var (_, password) = (int.TryParse(values[0], out var userId), values[1]);
        
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var user = await dbContext
            .Set<User>()
            .Where(u => u.Id == userId && 
                        u.Password == password)
            .Select(
                u => new
                {
                    UsedId = u.Id,
                    UserPassword = u.Password
                })
            .FirstOrDefaultAsync();

        if (user is null)
        {
            context.Result = AtlasPlatformResultFactory
                .Failure<AtlasPlatformErrorResponse>(AtlasPlatformErrorCode.SessionValidationFailed)
                .ToActionResult();
            return;
        }

        await next();
    }
}