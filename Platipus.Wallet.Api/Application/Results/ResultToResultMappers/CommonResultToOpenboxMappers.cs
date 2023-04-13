namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

public static class CommonResultToOpenboxMappers
{
    public static IOpenboxResult<TData> ToOpenboxResult<TData>(this IResult result)
        => result.IsFailure
            ? OpenboxResultFactory.Failure<TData>(result.Error.ToErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IOpenboxResult ToOpenboxResult(this IResult result)
        => result.IsSuccess
            ? OpenboxResultFactory.Success()
            : OpenboxResultFactory.Failure(result.Error.ToErrorCode(), result.Exception);

    private static OpenboxErrorCode ToErrorCode(this ErrorCode source)
    {
        return source switch
        {
            // ErrorCode. => OpenboxErrorCode.Success,
            ErrorCode.SessionNotFound or ErrorCode.SessionExpired => OpenboxErrorCode.TokenRelatedErrors,
            // ErrorCode. => OpenboxErrorCode.ValidationError,
            ErrorCode.BadParametersInTheRequest => OpenboxErrorCode.ParameterError,
            ErrorCode.Unknown or _ => OpenboxErrorCode.InternalServiceError,
        };
    }
}