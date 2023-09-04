namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch.Responses;

using Humanizer;
using JetBrains.Annotations;
using Platipus.Wallet.Api.Application.Results.Parimatch;

[PublicAPI]
public class ParimatchErrorResponse
{
    public ParimatchErrorResponse(ParimatchErrorCode errorCode)
    {
        Code = errorCode.Humanize();
        Message = errorCode.ToString().Underscore().Replace('_', '.');
        At = DateTimeOffset.UtcNow;
    }

    public string Code { get; init; }

    public string Message { get; init; }
    public DateTimeOffset At { get; init; }
}