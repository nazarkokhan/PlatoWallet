namespace PlatipusWallet.Api.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class LoggingFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuted(ResultExecutedContext context)
    {
        var httpContext = context.HttpContext;

        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<MockedErrorActionFilterAttribute>>();

        var request = httpContext.Items["request"];
        var response = (context.Result as OkObjectResult)?.Value;

        var requestHeaders = httpContext.Request.Headers.ToDictionary(x => x.Key, x => x.Value);
        var responseHeaders = httpContext.Response.Headers.ToDictionary(x => x.Key, x => x.Value);

        logger.LogInformation(
            "RequestBody: {@RequestBody} \n" +
            "ResponseBody: {@ResponseBody} \n" +
            "RequestHeaders: {@RequestHeaders} \n" +
            "ResponseHeaders: {@ResponseHeaders} \n",
            request,
            response,
            requestHeaders,
            responseHeaders);
    }
}