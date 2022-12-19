namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw;

using Base;
using Base.Response;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Results.ResultToResultMappers;
using Results.Sw;
using Results.Sw.WithData;
using Services.Wallet;
using Services.Wallet.DTOs;

public record SwBetWinRequest(
    [property: BindProperty(Name = "providerid")] int ProviderId,
    [property: BindProperty(Name = "userid")] int UserId,
    [property: BindProperty(Name = "md5")] string Md5,
    [property: BindProperty(Name = "amount")] decimal Amount,
    [property: BindProperty(Name = "remotetranid")] string RemotetranId,
    [property: BindProperty(Name = "gameid")] int GameId,
    [property: BindProperty(Name = "gameName")] string GameName,
    [property: BindProperty(Name = "roundid")] int RoundId,
    [property: BindProperty(Name = "trntype")] string TrnType,
    [property: BindProperty(Name = "finished")] bool Finished,
    [property: BindProperty(Name = "token")] Guid Token) : ISwMd5Request, IRequest<ISwResult<SwBetWinRefundFreespinResponse>>
{
    public class Handler : IRequestHandler<SwBetWinRequest, ISwResult<SwBetWinRefundFreespinResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<ISwResult<SwBetWinRefundFreespinResponse>> Handle(
            SwBetWinRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.SwUserId == request.UserId)
                .Select(
                    u => new
                    {
                        u.UserName,
                        Currency = u.Currency.Name,
                        u.Casino.SwProviderId
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return SwResultFactory.Failure<SwBetWinRefundFreespinResponse>(SwErrorCode.UserNotFound);

            SwBetWinRefundFreespinResponse response;
            switch (request.TrnType)
            {
                case "BET":
                    var betRequest = request.Map(
                        r => new BetRequest(
                            r.Token,
                            user.UserName,
                            user.Currency,
                            r.RoundId.ToString(),
                            r.RemotetranId,
                            r.Finished,
                            r.Amount));

                    var betResult = await _wallet.BetAsync(betRequest, cancellationToken);
                    if (betResult.IsFailure)
                        return betResult.ToSwResult<SwBetWinRefundFreespinResponse>();

                    response = betResult.Data.Map(d => new SwBetWinRefundFreespinResponse(d.Balance));
                    break;

                case "WIN":
                    var winRequest = request.Map(
                        r => new WinRequest(
                            r.Token,
                            user.UserName,
                            user.Currency,
                            r.GameName,
                            r.RoundId.ToString(),
                            r.RemotetranId,
                            r.Finished,
                            -r.Amount));

                    var winResult = await _wallet.WinAsync(winRequest, cancellationToken);
                    if (winResult.IsFailure)
                        return winResult.ToSwResult<SwBetWinRefundFreespinResponse>();

                    response = winResult.Data.Map(d => new SwBetWinRefundFreespinResponse(d.Balance));
                    break;

                default:
                    return SwResultFactory.Failure<SwBetWinRefundFreespinResponse>(SwErrorCode.InternalSystemError);
            }

            return SwResultFactory.Success(response);
        }
    }
}