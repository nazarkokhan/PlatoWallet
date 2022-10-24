namespace PlatipusWallet.Api.Filters;

using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class LoggingFilterAttribute : ResultFilterAttribute
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

        logger.LogInformation(
            "RawRequestBody: {RawRequestBody} \n" +
            "RequestBody: {@RequestBody} \n" +
            "ResponseBody: {@ResponseBody} \n" +
            "RequestHeaders: {@RequestHeaders} \n" +
            "ResponseHeaders: {@ResponseHeaders} \n",
            Encoding.UTF8.GetString(rawRequestBytes),
            request,
            response,
            requestHeaders,
            responseHeaders);
    }
}