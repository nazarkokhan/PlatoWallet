namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record OpenboxBalanceRequest(Guid Token) : IOpenboxBaseRequest, IRequest<IOpenboxResult<OpenboxBalanceResponse>>
{
    public class Handler : IRequestHandler<OpenboxBalanceRequest, IOpenboxResult<OpenboxBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IOpenboxResult<OpenboxBalanceResponse>> Handle(
            OpenboxBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .TagWith("GetUserBalance")
                .Where(u => u.Sessions.Select(s => s.Id).Contains(request.Token))
                .Select(
                    s => new
                    {
                        s.Id,
                        s.Balance,
                        s.IsDisabled
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.TokenRelatedErrors);

            if (user.IsDisabled)
                return OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.TokenRelatedErrors);

            var response = new OpenboxBalanceResponse((long)(user.Balance * 100));

            return OpenboxResultFactory.Success(response);
        }
    }
}