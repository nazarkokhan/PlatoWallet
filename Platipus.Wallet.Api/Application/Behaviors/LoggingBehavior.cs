namespace Platipus.Wallet.Api.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class, IPswResult
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started handling request: {@Request}", request);

        var result = await next();

        if (result.IsFailure)
            _logger.LogWarning(result.Exception, "Failure request handling. Result: {@Result}", result);
        else
            _logger.LogInformation("Successful request handling. Result: {@Result}", result);

        return result;
    }
}