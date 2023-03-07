namespace Platipus.Wallet.Api.Application.Behaviors;

using Results.Base;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class, IBaseResult
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
        _logger.LogInformation("Start handling Mediatr request --- {@MediatrRequest}", request);

        var result = await next();

        if (result.IsFailure)
            _logger.LogWarning(result.Exception, "Failure handling Mediatr request --- {@MediatrResult}", result);
        else
            _logger.LogInformation("Successful handling Mediatr request --- {@MediatrResult}", result);

        return result;
    }
}