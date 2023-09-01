namespace Platipus.Wallet.Api.Application.Requests.Wallets.Parimatch.Responses;

using Humanizer;
using JetBrains.Annotations;
using Platipus.Wallet.Api.Application.Results.Nemesis;

[PublicAPI]
public class ParimatchErrorResponse
{
    public ParimatchErrorResponse(NemesisErrorCode errorCode)
    {
        Code = errorCode.Humanize();
        Message = errorCode.ToString().Underscore().Replace('_', '.');
        At = DateTimeOffset.UtcNow;
    }

    public string Code { get; init; }

    public string Message { get; init; }
    public DateTimeOffset At { get; init; }
}