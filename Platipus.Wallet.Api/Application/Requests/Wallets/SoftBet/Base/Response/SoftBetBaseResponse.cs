namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet.Base.Response;

using Results.ISoftBet;

public record SoftBetBaseResponse(SoftBetStatus Status)
{
    public SoftBetBaseResponse()
        : this(SoftBetStatus.success)
    {
    }
}