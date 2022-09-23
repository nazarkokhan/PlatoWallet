namespace PlatipusWallet.Api.Filters;

using Application.Requests.Base.Requests;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result;

public class ErrorMockActionFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Before controller action
        await next();
        // After controller action

        var (_, baseRequestObject) = context.ActionArguments.FirstOrDefault(a => a.Value?.GetType() == typeof(BaseRequest));

        if (baseRequestObject is null)
        {
            context.Result = ResultFactory.Failure(ErrorCode.CouldNotTryToMockSessionError).ToActionResult();
            return;
        }

        var sessionId = ((BaseRequest) baseRequestObject).SessionId;

        var services = context.HttpContext.RequestServices;
        var dbContext = services.GetRequiredService<WalletDbContext>();

        var errorMock = await dbContext.Set<ErrorMock>()
            .Where(e => e.SessionId == sessionId)
            .FirstOrDefaultAsync(context.HttpContext.RequestAborted);

        if (errorMock is null)
            return;

        if (errorMock.MethodPath != context.HttpContext.Request.Path)
            return;

        context.Result = new JsonResult(errorMock.Body)
        {
            StatusCode = (int?) errorMock.ResponseStatusCode
        };
    }
}