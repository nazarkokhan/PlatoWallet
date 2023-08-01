namespace Platipus.Wallet.Api.StartupSettings.Middlewares;

public class ExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unhandled exception occured");
            context.Response.Body.SetLength(0);
            context.Response.StatusCode = 500;
        }
    }
}