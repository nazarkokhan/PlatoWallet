namespace PlatipusWallet.Api.Filters;

using Application.Requests.Base.Responses;
using Application.Requests.Base.Responses.Databet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Results.Common;
using Results.Common.Result;
using Results.Common.Result.WithData;
using Results.External;
using Results.External.ActionResults;
using Results.External.Enums;

public class ActionResultFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Cancel || context.Result is not ExternalActionResult<object> actionResult)
            return;

        if (actionResult.Result.IsSuccess)
        {
            if (actionResult.Result is IBaseResult<object, object> objectResult)
                // if (actionResult.Result is IResult<object> objectResult)
            {
                context.Result = new OkObjectResult(objectResult.Data);
                return;
            }

            if (actionResult.Result is IDatabetResult)
            {
                const DatabetErrorCode databetErrorCode = DatabetErrorCode.Success;
                context.Result = new OkObjectResult(new DatabetBaseResponse(databetErrorCode, databetErrorCode.ToString()));
                return;
            }

            context.Result = new OkObjectResult(new BaseResponse(Status.OK));
            return;
        }

        var services = context.HttpContext.RequestServices;
        var logger = services.GetRequiredService<ILogger<ActionResultFilterAttribute>>();

        logger.LogWarning("Request failed with ErrorCode: {ErrorCode}", actionResult.Result.ErrorCode);
        // var stringLocalizer = services.GetRequiredService<IStringLocalizer<IResult>>(); //TODO

        if (actionResult.Result is IDatabetResult)
        {
            var errorCode = (int) actionResult.Result.ErrorCode;

            var errorResponse = new DatabetErrorResponse(
                (DatabetErrorCode) errorCode,
                errorCode.ToString());

            context.Result = new OkObjectResult(errorResponse);
        }
        else
        {
            var errorCode = (int) actionResult.Result.ErrorCode;

            var errorResponse = new ErrorResponse(
                Status.ERROR,
                errorCode,
                errorCode.ToString());

            context.Result = new OkObjectResult(errorResponse);
        }
    }
}