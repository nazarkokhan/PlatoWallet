namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw;

using System.Globalization;
using Base;
using Base.Response;
using Microsoft.AspNetCore.Mvc;
using Results.ResultToResultMappers;
using Results.Sw;
using Results.Sw.WithData;
using Services.Wallet;

public record SwBetWinRequest(
    [property: BindProperty(Name = "providerid")] int ProviderId,
    [property: BindProperty(Name = "userid")] int UserId,
    [property: BindProperty(Name = "md5")] string Md5,
    [property: BindProperty(Name = "amount")] string Amount,
    [property: BindProperty(Name = "remotetranid")] string RemotetranId,
    [property: BindProperty(Name = "gameid")] int GameId,
    [property: BindProperty(Name = "gameName")] string GameName,
    [property: BindProperty(Name = "roundid")] long RoundId,
    [property: BindProperty(Name = "trntype")] string TrnType,
    [property: BindProperty(Name = "finished")] int Finished,
    [property: BindProperty(Name = "token")] string Token) : ISwMd5AmountRequest, IRequest<ISwResult<SwBetWinRefundFreespinResponse>>
{
    public class Handler : IRequestHandler<SwBetWinRequest, ISwResult<SwBetWinRefundFreespinResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISwResult<SwBetWinRefundFreespinResponse>> Handle(
            SwBetWinRequest request,
            CancellationToken cancellationToken)
        {
            SwBetWinRefundFreespinResponse response;
            switch (request.TrnType)
            {
                case "BET":
                {
                    var walletResult = await _wallet.BetAsync(
                        request.Token,
                        request.RoundId.ToString(),
                        request.RemotetranId,
                        -decimal.Parse(request.Amount, CultureInfo.InvariantCulture),
                        roundFinished: request.Finished is 1,
                        cancellationToken: cancellationToken);
                    if (walletResult.IsFailure)
                        return walletResult.ToSwResult<SwBetWinRefundFreespinResponse>();
                    var data = walletResult.Data;

                    response = new SwBetWinRefundFreespinResponse(data.Balance);

                    break;
                }

                case "WIN":
                {
                    var walletResult = await _wallet.WinAsync(
                        request.Token,
                        request.RoundId.ToString(),
                        request.RemotetranId,
                        decimal.Parse(request.Amount, CultureInfo.InvariantCulture),
                        request.Finished is 1,
                        cancellationToken: cancellationToken);
                    if (walletResult.IsFailure)
                        return walletResult.ToSwResult<SwBetWinRefundFreespinResponse>();
                    var data = walletResult.Data;

                    response = new SwBetWinRefundFreespinResponse(data.Balance);

                    break;
                }

                default:
                    return SwResultFactory.Failure<SwBetWinRefundFreespinResponse>(SwErrorCode.InternalSystemError);
            }

            return SwResultFactory.Success(response);
        }
    }
}