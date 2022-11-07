namespace Platipus.Wallet.Api.Application.Requests.JSysWallet;

using Base.Requests;
using Base.Responses.Databet;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Results.Common;
using Results.Common.Result.Factories;

public record DatabetGetBalanceRequest(
    string PlayerId,
    string Hash) : DatabetBaseRequest(PlayerId, Hash), IRequest<IDatabetResult<DatabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DatabetGetBalanceRequest, IDatabetResult<DatabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDatabetResult<DatabetBalanceResponse>> Handle(
            DatabetGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.UserName == request.PlayerId)
                .Select(
                    s => new
                    {
                        s.Id,
                        s.UserName,
                        s.Balance,
                        CurrencyName = s.Currency.Name,
                        s.IsDisabled
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null || user.IsDisabled)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DatabetErrorCode.PlayerNotFound);

            var response = new DatabetBalanceResponse(user.UserName, user.CurrencyName, user.Balance);

            return DatabetResultFactory.Success(response);
        }
    }

    public override string GetSource()
    {
        return PlayerId;
    }
}