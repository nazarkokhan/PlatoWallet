namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using System.Text.Json;
using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.BetConstruct.Base;
using Application.Requests.Wallets.BetConstruct.Base.Response;
using Application.Results.BetConstruct;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SkipFilterToGetHash;

public class BetConstructVerifyHashFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var suppress =
            context.ActionDescriptor.FilterDescriptors.FirstOrDefault(
                f => f.Filter.GetTypeName() == nameof(SkipVerifyFilterAttribute));

        if (suppress is not null)
        {
            await next();
            return;
        }

        var baseRequest = context.ActionArguments.Values
            .OfType<IBetConstructBaseRequest<IBetConstructDataRequest>>()
            .SingleOrDefault();

        if (baseRequest is null)
        {
            context.Result = BetConstructResultFactory.Failure<BetConstructBaseResponse>(BetConstructErrorCode.GeneralError)
                .ToActionResult();
            return;
        }

        var httpContext = context.HttpContext;

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var session = await dbContext
            .Set<Session>()
            .Where(s => s.Id == baseRequest.Data.Token)
            .Select(
                s => new
                {
                    s.Id,
                    s.ExpirationDate,
                    s.IsTemporaryToken,
                    CasinoSignatureKey = s.User.Casino.SignatureKey,
                })
            .FirstOrDefaultAsync();

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = BetConstructResultFactory.Failure<BetConstructBaseResponse>(BetConstructErrorCode.TokenNotFound)
                .ToActionResult();
            return;
        }

        var dataToSerialize = Convert.ChangeType(baseRequest.Data, baseRequest.Data.GetType());

        var dataToCompare = JsonSerializer.Serialize(dataToSerialize);

        var isHashValid = BetConstructSecurityHash.IsValid(
            baseRequest.Hash,
            baseRequest.Time.ToString("dd-MM-yyyy HH:mm:ss"),
            dataToCompare,
            session.CasinoSignatureKey);

        if (!isHashValid)
        {
            context.Result = BetConstructResultFactory
                .Failure<BetConstructBaseResponse>(BetConstructErrorCode.AuthenticationFailed)
                .ToActionResult();
            return;
        }

        await next();
    }
}