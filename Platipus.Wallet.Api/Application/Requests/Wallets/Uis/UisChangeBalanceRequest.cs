namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis;

using System.Xml.Serialization;
using Base;
using Base.Response;
using Microsoft.AspNetCore.Mvc;
using Results.ResultToResultMappers;
using Results.Uis;
using Results.Uis.WithData;
using Services.Wallet;

[XmlRoot("REQUEST")]
public class UisChangeBalanceRequest
    : IUisUserIdRequest,
        IRequest<IUisResult<UisChangeBalanceRequest.ChangeBalanceBoxResponse>>
{
    [XmlElement("USERID")]
    [BindProperty(Name = "userId")]
    public string UserId { get; set; } = null!;

    [XmlElement("AMOUNT")]
    public decimal Amount { get; set; }

    [XmlElement("TRANSACTIONID")]
    public string TransactionId { get; set; } = null!;

    [XmlIgnore]
    public string TrnType { get; set; } = null!;

    [XmlElement("GAMEID")]
    [BindProperty(Name = "gameId")]
    public int GameId { get; set; }

    [XmlIgnore]
    public string History { get; set; } = null!;

    [XmlElement("ROUNDID")]
    [BindProperty(Name = "roundId")]
    public string RoundId { get; set; } = null!;

    [XmlElement("TRNDESCRIPTION")]
    public string TrnDescription { get; set; } = null!;

    [XmlElement("ISROUNDFINISH")]
    [BindProperty(Name = "isRoundFinished2")]
    public bool IsRoundFinish { get; set; }

    [XmlElement("HASH")]
    [BindProperty(Name = "hash")]
    public string? Hash { get; set; }

    public class Handler
        : IRequestHandler<UisChangeBalanceRequest,
            IUisResult<ChangeBalanceBoxResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IUisResult<ChangeBalanceBoxResponse>> Handle(
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
                        return walletResult
                            .ToUisResult<ChangeBalanceBoxResponse>();
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
                        return walletResult
                            .ToUisResult<ChangeBalanceBoxResponse>();
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
                        return walletResult
                            .ToUisResult<ChangeBalanceBoxResponse>();
                    var data = walletResult.Data;

                    response = new UisChangeBalanceResponse
                    {
                        Balance = data.Balance,
                        ExSystemTransactionId = data.Transaction.InternalId
                    };

                    break;
                }
                default:
                    return UisResultFactory.Failure<ChangeBalanceBoxResponse>(UisErrorCode.InternalError);
            }

            var container = new ChangeBalanceBoxResponse(request, response);

            return UisResultFactory.Success(container);
        }
    }

    public string GetSource()
    {
        return UserId + Amount + TrnType + TrnDescription + RoundId + IsRoundFinish;
    }

    public record UisChangeBalanceResponse : UisBaseResponse
    {
        [XmlElement("EXSYSTEMTRANSACTIONID")]
        public string ExSystemTransactionId { get; set; } = null!;

        [XmlElement("BALANCE")]
        public decimal Balance { get; set; }
    }

    public class ChangeBalanceBoxResponse : UisResponseContainer<UisChangeBalanceRequest, UisChangeBalanceResponse>
    {
        public ChangeBalanceBoxResponse()
            : this(default!, default!)
        {
        }

        public ChangeBalanceBoxResponse(UisChangeBalanceRequest request, UisChangeBalanceResponse response)
            : base(request, response)
        {
        }
    }
}