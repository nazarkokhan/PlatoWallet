#pragma warning disable CS8618
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw;

using System.Globalization;
using Base;
using Base.Response;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Results.ResultToResultMappers;
using Results.Sw;
using Results.Sw.WithData;
using Services.Wallet;

[PublicAPI]
public class SwFreespinRequest : ISwMd5AmountRequest, IRequest<ISwResult<SwBetWinRefundFreespinResponse>>
{
    [FromForm(Name = "providerid")]
    public int ProviderId { get; init; }

    [FromForm(Name = "userid")]
    public int UserId { get; init; }

    [FromForm(Name = "md5")]
    public string Md5 { get; init; }

    [FromForm(Name = "amount")]
    public string Amount { get; init; }

    [FromForm(Name = "roundid")]
    public string RoundId { get; init; }

    [FromForm(Name = "roomid")]
    public string RoomId { get; init; }

    [FromForm(Name = "freespin_id")]
    public string FreespinId { get; init; }

    [FromForm(Name = "gameid")]
    public int GameId { get; init; }

    [FromForm(Name = "gameName")]
    public string GameName { get; init; }

    [FromForm(Name = "token")]
    public string Token { get; init; }

    public class Handler : IRequestHandler<SwFreespinRequest, ISwResult<SwBetWinRefundFreespinResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISwResult<SwBetWinRefundFreespinResponse>> Handle(
            SwFreespinRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.AwardAsync(
                request.Token,
                request.RoundId,
                request.RoundId,
                decimal.Parse(request.Amount, CultureInfo.InvariantCulture),
                request.FreespinId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSwResult<SwBetWinRefundFreespinResponse>();
            var data = walletResult.Data;

            var response = new SwBetWinRefundFreespinResponse((int)data.Balance);

            return SwResultFactory.Success(response);
        }
    }
}