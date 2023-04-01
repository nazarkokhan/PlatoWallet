namespace Platipus.Wallet.Api.Application.Extensions;

using Results.Base;
using Results.Base.WithData;

public static class DynamicResultFactory
{
    private static readonly int InterfaceGenericResultArgumentsCount = typeof(IBaseResult<>).GetGenericArguments().Length;
    private static readonly Type GenericResultType = typeof(BaseResult<,>);

    public static TResponse CreateFailureResult<TResponse>(
        Exception? exception = null)
        where TResponse : class
    {
        var responseType = typeof(TResponse);

        //TODO
        // var result = responseType switch
        // {
        //     IReevoResult => ReevoResultFactory.Failure(ReevoErrorCode.InternalError)
        // };
        if (responseType.IsGenericType && responseType.GenericTypeArguments.Length == InterfaceGenericResultArgumentsCount)
        {
            var genericArguments = responseType.GenericTypeArguments;
            var genericResultType = GenericResultType.MakeGenericType(genericArguments);

            return (Activator.CreateInstance(
                genericResultType,
                new ErrorCode(),
                exception) as TResponse)!;
        }

        return BaseResultFactory.Failure<TResponse>(default, exception) as TResponse ?? throw new InvalidOperationException();
    }

    public static TResponse CreateSuccessResult<TResponse>()
        where TResponse : class, IBaseResult
    {
        return BaseResultFactory.Success<ErrorCode>() as TResponse ?? throw new InvalidOperationException();
    }
}