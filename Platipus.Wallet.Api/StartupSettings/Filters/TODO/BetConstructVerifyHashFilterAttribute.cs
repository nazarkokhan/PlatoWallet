namespace Platipus.Wallet.Api.StartupSettings.Filters.TODO;

using System.Globalization;
using System.Text.Json;
using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.BetConstruct;
using Application.Requests.Wallets.BetConstruct.Base;
using Application.Requests.Wallets.BetConstruct.Base.Response;
using Application.Results.BetConstruct;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class BetConstructVerifyHashFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {

        var getPlayerInfoRequest = context.ActionArguments.Values
            .OfType<IBetConstructBaseRequest<GetPlayerInfoData>>()
            .SingleOrDefault();

        if (getPlayerInfoRequest is not null)
        {
            var result = await Verify(
                context,
                getPlayerInfoRequest.Data.Token,
                getPlayerInfoRequest.Hash,
                getPlayerInfoRequest.Time.ToString(CultureInfo.InvariantCulture),
                JsonSerializer.Serialize(getPlayerInfoRequest.Data));
            if (!result)
                return;
        }

        var withdrawRequest = context.ActionArguments.Values
            .OfType<IBetConstructBaseRequest<WithdrawData>>()
            .SingleOrDefault();

        if (withdrawRequest is not null)
        {
            var result = await Verify(
                context,
                withdrawRequest.Data.Token,
                withdrawRequest.Hash,
                withdrawRequest.Time.ToString(CultureInfo.InvariantCulture),
                JsonSerializer.Serialize(withdrawRequest.Data));
            if (!result)
                return;
        }

        var depositRequest = context.ActionArguments.Values
            .OfType<IBetConstructBaseRequest<DepositData>>()
            .SingleOrDefault();

        if (depositRequest is not null)
        {
            var result = await Verify(
                context,
                depositRequest.Data.Token,
                depositRequest.Hash,
                depositRequest.Time.ToString(CultureInfo.InvariantCulture),
                JsonSerializer.Serialize(depositRequest.Data));
            if (!result)
                return;
        }

        var rollbackRequest = context.ActionArguments.Values
            .OfType<IBetConstructBaseRequest<DepositData>>()
            .SingleOrDefault();

        if (rollbackRequest is not null)
        {
            var result = await Verify(
                context,
                rollbackRequest.Data.Token,
                rollbackRequest.Hash,
                rollbackRequest.Time.ToString(CultureInfo.InvariantCulture),
                JsonSerializer.Serialize(rollbackRequest.Data));
            if (!result)
                return;
        }


        await next();
    }

    public async Task<bool> Verify(
        ActionExecutingContext context,
        string token,
        string hash,
        string time,
        string data)
    {
        var httpContext = context.HttpContext;

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var session = await dbContext
            .Set<Session>()
            .Where(s => s.Id == token)
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
            return false;
        }


        var isHashValid = BetConstructSecurityHash.IsValid(hash, time, data);

        if (!isHashValid)
        {
            context.Result = BetConstructResultFactory
                .Failure<BetConstructBaseResponse>(BetConstructErrorCode.AuthenticationFailed)
                .ToActionResult();
            return false;
        }

        return true;
    }
}

public static class BetConstructActionName
{
    public const string GetPlayerInfo = "GetPlayerInfo";
    public const string Withdraw = "Withdraw";
    public const string Deposit = "Deposit";
    public const string Rollback = "Rollback";
}