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

public record SwRefundRequest(
    [property: BindProperty(Name = "providerid")] int ProviderId,
    [property: BindProperty(Name = "userid")] int UserId,
    [property: BindProperty(Name = "md5")] string Md5,
    [property: BindProperty(Name = "amount")] decimal Amount,
    [property: BindProperty(Name = "remotetranid")] string RemotetranId,
    [property: BindProperty(Name = "gameid")] int GameId,
    [property: BindProperty(Name = "gameName")] string GameName,
    [property: BindProperty(Name = "roundid")] int RoundId,
    [property: BindProperty(Name = "trntype")] string TrnType,
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
                .Select(u => new {u.UserName})
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return SwResultFactory.Failure<SwBetWinRefundFreespinResponse>(SwErrorCode.UserNotFound);

            var winRequest = request.Map(
                r => new RollbackRequest(
                    r.Token,
                    user.UserName,
                    r.GameName,
                    r.RoundId.ToString(),
                    r.RemotetranId));

            var winResult = await _wallet.RollbackAsync(winRequest, cancellationToken);
            if (winResult.IsFailure)
                winResult.ToSwResult();

            var response = winResult.Data.Map(d => new SwBetWinRefundFreespinResponse(d.Balance));

            return SwResultFactory.Success(response);
        }
    }
}