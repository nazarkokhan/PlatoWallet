namespace PlatipusWallet.Api.Application.Behaviors;

using System.Threading;
using System.Threading.Tasks;
using Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class, IResult
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var requestName = request.GetTypeName();

        _logger.LogInformation("Start handling request {RequestName}: {@Request}", requestName, request);

        var result = await next();

        if (result.IsFailure)
            _logger.LogWarning(result.Exception, "Failure request handling {RequestName}. Result: {@Result}", requestName, result);
        else
            _logger.LogInformation("Successful request handling {RequestName} ", requestName);

        return result;
    }
}