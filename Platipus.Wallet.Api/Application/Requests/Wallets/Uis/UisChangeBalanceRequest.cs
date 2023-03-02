#pragma warning disable CS8618
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis;

using System.Xml.Serialization;
using Base;
using Microsoft.AspNetCore.Mvc;
using Results.ResultToResultMappers;
using Results.Uis;
using Results.Uis.WithData;
using Services.Wallet;

[XmlRoot("REQUEST")]
public class UisChangeBalanceRequest : IUisUserIdRequest, IRequest<IUisResult<UisResponseContainer>>
{
    [XmlElement("USERID")]
    [BindProperty(Name = "userId")]
    public string UserId { get; set; }

    [XmlElement("AMOUNT")]
    public decimal Amount { get; set; }

    [XmlElement("TRANSACTIONID")]
    public string TransactionId { get; set; }

    [XmlElement("TRNTYPE")]
    public string TrnType { get; set; }

    [XmlElement("GAMEID")]
    [BindProperty(Name = "gameId")]
    public int GameId { get; set; }

    [XmlElement("HISTORY")]
    public string History { get; set; }

    [XmlElement("ROUNDID")]
    [BindProperty(Name = "roundId")]
    public string RoundId { get; set; }

    [XmlElement("TRNDESCRIPTION")]
    public string TrnDescription { get; set; }

    [XmlElement("ISROUNDFINISH")]
    [BindProperty(Name = "isRoundFinished")]
    public bool IsRoundFinish { get; set; }

    [XmlElement("HASH")]
    [BindProperty(Name = "hash")]
    public string? Hash { get; set; }

    public class Handler : IRequestHandler<UisChangeBalanceRequest, IUisResult<UisResponseContainer>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IUisResult<UisResponseContainer>> Handle(
            UisChangeBalanceRequest request,
            CancellationToken cancellationToken)
        {
            UisChangeBalanceResponse response;
            switch (request.TrnType)
            {
                case "BET":
                {
                    var walletResult = await _wallet.BetAsync(
                        request.UserId,
                        request.RoundId,
                        request.TransactionId,
                        request.Amount,
                        roundFinished: request.IsRoundFinish,
                        searchByUsername: true,
                        cancellationToken: cancellationToken);

                    if (walletResult.IsFailure)
                        return walletResult.ToUisResult<UisResponseContainer>();
                    var data = walletResult.Data;

                    response = new UisChangeBalanceResponse { Balance = data.Balance };

                    break;
                }

                case "WIN":
                {
                    var walletResult = await _wallet.WinAsync(
                        request.UserId,
                        request.RoundId,
                        request.TransactionId,
                        request.Amount,
                        request.IsRoundFinish,
                        searchByUsername: true,
                        cancellationToken: cancellationToken);

                    if (walletResult.IsFailure)
                        return walletResult.ToUisResult<UisResponseContainer>();
                    var data = walletResult.Data;

                    response = new UisChangeBalanceResponse { Balance = data.Balance };

                    break;
                }

                case "CANCELBET":
                {
                    var walletResult = await _wallet.RollbackAsync(
                        request.UserId,
                        request.TransactionId,
                        request.RoundId,
                        true,
                        cancellationToken);

                    if (walletResult.IsFailure)
                        return walletResult.ToUisResult<UisResponseContainer>();
                    var data = walletResult.Data;

                    response = new UisChangeBalanceResponse { Balance = data.Balance };

                    break;
                }
                default:
                    return UisResultFactory.Failure<UisResponseContainer>(UisErrorCode.InternalError);
            }

            var container = new UisResponseContainer
            {
                Request = request,
                Response = response
            };
            return UisResultFactory.Success(container);
        }
    }

    public string GetSource()
    {
        return UserId + Amount + TrnType + TrnDescription + RoundId + IsRoundFinish;
    }

    [XmlRoot("RESPONSE")]
    public record UisChangeBalanceResponse
    {
        [XmlElement("RESULT")]
        public string Result { get; set; } = "OK";

        [XmlElement("ECSYSTEMTRANSACTIONID")]
        public string EcSystemTransactionId { get; set; } = "123123123";

        [XmlElement("BALANCE")]
        public decimal Balance { get; set; }
    }
}