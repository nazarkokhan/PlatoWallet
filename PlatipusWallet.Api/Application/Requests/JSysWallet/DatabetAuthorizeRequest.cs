namespace PlatipusWallet.Api.Application.Requests.JSysWallet;

using Base.Requests;
using Base.Responses.Databet;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result.Factories;
using Results.Common.Result.WithData;

public record DatabetAuthorizeRequest(
    string PlayerId,
    Guid PlayerToken,
    string Hash) : DatabetBaseRequest(PlayerId, Hash), IRequest<IDatabetResult<DatabetBalanceResponse>>
{
    public class Handler : IRequestHandler<DatabetAuthorizeRequest, IDatabetResult<DatabetBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IDatabetResult<DatabetBalanceResponse>> Handle(
            DatabetAuthorizeRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(u => u.UserName == request.PlayerId)
                .Select(
                    u => new
                    {
                        u.Id,
                        u.UserName,
                        u.Balance,
                        CurrencyName = u.Currency.Name,
                        u.IsDisabled,
                        Sessions = u.Sessions
                            .Where(s => s.ExpirationDate > DateTime.UtcNow)
                            .Select(
                                s => new
                                {
                                    s.Id,
                                    s.ExpirationDate
                                })
                            .ToList()
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null || user.IsDisabled)
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DatabetErrorCode.PlayerNotFound);

            if (user.Sessions.All(s => s.Id != request.PlayerToken))
                return DatabetResultFactory.Failure<DatabetBalanceResponse>(DatabetErrorCode.InvalidToken);

            var response = new DatabetBalanceResponse(user.UserName, user.CurrencyName, user.Balance);

            return DatabetResultFactory.Success(response);
        }
    }

    public override string GetSource()
    {
        return PlayerId + PlayerToken;
    }
}