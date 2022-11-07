namespace Platipus.Wallet.Api.Filters;

using Application.Requests.Base.Responses;
using Application.Requests.Base.Responses.Databet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Results.Common;
using Results.External;
using Results.External.ActionResults;
using Results.External.Enums;

public class ActionResultFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Cancel)
            return;

        if (context.Result is ExternalActionResult actionResult)
        {
            if (actionResult.Result.IsSuccess)
            {
                if (actionResult.Result is IResult<object> objectResult)
                {
                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                var baseResponse = new BaseResponse(Status.OK);
                context.Result = new OkObjectResult(baseResponse);
                return;
            }

            var services = context.HttpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<ActionResultFilterAttribute>>();

            // logger.LogWarning("Request failed with ErrorCode: {ErrorCode}", actionResult.Result.ErrorCode);
            // var stringLocalizer = services.GetRequiredService<IStringLocalizer<IResult>>(); //TODO

            var errorCode = actionResult.Result.ErrorCode;

            var errorResponse = new ErrorResponse(
                Status.ERROR,
                (int) errorCode,
                errorCode.ToString());

            context.Result = new OkObjectResult(errorResponse);
        }

        if (context.Result is ExternalActionDatabetResult actionDatabetResult)
        {
            if (actionDatabetResult.Result.IsSuccess)
            {
                if (actionDatabetResult.Result is IDatabetResult<object> objectResult)
                {
                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                const DatabetErrorCode databetErrorCode = DatabetErrorCode.Success;
                var databetBaseResponse = new DatabetBaseResponse(databetErrorCode, databetErrorCode.ToString());
                context.Result = new OkObjectResult(databetBaseResponse);
                return;
            }

            var services = context.HttpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<ActionResultFilterAttribute>>();

            // logger.LogWarning("Request failed with ErrorCode: {ErrorCode}", actionDatabetResult.Result.ErrorCode);
            // var stringLocalizer = services.GetRequiredService<IStringLocalizer<IResult>>(); //TODO

            var errorCode = actionDatabetResult.Result.ErrorCode;

            var errorResponse = new DatabetErrorResponse(
                (int) errorCode,
                errorCode.ToString());

            context.Result = new OkObjectResult(errorResponse);
            
            context.HttpContext.Items.Add("response", errorResponse);
        }
    }
}