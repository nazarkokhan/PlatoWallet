namespace Platipus.Wallet.Api.StartupSettings.Middlewares;

using Results.Common;
using Results.External;
using Results.External.Enums;

public class ExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _logger.LogCritical("Unhandled exception occured");

        var unexpectedErrorResponseBody = context.Request.Path.Value switch
        {
            "databet" => GetDatabetErrorResponse(),
            "wallet" or _ => GetErrorResponse()
        };

        context.Response.StatusCode = 200; 
        await context.Response.WriteAsJsonAsync(unexpectedErrorResponseBody);

        _logger.LogInformation("Returning unexpected error {UnexpectedErrorResponseBody}", unexpectedErrorResponseBody);
    }

    private object GetErrorResponse()
    {
        const ErrorCode errorCode = ErrorCode.Unknown;
        return new ErrorResponse(Status.ERROR, (int) errorCode, errorCode.ToString());
    }

    private object GetDatabetErrorResponse()
    {
        const DatabetErrorCode errorCode = DatabetErrorCode.SystemError;
        return new DatabetErrorResponse((int) errorCode, errorCode.ToString());
    }
}