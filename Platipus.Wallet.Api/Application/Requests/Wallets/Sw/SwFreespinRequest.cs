namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw;

using Base;
using Base.Response;
using Microsoft.AspNetCore.Mvc;
using Results.ResultToResultMappers;
using Results.Sw;
using Results.Sw.WithData;
using Services.Wallet;

public record SwFreespinRequest(
    [property: BindProperty(Name = "providerid")] int ProviderId,
    [property: BindProperty(Name = "userid")] int UserId,
    [property: BindProperty(Name = "md5")] string Md5,
    [property: BindProperty(Name = "gameid")] int GameId,
    [property: BindProperty(Name = "gameName")] string GameName,
    [property: BindProperty(Name = "roundid")] string RoundId,
    [property: BindProperty(Name = "freespin_id")] string FreespinId,
    [property: BindProperty(Name = "token")] string Token) : ISwMd5Request, IRequest<ISwResult<SwBalanceResponse>>
{
    public class Handler : IRequestHandler<SwFreespinRequest, ISwResult<SwBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISwResult<SwBalanceResponse>> Handle(
            SwFreespinRequest request,
            CancellationToken cancellationToken)
        {
            //TODO
            var walletResult = await _wallet.AwardAsync(
                request.Token,
                request.RoundId,
                "", // request.TransactionId,
                0, // request.Amount,
                request.FreespinId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSwResult<SwBalanceResponse>();
            var data = walletResult.Data;

            var response = new SwBalanceResponse(data.UserId, (int)data.Balance, data.Currency);

            return SwResultFactory.Success(response);
        }
    }
}