namespace PlatipusWallet.Api.Filters;

using System.Net.Mime;
using Application.Requests.Base.Requests;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result.Factories;

public class ErrorMockActionFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ErrorMockActionFilterAttribute>>();
        logger.LogInformation("Handling request with possible mocked error");
        
        // Before controller action
        var executedContext = await next();
        // After controller action
        
        var baseRequest = context.ActionArguments.Select(a => a.Value as BaseRequest).SingleOrDefault(a => a is not null);

        if (baseRequest is null)
        {
            logger.LogCritical("Can not mock error for request not assignable to BaseRequest {BaseRequestTypeName}", typeof(BaseRequest).FullName);
            executedContext.Result = ResultFactory.Failure(ErrorCode.CouldNotTryToMockSessionError).ToActionResult();
            return;
        }

        var sessionId = baseRequest.SessionId;

        var services = executedContext.HttpContext.RequestServices;
        var dbContext = services.GetRequiredService<WalletDbContext>();

        var errorMock = await dbContext.Set<ErrorMock>()
            .Where(e => e.SessionId == sessionId)
            .Select(e => new
            {
                e.SessionId,
                e.Body,
                e.MethodPath,
                e.HttpStatusCode
            })
            .FirstOrDefaultAsync(executedContext.HttpContext.RequestAborted);

        if (errorMock is null)
        {
            logger.LogInformation("Error mock not present");
            return;
        }
        if (errorMock.MethodPath != executedContext.HttpContext.Request.Path)
        {
            logger.LogInformation("Error mock request path does not fit {@ErrorMock}", errorMock);
            return;
        }

        logger.LogInformation("Executing error mock {@ErrorMock}", errorMock);
        executedContext.Result = new ObjectResult(errorMock.Body)
        {
            StatusCode = (int?) errorMock.HttpStatusCode,
            ContentTypes = new MediaTypeCollection{MediaTypeNames.Application.Json} // TODO assign from body
        };
    }
}