namespace Platipus.Wallet.Api.Controllers.GamesGlobal;

using Application.Requests.Wallets.GamesGlobal;
using Application.Results.GamesGlobal;
using Application.Results.GamesGlobal.WithData;
using Horizon.XmlRpc.AspNetCore;
using Horizon.XmlRpc.Core;

public interface IWalletGamesGlobalAdminService
{
    [XmlRpcMethod("GetLoginFromToken")]
    object GetLoginFromToken(string token);

    [XmlRpcMethod("GetBalance")]
    object GetBalance(GamesGlobalGetBalanceDto[] getBalances);
}

public class WalletGamesGlobalAdminService : XmlRpcService, IWalletGamesGlobalAdminService
{
    private readonly IMediator _mediator;

    public WalletGamesGlobalAdminService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public object GetLoginFromToken(string token)
    {
        var request = new GamesGlobalGetLoginFromTokenRequest(new Guid(token));
        var result = _mediator.Send(request).GetAwaiter().GetResult();
        var response = GetResponse(result);
        return response;
    }

    public object GetBalance(GamesGlobalGetBalanceDto[] getBalances)
    {
        var request = new GamesGlobalGetBalanceRequest(getBalances);
        var result = _mediator.Send(request).GetAwaiter().GetResult();
        var response = GetResponse(result);
        return response;
    }

    private static T GetResponse<T>(IGamesGlobalResult<T> result)
    {
        if (result.IsFailure)
        {
            throw result.ErrorCode.ToException();
        }

        return result.Data;
    }
}