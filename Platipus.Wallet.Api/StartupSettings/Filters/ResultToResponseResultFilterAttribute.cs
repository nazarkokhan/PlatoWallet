using Humanizer;

namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.Net;
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
using Application.Requests.Wallets.Nemesis.Responses;
using Application.Requests.Wallets.Openbox.Base.Response;
using Application.Requests.Wallets.Parimatch.Responses;
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
using Application.Responses.Synot.Base;
using Application.Results.Anakatech.WithData;
using Application.Results.Atlas.WithData;
using Application.Results.BetConstruct.WithData;
using Application.Results.Betflag.WithData;
using Application.Results.Evenbet.WithData;
using Application.Results.Everymatrix.WithData;
using Application.Results.HttpClient.WithData;
using Application.Results.Hub88;
using Application.Results.Hub88.WithData;
using Application.Results.ISoftBet;
using Application.Results.ISoftBet.WithData;
using Application.Results.Nemesis;
using Application.Results.Nemesis.WithData;
using Application.Results.Parimatch;
using Application.Results.Parimatch.WithData;
using Application.Results.Reevo.WithData;
using Application.Results.Sw;
using Application.Results.Sw.WithData;
using Application.Results.Synot;
using Application.Results.Synot.WithData;
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
                    object responseObject;
                    if (uisResult.IsSuccess)
                    {
                        responseObject = uisResult.Data;
                    }
                    else
                    {
                        var requestObject = httpContext.Items[HttpContextItems.RequestObject]!;
                        var errorResponse = new UisErrorResponse(uisResult.Error);

                        responseObject = requestObject switch
                        {
                            UisAuthenticateRequest uisRequest
                                => new UisResponseContainer<UisAuthenticateRequest, UisErrorResponse>(
                                    uisRequest,
                                    errorResponse),
                            UisChangeBalanceRequest uisRequest
                                => new UisResponseContainer<UisChangeBalanceRequest, UisErrorResponse>(
                                    uisRequest,
                                    errorResponse),
                            UisGetBalanceRequest uisRequest
                                => new UisResponseContainer<UisGetBalanceRequest, UisErrorResponse>(
                                    uisRequest,
                                    errorResponse),
                            UisStatusRequest uisRequest
                                => new UisResponseContainer<UisStatusRequest, UisErrorResponse>(uisRequest, errorResponse),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }

                    context.Result = new OkObjectResult(responseObject)
                    {
                        ContentTypes = new MediaTypeCollection { MediaTypeNames.Application.Xml }
                    };

                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, responseObject);
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

                case IUranusResult<object> { IsSuccess: true } uranusResult:
                {
                    context.Result = new OkObjectResult(uranusResult.Data);
                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, uranusResult.Data);
                    return;
                }

                case IUranusResult<object> uranusResult:
                {
                    var errorCode = uranusResult.Error;

                    var errorResponse = new UranusFailureResponse(
                        new UranusCommonErrorResponse(
                            errorCode.Humanize(),
                            errorCode.ToString(),
                            Array.Empty<object>()));

                    context.Result = new OkObjectResult(errorResponse);
                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }

                case IEvenbetResult<object> { IsSuccess: true } evenbetResult:
                {
                    if (ResultAsJavaScript(evenbetResult))
                    {
                        context.Result = new ContentResult
                        {
                            ContentType = "text/html",
                            StatusCode = (int)HttpStatusCode.OK,
                            Content = evenbetResult.Data.ToString()
                        };

                        return;
                    }

                    context.Result = new OkObjectResult(evenbetResult.Data);
                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, evenbetResult.Data);
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

                case IAnakatechResult<object> { IsSuccess: true } anakatechResult:
                {
                    if (ResultAsJavaScript(anakatechResult))
                    {
                        context.Result = new ContentResult
                        {
                            ContentType = "text/html",
                            StatusCode = (int)HttpStatusCode.OK,
                            Content = anakatechResult.Data.ToString()
                        };

                        return;
                    }

                    context.Result = new OkObjectResult(anakatechResult.Data);
                    return;
                }

                case IAnakatechResult<object> anakatechResult:
                {
                    var errorCode = anakatechResult.Error;

                    var errorResponse = new AnakatechErrorResponse(
                        false,
                        0,
                        errorCode.ToString());

                    context.Result = new OkObjectResult(errorResponse);
                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
                    break;
                }

                case INemesisResult nemesisResult:
                {
                    object responseObject;
                    if (nemesisResult.IsSuccess)
                    {
                        if (nemesisResult is not INemesisResult<object> objectResult)
                            return;

                        responseObject = objectResult.Data;

                        if (responseObject is string objectResultDataHtml)
                        {
                            context.Result = new ContentResult
                            {
                                ContentType = "text/html",
                                StatusCode = (int)HttpStatusCode.OK,
                                Content = objectResultDataHtml
                            };

                            return;
                        }

                        context.Result = new OkObjectResult(responseObject);
                    }
                    else
                    {
                        var errorCode = nemesisResult.Error;

                        responseObject = new NemesisErrorResponse(errorCode);

                        context.Result = new ObjectResult(responseObject)
                        {
                            StatusCode = errorCode switch
                            {
                                NemesisErrorCode.SessionExpired
                                 or NemesisErrorCode.SessionNotFound
                                 or NemesisErrorCode.SessionNotActivated
                                 or NemesisErrorCode.SessionIsDeactivated
                                 or NemesisErrorCode.TokenIsInvalid
                                 or NemesisErrorCode.IpAddressUnknown => 401,
                                NemesisErrorCode.ApiDeprecated => 410,
                                NemesisErrorCode.Internal => 500,
                                NemesisErrorCode.NotImplemented => 501,
                                _ => 400
                            }
                        };
                    }

                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, responseObject);
                    return;
                }

                case ISoftswissResult<object> softswissResult:
                {
                    object responseObject;
                    if (softswissResult.IsSuccess)
                    {
                        responseObject = softswissResult.Data;
                        context.Result = new OkObjectResult(responseObject);
                    }
                    else
                    {
                        var errorCode = softswissResult.Error;

                        var statusCode = (int)errorCode;
                        var balance = softswissResult.Balance;

                        if (statusCode is not (>= 400 and <= 599))
                        {
                            statusCode = 400;
                            if (balance is null)
                                logger.LogWarning("Balance has to be present");
                        }

                        responseObject = new SoftswissErrorResponse(errorCode, balance);
                        context.Result = new ObjectResult(responseObject) { StatusCode = statusCode };
                    }

                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, responseObject);
                    return;
                }

                case IPswResult pswResult:
                {
                    object responseObject;
                    if (pswResult.IsSuccess)
                    {
                        if (pswResult is IPswResult<object> objectResult)
                            responseObject = objectResult.Data;
                        else
                            responseObject = new PswBaseResponse(PswStatus.OK);
                    }
                    else
                    {
                        var errorCode = pswResult.Error;
                        responseObject = new PswErrorResponse(PswStatus.ERROR, (int)errorCode, errorCode.ToString());
                    }

                    context.Result = new OkObjectResult(responseObject);
                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, responseObject);
                    return;
                }

                case IParimatchResult parimatchResult:
                {
                    object responseObject;
                    if (parimatchResult.IsSuccess)
                    {
                        if (parimatchResult is IParimatchResult<object> objectResult)
                            responseObject = objectResult.Data;
                        else
                            break;
                    }
                    else
                    {
                        var errorCode = parimatchResult.Error;
                        responseObject = new ParimatchErrorResponse(errorCode);
                    }

                    context.Result = new OkObjectResult(responseObject);
                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, responseObject);
                    return;
                }

                case ISynotResult synotResult:
                {
                    object responseObject;
                    if (synotResult.IsSuccess)
                    {
                        if (synotResult is not ISynotResult<object> objectResult)
                            return;

                        responseObject = objectResult.Data;

                        context.Result = new OkObjectResult(responseObject);
                    }
                    else
                    {
                        var error = synotResult.Error;
                        responseObject = new SynotErrorResponse(error.ToString(), error.Humanize());

                        context.Result = new OkObjectResult(responseObject)
                        {
                            StatusCode = error switch
                            {
                                SynotError.INVALID_API_KEY
                                 or SynotError.INVALID_TOKEN
                                 or SynotError.TOKEN_EXPIRED
                                 or SynotError.BAD_SIGNATURE => 403,
                                SynotError.CASINO_CLOSED => 503,
                                SynotError.UNSPECIFIED => 500,
                                SynotError.GAME_CLOSED
                                 or SynotError.INSUFFICIENT_FUNDS
                                 or SynotError.RESPONSIBLE_GAMING_LIMIT_REACHED
                                 or SynotError.INVALID_GAME_ROUND
                                 or SynotError.GAME_ROUND_CLOSED
                                 or SynotError.INVALID_STAKE
                                 or SynotError.FREEGAMES_INVALID_OFFER
                                 or SynotError.FREEGAMES_INVALID_PLAYER => 400,
                                _ => 400
                            }
                        };
                    }

                    context.HttpContext.Items.Add(HttpContextItems.ResponseObject, responseObject);
                    return;
                }
            }
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
                       .Get(WalletProvider.Openbox.ToString())
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

        if (context.Result is not ExternalActionResult externalActionResult)
            return;

        {
            if (externalActionResult.Result.IsSuccess)
            {
                if (externalActionResult.Result is not IResult<object> objectResult)
                    return;

                if (objectResult.Data is not IHttpClientResult<object, object> httpClientResult)
                {
                    context.Result = new OkObjectResult(objectResult.Data);
                    return;
                }

                var httpResponseObject = new
                {
                    httpClientResult.IsSuccess,
                    ResponseData = httpClientResult.IsSuccess ? httpClientResult.Data : httpClientResult.Error,
                    HttpRequestContext = httpClientResult.HttpRequest,
                    httpClientResult.Error,
                    httpClientResult.Exception
                };

                context.Result = new OkObjectResult(httpResponseObject);
                return;
            }

            var errorCode = externalActionResult.Result.Error;

            var errorResponse = new CommonErrorResponse(errorCode);

            context.Result = new BadRequestObjectResult(errorResponse);

            context.HttpContext.Items.Add(HttpContextItems.ResponseObject, errorResponse);
        }
    }

    private static bool ResultAsJavaScript(dynamic result)
    {
        return result.Data.ToString()!.Contains("<script type='text/javascript'>");
    }
}