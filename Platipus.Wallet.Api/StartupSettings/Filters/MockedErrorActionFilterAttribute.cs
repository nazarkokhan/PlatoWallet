namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.Net.Mime;
using System.Text.Json;
using Application.Requests.Wallets.Dafabet.Base;
using Application.Requests.Wallets.Hub88.Base;
using Application.Requests.Wallets.Openbox.Base;
using Application.Requests.Wallets.Psw.Base;
using Controllers;
using Domain.Entities;
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
        var services = httpContext.RequestServices;
        var logger = services.GetRequiredService<ILogger<MockedErrorActionFilterAttribute>>();

        logger.LogInformation("Handling request with possible mocked error");

        var executedContext = await next();

        var requestRoute = context.ActionDescriptor.EndpointMetadata
            .OfType<HttpMethodAttribute>()
            .SingleOrDefault()
            ?
            .Template;

        if (requestRoute is null)
        {
            logger.LogCritical("Request route not found");
            return;
        }

        ErrorMockMethod? currentMethod = requestRoute switch
        {
            "balance" => ErrorMockMethod.Balance,
            "bet" => ErrorMockMethod.Bet,
            "win" or "result" => ErrorMockMethod.Win,
            "award" or "bonusWin" => ErrorMockMethod.Award,
            "rollback" or "cancel" => ErrorMockMethod.Rollback,
            _ => null
        };

        if (currentMethod is null)
        {
            logger.LogCritical("ErrorMockMethod not found for route {RequestRoute}", requestRoute);
            return;
        }

        var username = context.Controller switch
        {
            WalletPswController => context.ActionArguments
                .Select(a => a.Value as PswBaseRequest)
                .SingleOrDefault(a => a is not null)
                ?.User,
            WalletDafabetController => context.ActionArguments
                .Select(a => a.Value as DatabetBaseRequest)
                .SingleOrDefault(a => a is not null)
                ?.PlayerId,
            WalletOpenboxController => context.ActionArguments
                .Select(a => a.Value as OpenboxBaseRequest)
                .SingleOrDefault(a => a is not null)
                ?.Token,
            WalletHub88Controller => context.ActionArguments
                .Select(a => a.Value as Hub88BaseRequest)
                .SingleOrDefault(a => a is not null)
                ?.SupplierUser,
            _ => null
        };

        if (username is null)
        {
            logger.LogCritical("Can not mock error for request because UserName is empty");
            return;
        }

        var dbContext = services.GetRequiredService<WalletDbContext>();

        var mockedErrorQuery = dbContext.Set<MockedError>()
            .Where(e => e.Method == currentMethod);

        mockedErrorQuery = context.Controller switch
        {
            WalletOpenboxController => mockedErrorQuery
                .Where(e => e.User.Sessions.Any(s => s.Id == new Guid(username))),
            _ => mockedErrorQuery
                .Where(e => e.User.UserName == username)
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
                mockedError.UserId
            });

        switch (mockedError.ContentType)
        {
            case MediaTypeNames.Text.Plain or MediaTypeNames.Text.Xml or MediaTypeNames.Text.Html:
                context.HttpContext.Items.Add("response", mockedError.Body);
                executedContext.Result = new ContentResult
                {
                    Content = mockedError.Body,
                    StatusCode = (int?) mockedError.HttpStatusCode,
                    ContentType = mockedError.ContentType
                };
                break;
            case MediaTypeNames.Application.Json or _:
                var response = JsonSerializer.Deserialize<object>(mockedError.Body);
                context.HttpContext.Items.Add("response", response);
                executedContext.Result = new ObjectResult(response)
                {
                    StatusCode = (int?) mockedError.HttpStatusCode,
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

        await dbContext.SaveChangesAsync();
    }
}