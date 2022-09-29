namespace PlatipusWallet.Api.Application.Behaviors;

using Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Results.Common;
using Results.Common.Result;

public class ExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class, IResult
{
    private readonly ILogger<ExceptionBehavior<TRequest, TResponse>> _logger;

    public ExceptionBehavior(ILogger<ExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Request {@Request} was handled with unexpected error", request);

            return DynamicResultFactory.CreateFailureResult<TResponse>(ErrorCode.Unknown, e);
        }
    }
}