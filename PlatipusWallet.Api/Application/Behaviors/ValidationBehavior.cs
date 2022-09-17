// namespace TradePlus.Api.Common.Behaviors;
//
// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using FluentValidation;
// using MediatR;
// using Microsoft.Extensions.DependencyInjection;
// using PlatipusWallet.Api.Application.Behaviors;
//
// public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//     where TRequest : IRequest<TResponse>
//     where TResponse : class, IResult
// {
//     private readonly IServiceScopeFactory _serviceScopeFactory;
//
//     public ValidationBehavior(IServiceScopeFactory serviceScopeFactory)
//     {
//         _serviceScopeFactory = serviceScopeFactory;
//     }
//
//     public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
//     {
//         using var scope = _serviceScopeFactory.CreateScope();
//         var validator = scope.ServiceProvider.GetService<IValidator<TRequest>>();
//
//         if (validator is null)
//             return await next();
//
//         var result = await validator.ValidateAsync(request, cancellationToken);
//
//         if (result.IsValid) 
//             return await next();
//
//         var errors = new Dictionary<string, string>();
//
//         foreach(var error in result.Errors)
//             errors[error.PropertyName] = error.ErrorMessage;
//
//         return DynamicResultFactory.CreateFailureResult<TResponse>(errors, null);
//     }
// }