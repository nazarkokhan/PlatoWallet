namespace Platipus.Wallet.Api.Application.Behaviors;

using Microsoft.Extensions.Localization;
using Results.Base;

public class LocalizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class, IBaseResult
{
    private readonly IStringLocalizer<TRequest> _stringLocalizer;

    public LocalizationBehavior(IStringLocalizer<TRequest> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
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