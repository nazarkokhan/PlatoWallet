namespace Platipus.Wallet.Api.Application.Behaviors;

using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Extensions;
using Results.Common;
using Results.Common.Result;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class, IResult
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ValidationBehavior(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var validator = scope.ServiceProvider.GetService<IValidator<TRequest>>();

        if (validator is null)
            return await next();

        var result = await validator.ValidateAsync(request, cancellationToken);

        if (result.IsValid) 
            return await next();

        return DynamicResultFactory.CreateFailureResult<TResponse>(ErrorCode.BadParametersInTheRequest);
    }
}