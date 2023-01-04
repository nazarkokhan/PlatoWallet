namespace Platipus.Wallet.Api.StartupSettings.Middlewares;

using Application.Requests.Base.Common;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Application.Requests.Wallets.Everymatrix.Base.Response;
using Application.Requests.Wallets.Psw.Base.Response;
using Application.Results.Everymatrix;

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

        context.Response.StatusCode = 200;
        var unexpectedErrorResponseBody = context.Request.Path.Value?.Replace("wallet/", "") switch
        {
            "databet" => GetDatabetErrorResponse(),
            //TODO "openbox" => GetDatabetErrorResponse(),
            "psw" => GetPswErrorResponse(),
            "everymatrix" => GetEveryMatrixErrorResponse(),
            _ => GetCommonErrorResponse(context)
        };

        await context.Response.WriteAsJsonAsync(unexpectedErrorResponseBody);

        _logger.LogInformation("Returning unexpected error {UnexpectedErrorResponseBody}", unexpectedErrorResponseBody);
    }

    private static object GetCommonErrorResponse(HttpContext context)
    {
        context.Response.StatusCode = 400;

        const ErrorCode errorCode = ErrorCode.Unknown;
        return new CommonErrorResponse(errorCode);
    }

    private static object GetPswErrorResponse()
    {
        const PswErrorCode errorCode = PswErrorCode.Unknown;
        return new PswErrorResponse(PswStatus.ERROR, (int)errorCode, errorCode.ToString());
    }

    private static object GetDatabetErrorResponse()
    {
        const DafabetErrorCode errorCode = DafabetErrorCode.SystemError;
        return new DafabetErrorResponse((int)errorCode, errorCode.ToString());
    }

    private static object GetEveryMatrixErrorResponse()
    {
        const EverymatrixErrorCode errorCode = EverymatrixErrorCode.UnknownError;

        return new EverymatrixErrorResponse(
            Status: "Failed",
            ErrorCode: $"{(int)errorCode}",
            Description: errorCode.ToString());
    }
}