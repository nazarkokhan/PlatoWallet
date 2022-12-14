namespace Platipus.Wallet.Api.Controllers.GamesGlobal;

using Application.Requests.Wallets.GamesGlobal;
using Application.Results.GamesGlobal;
using Application.Results.GamesGlobal.WithData;
using Horizon.XmlRpc.AspNetCore;
using Horizon.XmlRpc.Core;

public interface IWalletGamesGlobalService
{
    [XmlRpcMethod("FundGame")]
    object FundGame(GamesGlobalGameInfoDto gameInfo, GamesGlobalFundGameDto[] funds);

    [XmlRpcMethod("UpdateWinnings")]
    object UpdateWinnings(GamesGlobalUpdateWinningDto[] request);

    [XmlRpcMethod("CompleteGame")]
    object CompleteGame(GamesGlobalGameInfoDto gameInfo, GamesGlobalServerIdDto[] serverIds);

    [XmlRpcMethod("CancelFundGame")]
    object CancelFundGame(GamesGlobalCancelFundGameDto[] cancels);
}

public class WalletGamesGlobalService : XmlRpcService, IWalletGamesGlobalService
{
    private readonly IMediator _mediator;

    public WalletGamesGlobalService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public object FundGame(GamesGlobalGameInfoDto gameInfo, GamesGlobalFundGameDto[] funds)
    {
        var request = new GamesGlobalFundGameRequest(gameInfo, funds);
        var result = _mediator.Send(request).GetAwaiter().GetResult();
        var response = GetResponse(result);
        return response;
    }

    public object UpdateWinnings(GamesGlobalUpdateWinningDto[] winnings)
    {
        var request = new GamesGlobalUpdateWinningsRequest(winnings);
        var result = _mediator.Send(request).GetAwaiter().GetResult();
        var response = GetResponse(result);
        return response;
    }

    public object CompleteGame(GamesGlobalGameInfoDto gameInfo, GamesGlobalServerIdDto[] serverIds)
    {
        var request = new GamesGlobalCompleteGameRequest(gameInfo, serverIds);
        var result = _mediator.Send(request).GetAwaiter().GetResult();
        var response = GetResponse(result);
        return response;
    }

    public object CancelFundGame(GamesGlobalCancelFundGameDto[] cancels)
    {
        var request = new GamesGlobalCancelFundGamesRequest(cancels);
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