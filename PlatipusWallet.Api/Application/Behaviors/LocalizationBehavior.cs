namespace PlatipusWallet.Api.Application.Behaviors;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Localization;

public class LocalizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class, IResult
{
    private readonly IStringLocalizer<TRequest> _stringLocalizer;

    public LocalizationBehavior(IStringLocalizer<TRequest> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var response = await next();

        if (response.IsSuccess)
            return response;

        // var errorCode = ((int) response.ErrorCode).ToString(); //TODO
        //
        // if (string.IsNullOrEmpty(response.ErrorDescription))
        //     // response.ErrorDescription = _stringLocalizer[errorCode].Value;
        //     response.ErrorDescription = response.ErrorCode.ToString();

        return response;
    }
}