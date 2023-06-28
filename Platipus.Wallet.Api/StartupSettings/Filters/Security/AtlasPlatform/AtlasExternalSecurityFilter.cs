using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.StartupSettings.Filters.Security.AtlasPlatform;

using Application.Requests.Wallets.Atlas.Base;
using Application.Results.Atlas;

public sealed class AtlasExternalSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        byte[] bytes;
        string authHeader = context.HttpContext.Request.Headers["Authorization"]!;
        var encryptedClientData = authHeader.Replace("Basic", "");
        if (string.IsNullOrWhiteSpace(encryptedClientData))
        {
            context.Result = AtlasResultFactory.Failure<AtlasErrorResponse>(
                AtlasErrorCode.RequiredHeaderHashNotPresent).ToActionResult();
            return;
        }
        try
        {
            bytes = Convert.FromBase64String(encryptedClientData);
        }
        catch (Exception e)
        {
            context.Result = AtlasResultFactory.Failure<AtlasErrorResponse>(
                AtlasErrorCode.InvalidHashFormat).ToActionResult();
            return;
        }
        var decryptedData = Encoding.UTF8.GetString(bytes);
        var values = decryptedData.Split(':');
        if (values.Length is not 2)
        {
            AtlasResultFactory.Failure<AtlasErrorResponse>(
                AtlasErrorCode.InvalidHashFormat);
        }

        var (username, password) = (values[0], values[1]);
        
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var user = await dbContext
            .Set<User>()
            .Where(u => u.Username == username && 
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
            context.Result = AtlasResultFactory
                .Failure<AtlasErrorResponse>(AtlasErrorCode.SessionValidationFailed)
                .ToActionResult();
            return;
        }

        await next();
    }
}