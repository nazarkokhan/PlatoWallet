namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.Net.Mime;
using System.Text.Json;
using Application.Requests.Wallets.Dafabet.Base;
using Application.Requests.Wallets.Hub88.Base;
using Application.Requests.Wallets.Openbox;
using Application.Requests.Wallets.Openbox.Base;
using Application.Requests.Wallets.Psw.Base;
using Application.Requests.Wallets.SoftBet.Base;
using Application.Requests.Wallets.Softswiss.Base;
using Application.Requests.Wallets.Sw.Base;
using Controllers;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

public class MockedErrorActionFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var cancellationToken = httpContext.RequestAborted;
        var services = httpContext.RequestServices;
        var logger = services.GetRequiredService<ILogger<MockedErrorActionFilterAttribute>>();
        var dbContext = services.GetRequiredService<WalletDbContext>();

        logger.LogInformation("Handling request with possible mocked error");

        var executedContext = await next();

        var requestRoute = context.ActionDescriptor.EndpointMetadata
            .OfType<HttpMethodAttribute>()
            .SingleOrDefault()
            ?.Template;

        if (requestRoute is null)
        {
            logger.LogCritical("Request route not found");
            return;
        }

        ErrorMockMethod? currentMethod;
        string? usernameOrSession;

        if (context.Controller is WalletOpenboxController)
        {
            var singleRequest = context.ActionArguments.Values.OfType<OpenboxSingleRequest>().Single();

            if (!httpContext.Items.TryGetValue("OpenboxPayloadRequestObj", out var payloadRequestObj)
             || payloadRequestObj is not IOpenboxBaseRequest openboxPayloadObj)
            {
                LogOpenboxMockingFailed(logger, singleRequest);
                return;
            }

            usernameOrSession = openboxPayloadObj.Token.ToString();
            currentMethod = singleRequest.Method switch
            {
                OpenboxHelpers.GetPlayerBalance => ErrorMockMethod.Balance,
                OpenboxHelpers.MoneyTransactions => ((OpenboxMoneyTransactionRequest)openboxPayloadObj).OrderType switch
                {
                    3 => ErrorMockMethod.Bet,
                    4 => ErrorMockMethod.Win,
                    _ => null
                },
                OpenboxHelpers.CancelTransaction => ErrorMockMethod.Rollback,
                _ => null
            };
        }
        else if (context.Controller is WalletISoftBetController)
        {
            var singleRequest = context.ActionArguments.Values.OfType<SoftBetSingleRequest>().Single();

            usernameOrSession = singleRequest.Username;
            currentMethod = singleRequest.Action.Command switch
            {
                "balance" => ErrorMockMethod.Balance,
                "bet" => ErrorMockMethod.Bet,
                "win" => ErrorMockMethod.Win,
                "rollback" => ErrorMockMethod.Rollback,
                _ => null
            };
        }
        else
        {
            currentMethod = requestRoute switch
            {
                "balance" or "balance-md5" or "balance-hash" or "user/balance" => ErrorMockMethod.Balance,
                "bet" or "bet-win" or "play" or "transaction/bet" => ErrorMockMethod.Bet,
                "win" or "result" or "transaction/win" => ErrorMockMethod.Win,
                "award" or "bonusWin" or "freespin" or "freespins" => ErrorMockMethod.Award,
                "rollback" or "cancel" or "refund" or "transaction/rollback" => ErrorMockMethod.Rollback,
                _ => null
            };

            usernameOrSession = context.Controller switch
            {
                WalletPswController => context.ActionArguments
                    .Select(a => a.Value as IPswBaseRequest)
                    .SingleOrDefault(a => a is not null)
                    ?.SessionId.ToString(),
                WalletDafabetController => context.ActionArguments
                    .Select(a => a.Value as IDafabetBaseRequest)
                    .SingleOrDefault(a => a is not null)
                    ?.PlayerId,
                WalletOpenboxController => context.ActionArguments.Values
                    .OfType<IOpenboxBaseRequest>()
                    .SingleOrDefault()
                    ?.Token.ToString(),
                WalletHub88Controller => context.ActionArguments
                    .Select(a => a.Value as IHub88BaseRequest)
                    .SingleOrDefault(a => a is not null)
                    ?.SupplierUser,
                WalletSoftswissController => context.ActionArguments.Values
                    .OfType<ISoftswissBaseRequest>()
                    .SingleOrDefault()
                    ?.SessionId.ToString(),
                WalletSwController => context.ActionArguments
                    .Select(a => a.Value as ISwBaseRequest)
                    .SingleOrDefault(a => a is not null)
                    ?.Token.ToString(),
                _ => null
            };
        }

        if (currentMethod is null)
        {
            logger.LogCritical("ErrorMockMethod not found for route {RequestRoute}", requestRoute);
            return;
        }

        if (usernameOrSession is null)
        {
            logger.LogCritical("Can not mock error for request because UserName is empty");
            return;
        }

        var mockedErrorQuery = dbContext.Set<MockedError>()
            .Where(e => e.Method == currentMethod);

        mockedErrorQuery = context.Controller switch
        {
            WalletOpenboxController => mockedErrorQuery
                .Where(e => e.User.Sessions.Any(s => s.Id == new Guid(usernameOrSession))),
            _ => mockedErrorQuery
                .Where(e => e.User.UserName == usernameOrSession)
        };

        var mockedError = await mockedErrorQuery.FirstOrDefaultAsync(executedContext.HttpContext.RequestAborted);

        if (mockedError is null)
        {
            logger.LogInformation("Mocked error not found");
            return;
        }

        logger.LogInformation(
            "Executing mocked error {@MockedError}",
            new
            {
                mockedError.Method,
                mockedError.Body,
                mockedError.HttpStatusCode,
                mockedError.ContentType,
                mockedError.Count,
                mockedError.UserId,
                mockedError.Timeout
            });

        if (mockedError.Timeout is not null)
            await Task.Delay(mockedError.Timeout.Value, cancellationToken);

        switch (mockedError.ContentType)
        {
            case MediaTypeNames.Text.Plain or MediaTypeNames.Text.Xml or MediaTypeNames.Text.Html:
                context.HttpContext.Items.Add("response", mockedError.Body);
                executedContext.Result = new ContentResult
                {
                    Content = mockedError.Body,
                    StatusCode = (int?)mockedError.HttpStatusCode,
                    ContentType = mockedError.ContentType
                };
                break;
            case MediaTypeNames.Application.Json or _:
                object? response = null;
                try
                {
                    response = JsonSerializer.Deserialize<object>(mockedError.Body);
                    context.HttpContext.Items.Add("response", response);
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, "Error deserializing mocked error body");
                    context.HttpContext.Items.Add("response", mockedError.Body);
                }

                executedContext.Result = new ObjectResult(response ?? mockedError.Body)
                {
                    StatusCode = (int?)mockedError.HttpStatusCode,
                };
                break;
        }

        mockedError.Count -= 1;

        if (mockedError.Count <= 0)
        {
            logger.LogInformation("Mocked error count is 0, deleting it");
            dbContext.Remove(mockedError);
            logger.LogInformation("Mocked error deleted");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void LogOpenboxMockingFailed(ILogger logger, OpenboxSingleRequest openboxRequest)
        => logger.LogCritical("Failed mocking openbox request. {OpenboxRequest}", openboxRequest);
}