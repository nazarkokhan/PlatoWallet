namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw;

using Base;
using Base.Response;
using Microsoft.AspNetCore.Mvc;
using Results.ResultToResultMappers;
using Results.Sw;
using Results.Sw.WithData;
using Services.Wallet;

public record SwRefundRequest(
    [property: BindProperty(Name = "providerid")] int ProviderId,
    [property: BindProperty(Name = "userid")] int UserId,
    [property: BindProperty(Name = "md5")] string Md5,
    [property: BindProperty(Name = "amount")] string Amount,
    [property: BindProperty(Name = "remotetranid")] string RemotetranId,
    [property: BindProperty(Name = "gameid")] int GameId,
    [property: BindProperty(Name = "gameName")] string GameName,
    [property: BindProperty(Name = "roundid")] long RoundId,
    [property: BindProperty(Name = "trntype")] string TrnType,
    [property: BindProperty(Name = "token")] string Token) : ISwMd5AmountRequest, IRequest<ISwResult<SwBetWinRefundFreespinResponse>>
{
    public class Handler : IRequestHandler<SwRefundRequest, ISwResult<SwBetWinRefundFreespinResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISwResult<SwBetWinRefundFreespinResponse>> Handle(
            SwRefundRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.RollbackAsync(
                request.Token,
                request.RemotetranId,
                request.RoundId.ToString(),
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSwResult<SwBetWinRefundFreespinResponse>();
            var data = walletResult.Data;

            var response = new SwBetWinRefundFreespinResponse(data.Balance);

            return SwResultFactory.Success(response);
        }
    }
}