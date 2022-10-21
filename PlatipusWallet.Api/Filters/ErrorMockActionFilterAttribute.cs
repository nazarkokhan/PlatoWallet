namespace PlatipusWallet.Api.Filters;

using System.Net.Mime;
using Application.Requests.Base.Requests;
using Controllers.Wallets;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result.Factories;

public class ErrorMockActionFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var services = context.HttpContext.RequestServices;
        var logger = services.GetRequiredService<ILogger<ErrorMockActionFilterAttribute>>();

        logger.LogInformation("Handling request with possible mocked error");

        // Before controller action
        var executedContext = await next();
        // After controller action

        var username = context.Controller is WalletDafabetController
            ? context.ActionArguments.Select(a => a.Value as DatabetBaseRequest).SingleOrDefault(a => a is not null)?.PlayerId
            : context.ActionArguments.Select(a => a.Value as BaseRequest).SingleOrDefault(a => a is not null)?.User;

        if (username is null)
        {
            logger.LogCritical("Can not mock error for request because UserName is empty");
            executedContext.Result = ResultFactory.Failure(ErrorCode.CouldNotTryToMockSessionError).ToActionResult();
            return;
        }

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

        var dbContext = services.GetRequiredService<WalletDbContext>();
        var mockedError = await dbContext.Set<MockedError>()
            .Where(
                e => e.User.UserName == username &&
                     e.Method == currentMethod)
            .FirstOrDefaultAsync(executedContext.HttpContext.RequestAborted);

        if (mockedError is null)
        {
            logger.LogInformation("Mocked error not found");
            return;
        }

        if (mockedError.Count is 0)
        {
            logger.LogInformation("Mocked error count is 0, deleting it");
            dbContext.Remove(mockedError);
            logger.LogInformation("Mocked error deleted");
        }
        else
        {
            logger.LogInformation("Executing mocked error {@MockedError}", mockedError);

            executedContext.Result = new ObjectResult(mockedError.Body)
            {
                StatusCode = (int?)mockedError.HttpStatusCode,
                ContentTypes = new MediaTypeCollection { mockedError.ContentType }
            };

            mockedError.Count -= 1;
        }

        await dbContext.SaveChangesAsync();
    }
}