namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Api.Extensions;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record OpenboxGetPlayerInfoRequest(Guid Token)
    : IOpenboxBaseRequest, IRequest<IOpenboxResult<OpenboxGetPlayerInfoRequest.Response>>
{
    public class Handler : IRequestHandler<OpenboxGetPlayerInfoRequest, IOpenboxResult<Response>>
    {
        private readonly WalletDbContext _context;
        private readonly ILogger<Handler> _logger;

        public Handler(WalletDbContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IOpenboxResult<Response>> Handle(
            OpenboxGetPlayerInfoRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .TagWith("GetUser")
                .Where(u => u.Sessions.Any(s => s.Id == request.Token))
                .Select(
                    s => new
                    {
                        s.Id,
                        s.UserName,
                        Currency = s.Currency.Name
                    })
                .FirstOrDefaultAsync(cancellationToken);

            //TODO validate should be 2 token
            if (user is null)
                return OpenboxResultFactory.Failure<Response>(OpenboxErrorCode.TokenRelatedErrors);

            var currencyCode = OpenboxHelpers.ToCurrencyCode(user.Currency);

            if (currencyCode is null)
                _logger.LogWarning("Currency code not mapped from {CurrencyName}", user.Currency);

            var response = new Response(user.Id, user.UserName, currencyCode ?? -1);

            return OpenboxResultFactory.Success(response);
        }
    }

    public record Response(Guid MemberUid, string MemberAccount, int CurrencyCode);
}