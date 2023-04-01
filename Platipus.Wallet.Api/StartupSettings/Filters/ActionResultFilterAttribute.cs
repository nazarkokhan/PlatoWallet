namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.Net.Mime;
using System.Text.Json;
using ActionResults;
using Api.Extensions.SecuritySign;
using Application.Requests.Base.Common;
using Application.Requests.Wallets.Betflag.Base;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Application.Requests.Wallets.Everymatrix.Base.Response;
using Application.Requests.Wallets.Hub88.Base.Response;
using Application.Requests.Wallets.Openbox.Base.Response;
using Application.Requests.Wallets.Psw.Base.Response;
using Application.Requests.Wallets.Reevo.Base;
using Application.Requests.Wallets.SoftBet.Base.Response;
using Application.Requests.Wallets.Softswiss.Base;
using Application.Requests.Wallets.Sw.Base.Response;
using Application.Requests.Wallets.Uis.Base;
using Application.Requests.Wallets.Uis.Base.Response;
using Application.Results.Betflag.WithData;
using Application.Results.Everymatrix.WithData;
using Application.Results.Hub88;
using Application.Results.Hub88.WithData;
using Application.Results.ISoftBet;
using Application.Results.ISoftBet.WithData;
using Application.Results.Reevo.WithData;
using Application.Results.Sw;
using Application.Results.Sw.WithData;
using Application.Results.Uis.WithData;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

public class ActionResultFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        const string responseItemsKey = "response";
        if (context.Cancel)
            return;

        var httpContext = context.HttpContext;
        var services = httpContext.RequestServices;
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

            if (baseExternalActionResult.Result is ISoftBetResult softBetResult)
            {
                if (softBetResult.IsSuccess)
                {
                    if (softBetResult is not ISoftBetResult<object> objectResult)
                        return;

                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                var errorCode = softBetResult.ErrorCode;

                var errorResponse = new SoftBetErrorResponse(
                    errorCode.ToCode(),
                    errorCode.ToString(),
                    "action",
                    true);

                context.Result = new BadRequestObjectResult(errorResponse);

                context.HttpContext.Items.Add(responseItemsKey, errorResponse);
            }

            if (baseExternalActionResult.Result is IUisResult<object> uisResult)
            {
                if (uisResult.IsSuccess)
                {
                    context.Result = new OkObjectResult(uisResult.Data)
                    {
                        ContentTypes = new MediaTypeCollection { MediaTypeNames.Application.Xml }
                    };
                    return;
                }

                var container = new UisResponseContainer
                {
                    Request = httpContext.Items[HttpContextItems.RequestObject]!,
                    Time = DateTime.UtcNow,
                    Response = new UisErrorResponse(uisResult.ErrorCode)
                };

                context.Result = new OkObjectResult(container)
                {
                    ContentTypes = new MediaTypeCollection { MediaTypeNames.Application.Xml }
                };

                context.HttpContext.Items.Add(responseItemsKey, container);
            }

            if (baseExternalActionResult.Result is IBetflagResult<object> betflagResult)
            {
                var errorCode = betflagResult.ErrorCode;
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var secretKey = (string?)httpContext.Items[HttpContextItems.BetflagCasinoSecretKey];

                if (betflagResult.IsSuccess)
                {
                    var data = (BetflagBaseResponse)betflagResult.Data;
                    data.Hash = BetflagSecurityHash.Compute(data.Result.ToString(), timestamp, secretKey!);
                    data.Timestamp = timestamp;
                    context.Result = new OkObjectResult(data);
                    return;
                }

                var hash = secretKey is not null
                    ? BetflagSecurityHash.Compute(((int)errorCode).ToString(), timestamp, secretKey)
                    : string.Empty;

                var errorResponse = new BetflagErrorResponse(
                    errorCode,
                    timestamp,
                    hash);

                context.Result = new OkObjectResult(errorResponse);

                context.HttpContext.Items.Add(responseItemsKey, errorResponse);
            }

            if (baseExternalActionResult.Result is IReevoResult<object> reevoResult)
            {
                if (reevoResult.IsSuccess)
                {
                    context.Result = new OkObjectResult(reevoResult.Data);
                    return;
                }

                var errorCode = reevoResult.ErrorCode;

                var errorResponse = new ReevoErrorResponse(errorCode);

                context.Result = new OkObjectResult(errorResponse);

                context.HttpContext.Items.Add(responseItemsKey, errorResponse);
            }

            if (baseExternalActionResult.Result is IEverymatrixResult<object> everymatrixResult)
            {
                if (everymatrixResult.IsSuccess)
                {
                    context.Result = new OkObjectResult(everymatrixResult.Data);
                    return;
                }

                var errorCode = everymatrixResult.ErrorCode;

                var errorResponse = new EverymatrixErrorResponse(errorCode);

                context.Result = new OkObjectResult(errorResponse);

                context.HttpContext.Items.Add(responseItemsKey, errorResponse);
            }
        }

        if (context.Result is PswExternalActionResult { Result: { } pswActionResult })
        {
            if (pswActionResult.IsSuccess)
            {
                if (pswActionResult is IPswResult<object> objectResult)
                {
                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                var baseResponse = new PswBaseResponse(PswStatus.OK);
                context.Result = new OkObjectResult(baseResponse);
                return;
            }

            var errorCode = pswActionResult.ErrorCode;

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
                var databetBaseResponse = new DafabetBaseResponse(databetErrorCode, databetErrorCode.ToString());
                context.Result = new OkObjectResult(databetBaseResponse);
                return;
            }

            var errorCode = dafabetActionResult.Result.ErrorCode;

            var errorResponse = new DafabetErrorResponse((int)errorCode, errorCode.ToString());

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

            context.Result = new BadRequestObjectResult(errorResponse) { StatusCode = statusCode };

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