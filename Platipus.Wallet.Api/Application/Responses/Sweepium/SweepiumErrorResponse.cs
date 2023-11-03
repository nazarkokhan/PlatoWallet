using Humanizer;
using Platipus.Wallet.Api.Application.Results.Sweepium;

namespace Platipus.Wallet.Api.Application.Responses.Sweepium.Base;

public record SweepiumErrorResponse : SweepiumCommonResponse
{
    public SweepiumErrorResponse(SweepiumErrorCode error)
        : base(false, error.Humanize(), (int)error)
    {
    }
}