namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Everymatrix;
using Results.Everymatrix.WithData;

public record EverymatrixGetBalanceRequest(
    string Token,
    string Currency,
    string Hash) : IRequest<IEverymatrixResult<EveryMatrixBaseResponse>>, IEveryMatrixBaseRequest
{
    public class Handler : IRequestHandler<EverymatrixGetBalanceRequest, IEverymatrixResult<EveryMatrixBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEverymatrixResult<EveryMatrixBaseResponse>> Handle(
            EverymatrixGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>().FirstOrDefaultAsync(s => s.Id == new Guid(request.Token));

            if (session is null)
            {
                EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.TokenNotFound);
            }

            var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == session.UserId);

            if (user is null)
            {
                EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.VendorAccountNotActive);
            }

            var userIsBlocked = user.IsDisabled;

            if (userIsBlocked)
            {
                return EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.UserIsBlocked);
            }

            var currency = await _context.Set<Currency>().FirstOrDefaultAsync(c => c.Id == user.CurrencyId);

            var currencyIsValid = currency.Name == request.Currency;

            if (!currencyIsValid)
            {
                EverymatrixResultFactory.Failure<EveryMatrixBaseResponse>(EverymatrixErrorCode.CurrencyDoesntMatch);
            }

            var response = new EveryMatrixBaseResponse(
                Status: "Ok",
                Currency: $"{request.Currency}",
                TotalBalance: user.Balance);

            return EverymatrixResultFactory.Success(response);
        }
    }
}