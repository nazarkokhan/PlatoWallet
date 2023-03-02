#pragma warning disable CS8907
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag.Base;

using Results.Betflag;

public abstract record BetflagExtendedResponse(
    int Result,
    string Message,
    double Balance,
    bool Bonus,
    string Currency,
    string IdTicket,
    string IdSession) : BetflagBaseResponse
{
    public BetflagExtendedResponse(double balance, string currency)
        : this(
            (int)BetflagErrorCode.SUCCSESS,
            nameof(BetflagErrorCode.SUCCSESS),
            balance,
            false,
            currency,
            "",
            "")
    {
    }
}