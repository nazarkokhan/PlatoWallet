namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.Text.Json;
using ActionResults;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Application.Requests.Wallets.Openbox.Base.Response;
using Application.Requests.Wallets.Psw.Base.Response;
using Application.Results.Dafabet;
using Application.Results.Psw;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

public class ActionResultFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Cancel)
            return;

        var services = context.HttpContext.RequestServices;
        if (context.Result is PswExternalActionResult actionResult)
        {
            if (actionResult.Result.IsSuccess)
            {
                if (actionResult.Result is IPswResult<object> objectResult)
                {
                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                var baseResponse = new PswBaseResponse(PswStatus.OK);
                context.Result = new OkObjectResult(baseResponse);
                return;
            }

            var logger = services.GetRequiredService<ILogger<ActionResultFilterAttribute>>();

            // logger.LogWarning("Request failed with ErrorCode: {ErrorCode}", actionResult.Result.ErrorCode);
            // var stringLocalizer = services.GetRequiredService<IStringLocalizer<IResult>>(); //TODO

            var errorCode = actionResult.Result.ErrorCode;

            var errorResponse = new PswErrorResponse(PswStatus.ERROR, (int) errorCode, errorCode.ToString());

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

            var logger = services.GetRequiredService<ILogger<ActionResultFilterAttribute>>();

            // logger.LogWarning("Request failed with ErrorCode: {ErrorCode}", actionDatabetResult.Result.ErrorCode);
            // var stringLocalizer = services.GetRequiredService<IStringLocalizer<IResult>>(); //TODO

            var errorCode = actionDatabetResult.Result.ErrorCode;

            var errorResponse = new DatabetErrorResponse((int) errorCode, errorCode.ToString());

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