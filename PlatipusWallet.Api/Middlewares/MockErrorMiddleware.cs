namespace PlatipusWallet.Api.Middlewares;

using Application.Requests.Base;
using Results.External.Enums;

public class MockErrorMiddleware : IMiddleware
{
    private readonly BaseResponse _errorResponse = new(Status.Error);

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {

    }
}