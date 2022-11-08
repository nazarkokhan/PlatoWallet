namespace Platipus.Wallet.Api.StartupSettings.Middlewares;

using Application.Requests.Wallets.Dafabet.Base.Response;
using Application.Requests.Wallets.Psw.Base.Response;
using Application.Results.Dafabet;
using Application.Results.Psw;

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

        var unexpectedErrorResponseBody = context.Request.Path.Value?.Replace("wallet/", "") switch
        {
            "databet" => GetDatabetErrorResponse(),
            //TODO "openbox" => GetDatabetErrorResponse(),
            "psw" or _ => GetErrorResponse()
        };

        context.Response.StatusCode = 200; 
        await context.Response.WriteAsJsonAsync(unexpectedErrorResponseBody);

        _logger.LogInformation("Returning unexpected error {UnexpectedErrorResponseBody}", unexpectedErrorResponseBody);
    }

    private object GetErrorResponse()
    {
        const ErrorCode errorCode = ErrorCode.Unknown;
        return new PswErrorResponse(Status.ERROR, (int) errorCode, errorCode.ToString());
    }

    private object GetDatabetErrorResponse()
    {
        const DafabetErrorCode errorCode = DafabetErrorCode.SystemError;
        return new DatabetErrorResponse((int) errorCode, errorCode.ToString());
    }
}