namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.Text;
using System.Text.Json;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Application.Requests.Wallets.Psw.Base.Response;
using Controllers;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class LoggingResultFilterAttribute : ResultFilterAttribute
{
    private readonly ILogger<LoggingResultFilterAttribute> _logger;

    public LoggingResultFilterAttribute(ILogger<LoggingResultFilterAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnResultExecuted(ResultExecutedContext context)
    {
        var httpContext = context.HttpContext;

        var rawRequestBytes = httpContext.GetRequestBodyBytesItem();
        var request = httpContext.Items[HttpContextItems.RequestObject];
        // var objectResult = context.Result as ObjectResult; //TODO
        var response = (context.Result as ObjectResult)?.Value;

        var requestHeaders = httpContext.Request.Headers.ToDictionary(x => x.Key, x => x.Value);
        var responseHeaders = httpContext.Response.Headers.ToDictionary(x => x.Key, x => x.Value);

        var isError = response is PswErrorResponse or DafabetErrorResponse;

        var provider = context.Controller switch
        {
            WalletPswController => CasinoProvider.Psw.ToString(),
            WalletDafabetController => CasinoProvider.Dafabet.ToString(),
            WalletOpenboxController => CasinoProvider.Openbox.ToString(),
            WalletHub88Controller => CasinoProvider.Hub88.ToString(),
            WalletSoftswissController => CasinoProvider.Softswiss.ToString(),
            WalletSwController => CasinoProvider.Sw.ToString(),
            WalletISoftBetController => CasinoProvider.SoftBet.ToString(),
            WalletGamesGlobalController => CasinoProvider.GamesGlobal.ToString(),
            WalletUisController => CasinoProvider.Uis.ToString(),
            WalletBetflagController => CasinoProvider.Betflag.ToString(),
            WalletReevoController => CasinoProvider.Reevo.ToString(),
            WalletEverymatrixController => CasinoProvider.Everymatrix.ToString(),
            WalletBetConstructController => CasinoProvider.BetConstruct.ToString(),
            _ => "Other"
        };

        if (response is JsonDocument responseJsonNode)
        {
            try
            {
                response = GetConcreteObject(responseJsonNode);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed serializing mocked error {@MockedErrorResponse}", response);
            }
        }

        _logger.Log(
            isError ? LogLevel.Error : LogLevel.Information,
            "Provider: {Provider} \n"
          + "RawRequestBody: {RawRequestBody} \n"
          + "RequestBody: {@RequestBody} \n"
          + "RawResponseBody: {RawResponseBody} \n"
          + "ResponseBody: {@ResponseBody} \n"
          + "RequestHeaders: {@RequestHeaders} \n"
          + "ResponseHeaders: {@ResponseHeaders} \n",
            provider,
            Encoding.UTF8.GetString(rawRequestBytes),
            request,
            JsonSerializer.Serialize(response),
            response,
            requestHeaders,
            responseHeaders);
    }

    private static object? GetConcreteObject(JsonDocument dataJsonNode)
    {
        return ConvertElementToObject(dataJsonNode.RootElement);

        object? ConvertElementToObject(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                {
                    var dic = element.EnumerateObject()
                        .ToDictionary(k => k.Name, v => ConvertElementToObject(v.Value))!
                        .Append(new KeyValuePair<string, object>("$type", "ErrorMock"))
                        .ToDictionary(x => x.Key, x => x.Value);
                    return dic;
                }
                case JsonValueKind.Array:
                    return element.EnumerateArray()
                        .Select(ConvertElementToObject)
                        .ToArray();
                case JsonValueKind.Number:
                    return element.GetDecimal();
                case JsonValueKind.True or JsonValueKind.False:
                    return element.GetBoolean();
                default:
                    return element.GetString();
            }
        }
    }
}