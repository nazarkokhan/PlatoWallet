namespace PlatipusWallet.Api.StartupSettings.Middlewares;

using Application.Requests.Base.Responses;
using PlatipusWallet.Api.Results.External.Enums;

public class MockErrorMiddleware : IMiddleware
{
    private readonly BaseResponse _errorResponse = new(Status.ERROR);

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {

    }
}