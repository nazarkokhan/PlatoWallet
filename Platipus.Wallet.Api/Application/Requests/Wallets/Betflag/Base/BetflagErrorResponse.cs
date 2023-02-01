namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag.Base;

using Results.Betflag;

public record BetflagErrorResponse(
    int Result,
    string Message,
    long TimeStamp,
    string Hash)
{
    public BetflagErrorResponse(
        BetflagErrorCode result,
        long timeStamp,
        string hash)
        : this(
            (int)result,
            result.ToString(),
            timeStamp,
            hash)
    {
    }
}