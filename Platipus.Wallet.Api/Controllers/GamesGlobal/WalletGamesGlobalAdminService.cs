namespace Platipus.Wallet.Api.Controllers.GamesGlobal;

using Horizon.XmlRpc.AspNetCore;

public class WalletGamesGlobalAdminService : XmlRpcService, IWalletGamesGlobalAdminService
{
    // private readonly IMediator _mediator;
    //
    // public WalletGamesGlobalAdminService(IMediator mediator)
    // {
    //     _mediator = mediator;
    // }
    //
    // public object GetLoginFromToken(string token)
    // {
    //     var request = new GamesGlobalGetLoginFromTokenRequest(new Guid(token));
    //     var result = _mediator.Send(request).GetAwaiter().GetResult();
    //     var response = GetResponse(result);
    //     return response;
    // }
    //
    // public object GetBalance(GamesGlobalGetBalanceDto[] getBalances)
    // {
    //     var request = new GamesGlobalGetBalanceRequest(getBalances);
    //     var result = _mediator.Send(request).GetAwaiter().GetResult();
    //     var response = GetResponse(result);
    //     return response;
    // }
    //
    // private static T GetResponse<T>(IGamesGlobalResult<T> result)
    // {
    //     if (result.IsFailure)
    //     {
    //         throw result.ErrorCode.ToException();
    //     }
    //
    //     return result.Data;
    // }
}