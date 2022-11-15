namespace Platipus.Wallet.Api.Application.Extensions;

using Results.Psw;

public static class DynamicResultFactory
{
    private static readonly int InterfaceGenericResultArgumentsCount = typeof(IPswResult<>).GetGenericArguments().Length;
    private static readonly Type GenericResultType = typeof(PswResult<>);

    public static TResponse CreateFailureResult<TResponse>(
        PswErrorCode errorCode,
        Exception? exception = null)
        where TResponse : class
    {
        var responseType = typeof(TResponse);

        if (responseType.IsGenericType && responseType.GenericTypeArguments.Length == InterfaceGenericResultArgumentsCount)
        {
            var genericArguments = responseType.GenericTypeArguments;
            var genericResultType = GenericResultType.MakeGenericType(genericArguments);

            return (Activator.CreateInstance(
                genericResultType,
                errorCode,
                exception) as TResponse)!;
        }

        return PswResultFactory.Failure<TResponse>(errorCode, exception) as TResponse ?? throw new InvalidOperationException();
    }

    public static TResponse CreateSuccessResult<TResponse>()
        where TResponse : class, IPswResult
    {
        return PswResultFactory.Success() as TResponse ?? throw new InvalidOperationException();
    }
}