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
using Results.Common.Result;

public class ErrorMockActionFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Before controller action
        var executedContext = await next();
        // After controller action
        
        var (_, baseRequestObject) = context.ActionArguments.FirstOrDefault(a => a.Value?.GetType().IsAssignableTo(typeof(BaseRequest)) ?? false);
        
        if (baseRequestObject is null)
        {
            executedContext.Result = ResultFactory.Failure(ErrorCode.CouldNotTryToMockSessionError).ToActionResult();
            return;
        }

        var sessionId = ((BaseRequest) baseRequestObject).SessionId;

        var services = executedContext.HttpContext.RequestServices;
        var dbContext = services.GetRequiredService<WalletDbContext>();

        var errorMock = await dbContext.Set<ErrorMock>()
            .Where(e => e.SessionId == sessionId)
            .FirstOrDefaultAsync(executedContext.HttpContext.RequestAborted);

        if (errorMock is null)
            return;
        if (errorMock.MethodPath != executedContext.HttpContext.Request.Path)
            return;

        executedContext.Result = new ObjectResult(errorMock.Body)
        {
            StatusCode = (int?) errorMock.HttpStatusCode,
            ContentTypes = new MediaTypeCollection{MediaTypeNames.Application.Json} // TODO assign from body
        };
    }
}