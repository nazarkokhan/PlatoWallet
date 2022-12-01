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

public record SwGetBalanceMd5Request(
    [property: BindProperty(Name = "providerid")] int ProviderId,
    [property: BindProperty(Name = "userid")] int UserId,
    [property: BindProperty(Name = "md5")] string Md5,
    [property: BindProperty(Name = "token")] Guid Token) : ISwMd5Request, IRequest<ISwResult<SwBalanceResponse>>
{
    public class Handler : IRequestHandler<SwGetBalanceMd5Request, ISwResult<SwBalanceResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<ISwResult<SwBalanceResponse>> Handle(
            SwGetBalanceMd5Request request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.SwUserId == request.UserId)
                .Select(u => new {u.UserName})
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return SwResultFactory.Failure<SwBalanceResponse>(SwErrorCode.UserNotFound);

            var walletRequest = request.Map(r => new GetBalanceRequest(r.Token, user.UserName));

            var walletResult = await _wallet.GetBalanceAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                walletResult.ToSwResult();
            var walletData = walletResult.Data;

            var response = walletData.Map(
                d => new SwBalanceResponse(
                    request.UserId,
                    d.Balance,
                    d.Currency));

            return SwResultFactory.Success(response);
        }
    }
}