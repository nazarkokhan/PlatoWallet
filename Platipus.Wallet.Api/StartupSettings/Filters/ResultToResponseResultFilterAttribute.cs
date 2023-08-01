using Humanizer;

namespace Platipus.Wallet.Api.StartupSettings.Filters;

using Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using System.Net.Mime;
using System.Text.Json;
using ActionResults;
using Api.Extensions.SecuritySign;
using Application.Requests.Base.Common;
using Application.Requests.Wallets.Atlas.Base;
using Application.Requests.Wallets.BetConstruct.Base.Response;
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
using Application.Requests.Wallets.Uis;
using Application.Requests.Wallets.Uis.Base.Response;
using Application.Requests.Wallets.Uranus.Base;
using Application.Responses.Anakatech.Base;
using Application.Responses.Evenbet.Base;
using Application.Results.Anakatech.WithData;
using Application.Results.Atlas.WithData;
using Application.Results.BetConstruct.WithData;
using Application.Results.Betflag.WithData;
using Application.Results.Evenbet.WithData;
using Application.Results.Everymatrix.WithData;
using Application.Results.Hub88;
using Application.Results.Hub88.WithData;
using Application.Results.ISoftBet;
using Application.Results.ISoftBet.WithData;
using Application.Results.Reevo.WithData;
using Application.Results.Sw;
using Application.Results.Sw.WithData;
using Application.Results.Uis.WithData;
using Application.Results.Uranus.WithData;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

public sealed class ResultToResponseResultFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Cancel)
            return;

        var httpContext = context.HttpContext;
        var services = httpContext.RequestServices;
        var logger = services.GetRequiredService<ILogger<ResultToResponseResultFilterAttribute>>();

        if (context.Result is BaseExternalActionResult baseExternalActionResult)
        {
            switch (baseExternalActionResult.Result)
            {
                case ISwResult { IsSuccess: true } swResult:
                {
                    if (swResult is not ISwResult<object> objectResult)
                        return;

                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                case ISwResult swResult:
                {
                    var errorCode = swResult.Error;

                    var errorResponse = new SwErrorResponse(errorCode);

                    context.Result = new BadRequestObjectResult(errorResponse);

                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }

                //TODO why ResponseObject not added?
                //TODO why split on two switch cases?
                case ISoftBetResult { IsSuccess: true } softBetResult:
                {
                    if (softBetResult is not ISoftBetResult<object> objectResult)
                        return;

                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                case ISoftBetResult softBetResult:
                {
                    var errorCode = softBetResult.Error;

                    var errorResponse = new SoftBetErrorResponse(
                        errorCode.ToCode(),
                        errorCode.ToString(),
                        "action",
                        true);

                    context.Result = new BadRequestObjectResult(errorResponse);

                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }

                // TODO use one case for one provider everywhere as i made here!
                case IUisResult<object> uisResult:
                {
                    if (uisResult.IsSuccess)
                    {
                        context.Result = new OkObjectResult(uisResult.Data)
                        {
                            ContentTypes = new MediaTypeCollection { MediaTypeNames.Application.Xml }
                        };
                        break;
                    }

                    var requestObject = httpContext.Items[HttpContextItems.RequestObject]!;
                    var responseObject = new UisErrorResponse(uisResult.Error);

                    object container = requestObject switch
                    {
                        UisAuthenticateRequest uisRequest
                            => new UisResponseContainer<UisAuthenticateRequest, UisErrorResponse>(
                                uisRequest,
                                responseObject),
                        UisChangeBalanceRequest uisRequest
                            => new UisResponseContainer<UisChangeBalanceRequest, UisErrorResponse>(
                                uisRequest,
                                responseObject),
                        UisGetBalanceRequest uisRequest
                            => new UisResponseContainer<UisGetBalanceRequest, UisErrorResponse>(uisRequest, responseObject),
                        UisStatusRequest uisRequest
                            => new UisResponseContainer<UisStatusRequest, UisErrorResponse>(uisRequest, responseObject),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    context.Result = new OkObjectResult(container)
                    {
                        ContentTypes = new MediaTypeCollection { MediaTypeNames.Application.Xml }
                    };

                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, container);
                    break;
                }

                case IBetflagResult<object> betflagResult:
                {
                    var errorCode = betflagResult.Error;
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

                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }

                case IReevoResult<object> { IsSuccess: true } reevoResult:
                    context.Result = new OkObjectResult(reevoResult.Data);
                    return;

                case IReevoResult<object> reevoResult:
                {
                    var errorCode = reevoResult.Error;

                    var errorResponse = new ReevoErrorResponse(errorCode);

                    context.Result = new OkObjectResult(errorResponse);

                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }

                case IEverymatrixResult<object> { IsSuccess: true } everymatrixResult:
                    context.Result = new OkObjectResult(everymatrixResult.Data);
                    return;

                case IEverymatrixResult<object> everymatrixResult:
                {
                    var errorCode = everymatrixResult.Error;

                    var errorResponse = new EverymatrixErrorResponse(errorCode);

                    context.Result = new OkObjectResult(errorResponse);

                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }

                //TODO fix
                case IBetconstructResult<object> { IsSuccess: true } betConstructResult:
                    context.Result = new OkObjectResult(betConstructResult.Data);
                    return;

                case IBetconstructResult<object> betConstructResult:
                {
                    var errorCode = betConstructResult.Error;

                    var errorResponse = new BetconstructErrorResponse(errorCode);

                    context.Result = new OkObjectResult(errorResponse);

                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }

                case IEmaraPlayResult<object> { IsSuccess: true } emaraPlayResult:
                    context.Result = new OkObjectResult(emaraPlayResult.Data);
                    return;

                case IEmaraPlayResult<object> emaraPlayResult:
                {
                    var errorCode = emaraPlayResult.Error;

                    var errorResponse = new EmaraPlayErrorResponse(errorCode);

                    context.Result = new OkObjectResult(errorResponse);
                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }

                case IAtlasResult<object> { IsSuccess: true } atlasPlatformResult:
                    context.Result = new OkObjectResult(atlasPlatformResult.Data);
                    return;

                case IAtlasResult<object> atlasPlatformResult:
                {
                    var errorCode = atlasPlatformResult.Error;

                    var errorResponse = new AtlasErrorResponse(errorCode.Humanize(), (int)errorCode);

                    context.Result = new OkObjectResult(errorResponse);
                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }

                case IUranusResult<object> { IsSuccess: true } evoplayResult:
                {
                    context.Result = new OkObjectResult(evoplayResult.Data);
                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, evoplayResult.Data);
                    return;
                }

                //TODO why split switch
                case IUranusResult<object> evoplayResult:
                {
                    var errorCode = evoplayResult.Error;

                    var errorResponse = new UranusFailureResponse(
                        new UranusCommonErrorResponse(
                            errorCode.Humanize(),
                            errorCode.ToString(),
                            Array.Empty<object>()));

                    context.Result = new OkObjectResult(errorResponse);
                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }

                //TODO what is going to happen when result is failure?
                case IEvenbetResult<object> { IsSuccess: true } evenbetResult:
                {
                    context.Result = new OkObjectResult(evenbetResult.Data);

                    //TODO why dont you add HttpContext.Items? It is used for logging!
                    return;
                }

                case IEvenbetResult<object> evenbetResult:
                {
                    var errorCode = evenbetResult.Error;

                    var errorResponse = new EvenbetFailureResponse(
                        new EvenbetErrorResponse(
                            (int)errorCode,
                            errorCode.Humanize()));

                    context.Result = new OkObjectResult(errorResponse);
                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }

                case IAnakatechResult<object> { IsSuccess: true } evenbetResult:
                    context.Result = new OkObjectResult(evenbetResult.Data);
                    return;

                case IAnakatechResult<object> evenbetResult:
                {
                    var errorCode = evenbetResult.Error;

                    var errorResponse = new AnakatechErrorResponse(
                        false,
                        0,
                        errorCode.ToString());

                    context.Result = new OkObjectResult(errorResponse);
                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }
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

            var errorCode = pswActionResult.Error;

            var errorResponse = new PswErrorResponse(PswStatus.ERROR, (int)errorCode, errorCode.ToString());

            context.Result = new OkObjectResult(errorResponse);

            context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
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

            var errorCode = dafabetActionResult.Result.Error;

            var errorResponse = new DafabetErrorResponse((int)errorCode, errorCode.ToString());

            context.Result = new OkObjectResult(errorResponse);

            context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
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

            var errorCode = openboxActionResult.Result.Error;

            var errorResponse = new OpenboxSingleResponse(errorCode);

            context.Result = new OkObjectResult(errorResponse);

            context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
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

            var errorCode = hub88ActionResult.Result.Error;

            var errorResponse = new Hub88ErrorResponse(errorCode);

            context.Result = new OkObjectResult(errorResponse);

            context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
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

            var errorCode = softswissActionResult.Result.Error;

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

            context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
        }

        if (context.Result is not ExternalActionResult externalActionResult)
            return;

        {
            if (externalActionResult.Result.IsSuccess)
            {
                if (externalActionResult.Result is not IResult<object> objectResult)
                    return;

                context.Result = new OkObjectResult(objectResult.Data);
                return;
            }

            var errorCode = externalActionResult.Result.Error;

            var errorResponse = new CommonErrorResponse(errorCode);

            context.Result = new BadRequestObjectResult(errorResponse);

            context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
        }
    }
}