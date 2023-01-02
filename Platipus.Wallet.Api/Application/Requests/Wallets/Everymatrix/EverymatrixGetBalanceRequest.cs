namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Everymatrix;
using Results.Everymatrix.WithData;
using Services.Wallet;

public record EverymatrixGetBalanceRequest(
    Guid Token,
    string Curency,
    string Hash) : IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>, IEveryMatrixBaseRequest
{


    public class Handler : IRequestHandler<EverymatrixGetBalanceRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context, HttpContext httpContext)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
            EverymatrixGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>().FirstOrDefaultAsync(s => s.Id == request.Token);

            if (session is null)
            {
                EverymatrixResultFactory.Failure<EverymatrixBalanceResponse>(EverymatrixErrorCode.TokenNotFound);
            }

            var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == session.UserId);

            if (user is null)
            {
                EverymatrixResultFactory.Failure<EverymatrixBalanceResponse>(EverymatrixErrorCode.UnknownError);
            }

            var response = new EverymatrixBalanceResponse(
                Status: "200",
                Currency: $"{request.Curency}",
                TotalBalance: user.Balance);

            return EverymatrixResultFactory.Success(response);
        }
    }

}