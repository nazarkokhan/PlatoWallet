namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.Text.Json;
using ActionResults;
using Application.Results.Psw;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Application.Requests.Wallets.Openbox.Base.Response;
using Application.Requests.Wallets.Psw.Base.Response;
using Application.Results.Openbox.WithData;
using Controllers;
using Platipus.Wallet.Api.Application.Results.Dafabet;
using Platipus.Wallet.Api.Application.Results.Dafabet.WithData;
using Platipus.Wallet.Api.Application.Results.Psw.WithData;

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

                var baseResponse = new PswBaseResponse(Status.OK);
                context.Result = new OkObjectResult(baseResponse);
                return;
            }

            var services = context.HttpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<ActionResultFilterAttribute>>();

            // logger.LogWarning("Request failed with ErrorCode: {ErrorCode}", actionResult.Result.ErrorCode);
            // var stringLocalizer = services.GetRequiredService<IStringLocalizer<IResult>>(); //TODO

            var errorCode = actionResult.Result.ErrorCode;

            var errorResponse = new PswErrorResponse(Status.ERROR, (int)errorCode, errorCode.ToString());

            context.Result = new OkObjectResult(errorResponse);
        }

        if (context.Result is DafabetExternalActionResult actionDatabetResult)
        {
            if (actionDatabetResult.Result.IsSuccess)
            {
                if (actionDatabetResult.Result is IDafabetResult<object> objectResult)
                {
                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                const DafabetErrorCode databetErrorCode = DafabetErrorCode.Success;
                var databetBaseResponse = new DatabetBaseResponse(databetErrorCode, databetErrorCode.ToString());
                context.Result = new OkObjectResult(databetBaseResponse);
                return;
            }

            var services = context.HttpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<ActionResultFilterAttribute>>();

            // logger.LogWarning("Request failed with ErrorCode: {ErrorCode}", actionDatabetResult.Result.ErrorCode);
            // var stringLocalizer = services.GetRequiredService<IStringLocalizer<IResult>>(); //TODO

            var errorCode = actionDatabetResult.Result.ErrorCode;

            var errorResponse = new DatabetErrorResponse((int)errorCode, errorCode.ToString());

            context.Result = new OkObjectResult(errorResponse);

            context.HttpContext.Items.Add("response", errorResponse);
        }

        //TODO
        if (context.Result is OpenboxExternalActionResult actionOpenboxResult)
        {
            if (actionOpenboxResult.Result.IsSuccess)
            {
                if (actionOpenboxResult.Result is IOpenboxResult<object> objectResult)
                {
                    var payload = JsonSerializer.Serialize(objectResult.Data, OpenboxSerializer.Value);
                    var openboxObjResponse = new OpenboxSingleResponse(payload);
                    context.Result = new OkObjectResult(openboxObjResponse);
                    return;
                }

                var openboxResponse = new OpenboxSingleResponse(string.Empty);
                context.Result = new OkObjectResult(openboxResponse);
                return;
            }

            var services = context.HttpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<ActionResultFilterAttribute>>();

            // logger.LogWarning("Request failed with ErrorCode: {ErrorCode}", actionDatabetResult.Result.ErrorCode);
            // var stringLocalizer = services.GetRequiredService<IStringLocalizer<IResult>>(); //TODO

            var errorCode = actionOpenboxResult.Result.ErrorCode;

            var errorResponse = new OpenboxSingleResponse(errorCode);

            context.Result = new OkObjectResult(errorResponse);

            context.HttpContext.Items.Add("response", errorResponse);
        }
    }
}