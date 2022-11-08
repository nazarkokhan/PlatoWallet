namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Infrastructure.Persistence;
using Results.Openbox;
using Results.Openbox.WithData;

public record OpenboxBalanceRequest(
        
        OpenboxSingleRequest Request)
    : OpenboxBaseRequest(Request), IRequest<IOpenboxResult<OpenboxBalanceRequest.Response>>
{
    public class Handler : IRequestHandler<OpenboxBalanceRequest, IOpenboxResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IOpenboxResult<Response>> Handle(
            OpenboxBalanceRequest request,
            CancellationToken cancellationToken)
        {
            // var user = await _context.Set<User>()
            //     .TagWith("GetUserBalance")
            //     .Where(
            //         u => u.UserName == request.User
            //              && u.Sessions
            //                  .Select(s => s.Id)
            //                  .Contains(request.SessionId))
            //     .Select(
            //         s => new
            //         {
            //             s.Id,
            //             s.Balance,
            //             s.IsDisabled
            //         })
            //     .FirstOrDefaultAsync(cancellationToken);
            //
            // if (user is null)
            //     return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.InvalidUser);
            //
            // if (user.IsDisabled)
            //     return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.UserDisabled);
            //
            // var response = new PswBalanceResponse(user.Balance);

            return OpenboxResultFactory.Success(new Response());
        }
    }

    public record Response();
}