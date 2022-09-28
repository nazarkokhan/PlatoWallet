namespace PlatipusWallet.Api.Filters;

using Application.Requests.Base.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Results.Common.Result.WithData;
using Results.External;
using Results.External.ActionResults;
using Results.External.Enums;

public class ActionResultFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Cancel || context.Result is not ExternalActionResult actionResult)
            return;

        if (actionResult.Result.IsSuccess)
        {
            if (actionResult.Result is IResult<object> objectResult)
            {
                context.Result = new OkObjectResult(objectResult.Data);
                return;
            }

            context.Result = new OkObjectResult(new BaseResponse(Status.OK));
            return;
        }

        var services = context.HttpContext.RequestServices;
        var logger = services.GetRequiredService<ILogger<ActionResultFilterAttribute>>();
        
        logger.LogInformation("{@platipus-test}",
            new {
                messageTemplate = "Request failed with ErrorCode: {ErrorCode}",
                ErrorCode = 132,
                t = 3,
                b = DateTime.Now
            });
        logger.LogWarning("Request failed with ErrorCode: {ErrorCode}", actionResult.Result.ErrorCode);
        logger.LogWarning("Request failed with ErrorCode: {User}, {Age}", actionResult.Result.ErrorCode);

        var errorCode = (int) actionResult.Result.ErrorCode;

        // var stringLocalizer = services.GetRequiredService<IStringLocalizer<IResult>>();

        var description = actionResult.Result.ErrorDescription;
        var errorResponse = new ErrorResponse(
            Status.ERROR,
            errorCode,
            description
            // stringLocalizer[errorCode.ToString()]
        );

        context.Result = new OkObjectResult(errorResponse);
    }
}