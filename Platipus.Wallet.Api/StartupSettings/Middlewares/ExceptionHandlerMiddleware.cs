using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;

namespace Platipus.Wallet.Api.StartupSettings.Middlewares;

using Application.Requests.Base.Common;
using Application.Requests.Wallets.Dafabet.Base.Response;
using Application.Requests.Wallets.Everymatrix.Base.Response;
using Application.Requests.Wallets.Psw.Base.Response;
using Application.Requests.Wallets.TODO.PariMatch.Base;
using Application.Results.EmaraPlay;
using Application.Results.Everymatrix;
using Application.Results.PariMatch;

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
            "parimatch" => GetPariMatchErrorResponse(),
            "emaraplay" => GetEmaraPlayErrorResponse(),
            // "betflag" => GetPariBetflagErrorResponse(),
            // "betconstruct" => GetBetConstructErrorResponse(),
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

    private static object GetEmaraPlayErrorResponse()
    {
        const EmaraPlayErrorCode errorCode = EmaraPlayErrorCode.InternalServerError;
        const int code = (int)errorCode;
        return new EmaraPlayErrorResponse(
            code.ToString(),
            errorCode.ToString());
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
            "Failed",
            (int)errorCode,
            errorCode.ToString());
    }

    private static object GetPariMatchErrorResponse()
    {
        return new PariMatchErrorResponse(
            PariMatchErrorCode.ErrorInternal.ToString(),
            "Internal error",
            DateTimeOffset.UtcNow.ToString("yyyy:MM:dd:HH:mm:ss:fff t zzz"));
    }

    // private static object GetPariBetflagErrorResponse()
    // {
    //     var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    //     var hash = BetflagRequestHash.Compute(BetflagErrorCode.GeneralError.ToString(), timeStamp);
    //
    //     return new BetflagErrorResponse(
    //         (int) BetflagErrorCode.GeneralError,
    //         BetflagErrorCode.GeneralError.ToString(),
    //         timeStamp,
    //         hash);
    // }

    // private static object GetBetConstructErrorResponse()
    // {
    //     return new BetconstructErrorResponse(
    //         false,
    //         BetconstructErrorCode.GeneralError.ToString(),
    //         (int)BetconstructErrorCode.GeneralError);
    // }
}