#pragma warning disable CS1591
namespace PlatipusWallet.Api.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Results.Common.Result.WithData;
using Results.External;
using Results.External.ActionResults;
using Results.External.Enums;

public class ActionResultFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is not ExternalActionResult actionResult)
        {
            return;
        }

        if (actionResult.Result.IsSuccess)
        {
            if (actionResult.Result is IResult<object> objectResult)
            {
                context.Result = new OkObjectResult(objectResult.Data);
                return;
            }

            context.Result = new OkObjectResult(new BaseResponse(Status.Ok));
            return;
        }

        if (actionResult.Result.IsFailure)
        {
            var services = context.HttpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<ActionResultFilterAttribute>>();

            logger.LogWarning("Request failed with ErrorCode: {ErrorCode}", actionResult.Result.ErrorCode);
            
            var errorCode = ((int) actionResult.Result.ErrorCode).ToString();

            var stringLocalizer = services.GetRequiredService<IStringLocalizer<IResult>>();

            var description = actionResult.Result.ErrorCode.ToString();
            var errorResponse = new ErrorResponse(
                Status.Error,
                errorCode,
                stringLocalizer[errorCode]);

            context.Result = new OkObjectResult(errorResponse);
        }
    }
}