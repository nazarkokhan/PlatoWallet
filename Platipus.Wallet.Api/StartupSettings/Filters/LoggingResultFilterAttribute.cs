namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Application.Requests.Wallets.Psw.Base.Response;
using Controllers;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class LoggingResultFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuted(ResultExecutedContext context)
    {
        var httpContext = context.HttpContext;

        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<LoggingResultFilterAttribute>>();

        var rawRequestBytes = httpContext.GetRequestBodyBytesItem();
        var request = httpContext.Items[HttpContextItems.RequestObject];
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
            _ => "Other"
        };

        if (response is JsonNode responseJsonNode)
        {
            try
            {
                response = responseJsonNode.Deserialize<Dictionary<string, string>>() ?? new Dictionary<string, string>();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed serializing mocked error {@MockedErrorResponse}", response);
            }
        }

        logger.Log(
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
}