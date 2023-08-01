namespace Platipus.Wallet.Api.StartupSettings.Middlewares;

using System.Text;
using System.Text.Json;
using Api.Extensions;
using Domain.Entities.Enums;
using Platipus.Api.Common;

public class LoggingMiddleware : IMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;
    private readonly HashSet<string> _pathsToSkip;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
        _pathsToSkip = new HashSet<string>
        {
            "/",
            "/health",
            "/version",
            "/config",
            "/configname"
        };
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        if (_pathsToSkip.Contains(context.Request.Path))
            return;

        var provider = context.Items[HttpContextItems.Provider] as CasinoProvider?;
        var request = context.Items[HttpContextItems.RequestObject];
        var rawRequestBytes = context.Items[HttpContextItems.RequestBodyBytes] as byte[];
        var rawRequestBody = rawRequestBytes?.Map(b => Encoding.UTF8.GetString(b));
        var requestQueryString = context.Items[HttpContextItems.RequestQueryString];
        var response = context.Items[HttpContextItems.ResponseObject];
        var rawResponseBytes = context.Items[HttpContextItems.ResponseBodyBytes] as byte[];
        var rawResponseBody = rawResponseBytes?.Map(b => Encoding.UTF8.GetString(b));

        // If was replaced by error mock
        if (response is JsonDocument responseJsonDocument)
            response = responseJsonDocument.ToConcreteObject();

        var requestHeaders = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
        var responseHeaders = context.Response.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());

        if (provider is CasinoProvider.Openbox)
        {
            var openboxDecryptedPayloadJsonString = context.Items[HttpContextItems.OpenboxDecryptedPayloadJsonString];
            var openboxDecryptedPayloadRequestObject = context.Items[HttpContextItems.OpenboxDecryptedPayloadRequestObject];

            _logger.LogInformation(
                "Provider: {Provider} \n"
              + "RawRequestBody: {RawRequestBody} \n"
              + "RequestBody: {@RequestBody} \n" //it is RequestObject
              + "OpenboxDecryptedPayloadJsonString: {@OpenboxDecryptedPayloadJsonString} \n"
              + "OpenboxDecryptedPayloadRequestObject: {@OpenboxDecryptedPayloadRequestObject} \n"
              + "RequestQueryString: {@RequestQueryString} \n"
              + "RawResponseBody: {RawResponseBody} \n"
              + "ResponseBody: {@ResponseBody} \n"
              + "RequestHeaders: {@RequestHeaders} \n"
              + "ResponseHeaders: {@ResponseHeaders} \n",
                provider,
                rawRequestBody,
                request,
                openboxDecryptedPayloadJsonString,
                openboxDecryptedPayloadRequestObject,
                requestQueryString,
                rawResponseBody,
                response,
                requestHeaders,
                responseHeaders);
        }
        else
        {
            _logger.LogInformation(
                "Provider: {Provider} \n"
              + "RawRequestBody: {RawRequestBody} \n"
              + "RequestBody: {@RequestBody} \n"
              + "RequestQueryString: {@RequestQueryString} \n"
              + "RawResponseBody: {RawResponseBody} \n"
              + "ResponseBody: {@ResponseBody} \n"
              + "RequestHeaders: {@RequestHeaders} \n"
              + "ResponseHeaders: {@ResponseHeaders} \n",
                provider,
                rawRequestBody,
                request,
                requestQueryString,
                rawResponseBody,
                response,
                requestHeaders,
                responseHeaders);
        }
    }
}