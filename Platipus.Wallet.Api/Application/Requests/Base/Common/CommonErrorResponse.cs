namespace Platipus.Wallet.Api.Application.Requests.Base.Common;

using Humanizer;
using JetBrains.Annotations;
using Results.Common;

[PublicAPI]
public record CommonErrorResponse(int ErrorCode, string ErrorMessage)
{
    public CommonErrorResponse(ErrorCode ErrorCode)
        : this((int)ErrorCode, ErrorCode.Humanize())
    {
    }
};