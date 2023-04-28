namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base.Response;

using Humanizer;
using Results.BetConstruct;

public record BetconstructErrorResponse : BetconstructBaseResponse
{
    public BetconstructErrorResponse(BetconstructErrorCode error)
        : base(false, error.Humanize(), (int)error)
    {
    }
}