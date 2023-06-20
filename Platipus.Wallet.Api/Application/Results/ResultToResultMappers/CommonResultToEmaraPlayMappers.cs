using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;

namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

public static class CommonResultToEmaraPlayMappers
{
    public static IEmaraPlayResult<TData> ToEmaraPlayResult<TData>(this IResult result)
        => result.IsFailure
            ? EmaraPlayResultFactory.Failure<TData>(result.Error.ToEmaraPlayErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IEmaraPlayResult ToEmaraPlayResult(this IResult result)
        => result.IsSuccess
            ? EmaraPlayResultFactory.Success()
            : EmaraPlayResultFactory.Failure(result.Error.ToEmaraPlayErrorCode(), 
                exception: result.Exception);

    private static EmaraPlayErrorCode ToEmaraPlayErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.UserIsDisabled => EmaraPlayErrorCode.GameIsNotFoundOrDisabled,
            ErrorCode.CasinoNotFound => EmaraPlayErrorCode.ProviderNotFound,
            
            ErrorCode.Unknown or _ => EmaraPlayErrorCode.Success
        };
    }
}