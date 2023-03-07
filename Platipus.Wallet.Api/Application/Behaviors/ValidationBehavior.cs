namespace Platipus.Wallet.Api.Application.Behaviors;

using Extensions;
using FluentValidation;
using Results.Base;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class, IBaseResult
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ValidationBehavior(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var validator = scope.ServiceProvider.GetService<IValidator<TRequest>>();

        if (validator is null)
            return await next();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (result.IsValid)
            return await next();

        return DynamicResultFactory.CreateFailureResult<TResponse>();
    }
}