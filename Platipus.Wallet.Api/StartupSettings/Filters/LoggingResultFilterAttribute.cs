namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Application.Requests.Wallets.Psw.Base.Response;
using Controllers;
using Controllers.Other;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class LoggingResultFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuted(ResultExecutedContext context)
    {
        var httpContext = context.HttpContext;

        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<MockedErrorActionFilterAttribute>>();

        var rawRequestBytes = (byte[])httpContext.Items["rawRequestBytes"]!;
        var request = httpContext.Items["request"];
        var response = (context.Result as OkObjectResult)?.Value;

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
            WalletEveryMatrixController => CasinoProvider.Everymatrix.ToString(),
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

        // logger.LogInformation();
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