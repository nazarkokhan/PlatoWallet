namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.Text.Json;
using ActionResults;
using Application.Requests.Base.Common;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Application.Requests.Wallets.Hub88.Base.Response;
using Application.Requests.Wallets.Openbox.Base.Response;
using Application.Requests.Wallets.Psw.Base.Response;
using Application.Requests.Wallets.Softswiss.Base;
using Application.Requests.Wallets.Sw.Base.Response;
using Application.Results.Hub88;
using Application.Results.Hub88.WithData;
using Application.Results.Sw;
using Application.Results.Sw.WithData;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

public class ActionResultFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        const string responseItemsKey = "response";
        if (context.Cancel)
            return;

        var services = context.HttpContext.RequestServices;
        var logger = services.GetRequiredService<ILogger<ActionResultFilterAttribute>>();

        if (context.Result is BaseExternalActionResult baseExternalActionResult)
        {
            if (baseExternalActionResult.Result is ISwResult swResult)
            {
                if (swResult.IsSuccess)
                {
                    if (swResult is not ISwResult<object> objectResult)
                        return;

                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                var errorCode = swResult.ErrorCode;

                var errorResponse = new SwErrorResponse(errorCode);

                context.Result = new BadRequestObjectResult(errorResponse);

                context.HttpContext.Items.Add(responseItemsKey, errorResponse);
            }
        }

        if (context.Result is PswExternalActionResult pswActionResult)
        {
            if (pswActionResult.Result.IsSuccess)
            {
                if (pswActionResult.Result is IPswResult<object> objectResult)
                {
                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                var baseResponse = new PswBaseResponse(PswStatus.OK);
                context.Result = new OkObjectResult(baseResponse);
                return;
            }

            var errorCode = pswActionResult.Result.ErrorCode;

            var errorResponse = new PswErrorResponse(PswStatus.ERROR, (int)errorCode, errorCode.ToString());

            context.Result = new OkObjectResult(errorResponse);

            context.HttpContext.Items.Add(responseItemsKey, errorResponse);
        }

        if (context.Result is DafabetExternalActionResult dafabetActionResult)
        {
            if (dafabetActionResult.Result.IsSuccess)
            {
                if (dafabetActionResult.Result is IDafabetResult<object> objectResult)
                {
                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                const DafabetErrorCode databetErrorCode = DafabetErrorCode.Success;
                var databetBaseResponse = new DatabetBaseResponse(databetErrorCode, databetErrorCode.ToString());
                context.Result = new OkObjectResult(databetBaseResponse);
                return;
            }

            var errorCode = dafabetActionResult.Result.ErrorCode;

            var errorResponse = new DatabetErrorResponse((int)errorCode, errorCode.ToString());

            context.Result = new OkObjectResult(errorResponse);

            context.HttpContext.Items.Add(responseItemsKey, errorResponse);
        }

        if (context.Result is OpenboxExternalActionResult openboxActionResult)
        {
            if (openboxActionResult.Result.IsSuccess)
            {
                if (openboxActionResult.Result is IOpenboxResult<object> objectResult)
                {
                    var jsonOptions = services.GetRequiredService<IOptionsMonitor<JsonOptions>>()
                        .Get(CasinoProvider.Openbox.ToString())
                        .JsonSerializerOptions;
                    var payload = JsonSerializer.Serialize(objectResult.Data, jsonOptions);
                    var openboxObjResponse = new OpenboxSingleResponse(payload);
                    context.Result = new OkObjectResult(openboxObjResponse);
                    return;
                }

                var openboxResponse = new OpenboxSingleResponse(string.Empty);
                context.Result = new OkObjectResult(openboxResponse);
                return;
            }

            var errorCode = openboxActionResult.Result.ErrorCode;

            var errorResponse = new OpenboxSingleResponse(errorCode);

            context.Result = new OkObjectResult(errorResponse);

            context.HttpContext.Items.Add(responseItemsKey, errorResponse);
        }

        if (context.Result is Hub88ExternalActionResult hub88ActionResult)
        {
            if (hub88ActionResult.Result.IsSuccess)
            {
                if (hub88ActionResult.Result is IHub88Result<object> objectResult)
                {
                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                logger.LogWarning("We should not get here");
                context.Result = new OkObjectResult(new Hub88ErrorResponse(Hub88ErrorCode.RS_ERROR_UNKNOWN));
                return;
            }

            var errorCode = hub88ActionResult.Result.ErrorCode;

            var errorResponse = new Hub88ErrorResponse(errorCode);

            context.Result = new OkObjectResult(errorResponse);

            context.HttpContext.Items.Add(responseItemsKey, errorResponse);
        }

        if (context.Result is SoftswissExternalActionResult softswissActionResult)
        {
            if (softswissActionResult.Result.IsSuccess)
            {
                if (softswissActionResult.Result is ISoftswissResult<object> objectResult)
                {
                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                context.Result = new OkResult();
                return;
            }

            var errorCode = softswissActionResult.Result.ErrorCode;

            var statusCode = (int)errorCode;
            var balance = softswissActionResult.Result.Balance;

            if (statusCode is not (>= 400 and <= 599))
            {
                statusCode = 400;
                if (balance is null)
                    logger.LogWarning("Balance has to be present");
            }

            var errorResponse = new SoftswissErrorResponse(errorCode, balance);

            context.Result = new BadRequestObjectResult(errorResponse) {StatusCode = statusCode};

            context.HttpContext.Items.Add(responseItemsKey, errorResponse);
        }

        if (context.Result is ExternalActionResult externalActionResult)
        {
            if (externalActionResult.Result.IsSuccess)
            {
                if (externalActionResult.Result is IResult<object> objectResult)
                {
                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                return;
            }

            var errorCode = externalActionResult.Result.ErrorCode;

            var errorResponse = new CommonErrorResponse(errorCode);

            context.Result = new BadRequestObjectResult(errorResponse);

            context.HttpContext.Items.Add(responseItemsKey, errorResponse);
        }
    }
}