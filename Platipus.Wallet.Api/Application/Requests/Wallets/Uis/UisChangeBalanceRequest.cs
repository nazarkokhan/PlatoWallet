#pragma warning disable CS8618
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis;

using System.Xml.Serialization;
using Base;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Results.ResultToResultMappers;
using Results.Uis;
using Results.Uis.WithData;
using Services.Wallet;
using Services.Wallet.DTOs;

[XmlRoot("REQUEST")]
public class UisChangeBalanceRequest : IUisHashRequest, IRequest<IUisResult<UisResponseContainer>>
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
        private readonly WalletDbContext _context;
        private readonly IWalletService _wallet;

        public Handler(WalletDbContext context, IWalletService wallet)
        {
            _context = context;
            _wallet = wallet;
        }

        public async Task<IUisResult<UisResponseContainer>> Handle(
            UisChangeBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.UserName == request.UserId)
                .Select(
                    u => new
                    {
                        u.UserName,
                        Currency = u.Currency.Name,
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return UisResultFactory.Failure<UisResponseContainer>(UisErrorCode.UserNotFound);

            UisChangeBalanceResponse response;
            switch (request.TrnType)
            {
                case "BET":
                    var betRequest = request.Map(
                        r => new BetRequest(
                            Guid.Empty,
                            user.UserName,
                            user.Currency,
                            r.RoundId,
                            r.TransactionId,
                            r.IsRoundFinish,
                            r.Amount));

                    var betResult = await _wallet.BetAsync(betRequest, cancellationToken);
                    if (betResult.IsFailure)
                        return betResult.ToUisResult<UisResponseContainer>();

                    response = betResult.Data.Map(d => new UisChangeBalanceResponse { Balance = d.Balance });
                    break;

                case "WIN":
                    var winRequest = request.Map(
                        r => new WinRequest(
                            Guid.Empty,
                            user.UserName,
                            user.Currency,
                            r.GameId.ToString(),
                            r.RoundId,
                            r.TransactionId,
                            r.IsRoundFinish,
                            r.Amount));

                    var winResult = await _wallet.WinAsync(winRequest, cancellationToken);
                    if (winResult.IsFailure)
                        return winResult.ToUisResult<UisResponseContainer>();

                    response = winResult.Data.Map(d => new UisChangeBalanceResponse { Balance = d.Balance });
                    break;

                case "CANCELBET":
                    var cancelRequest = request.Map(
                        r => new WinRequest(
                            Guid.Empty,
                            user.UserName,
                            user.Currency,
                            r.GameId.ToString(),
                            r.RoundId.ToString(),
                            r.TransactionId,
                            r.IsRoundFinish,
                            r.Amount));

                    var cancelResult = await _wallet.WinAsync(cancelRequest, cancellationToken);
                    if (cancelResult.IsFailure)
                        return cancelResult.ToUisResult<UisResponseContainer>();

                    // response = cancelResult.Data.Map(d => new UisChangeBalanceResponse { Balance = 100 });
                    response = new UisChangeBalanceResponse { Balance = 0 };
                    break;
                default:
                    return UisResultFactory.Failure<UisResponseContainer>(UisErrorCode.InternalSystemError);
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
        public string Result { get; set; }

        [XmlElement("ECSYSTEMTRANSACTIONID")]
        public string EcSystemTransactionId { get; set; }

        [XmlElement("BALANCE")]
        public decimal Balance { get; set; }
    }
}