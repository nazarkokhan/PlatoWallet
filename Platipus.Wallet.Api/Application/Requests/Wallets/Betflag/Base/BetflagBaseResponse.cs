namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag.Base;

using Results.Betflag;

public abstract record BetflagBaseResponse(
    int Result,
    string Message)
{
    public BetflagBaseResponse()
        : this(
            (int)BetflagErrorCode.SUCCSESS,
            nameof(BetflagErrorCode.SUCCSESS))
    {
    }

    public long Timestamp { get; set; }

    public string Hash { get; set; } = null!;
}