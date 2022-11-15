namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw.Base.Response;

using Requests.Base;
using Results.Psw;

public record PswBaseResponse(PswStatus Status) : BaseResponse
{
    // Ok by default in all responses
    // Status is stored in IResult and converted to error so
    protected PswBaseResponse()
        : this(PswStatus.OK)
    {
    }
};