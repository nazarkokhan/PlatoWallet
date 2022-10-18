namespace PlatipusWallet.Api.Application.Extensions;

using Results.Common;
using Results.Common.Result;
using Results.Common.Result.Factories;
using Results.Common.Result.WithData;

public static class DynamicResultFactory
{
    private static readonly int InterfaceGenericResultArgumentsCount = typeof(IResult<>).GetGenericArguments().Length;
    private static readonly Type GenericResultType = typeof(Result<>);

    public static TResponse CreateFailureResult<TResponse>(
        ErrorCode errorCode,
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

        return ResultFactory.Failure<TResponse>(errorCode, exception) as TResponse ?? throw new InvalidOperationException();
    }

    public static TResponse CreateSuccessResult<TResponse>()
        where TResponse : class, IResult
    {
        return ResultFactory.Success() as TResponse ?? throw new InvalidOperationException();
    }
}