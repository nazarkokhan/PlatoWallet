namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Api.Extensions;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record OpenboxGetPlayerInfoRequest(string Token)
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
            var session = await _context.Set<Session>()
                .TagWith("GetSession")
                .Where(s => s.Id == request.Token)
                .Select(
                    s => new
                    {
                        s.Id,
                        s.ExpirationDate,
                        s.IsTemporaryToken,
                        User = new
                        {
                            s.User.Id,
                            s.User.Username,
                            Currency = s.User.CurrencyId
                        }
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null || session.ExpirationDate < DateTime.UtcNow)
                return OpenboxResultFactory.Failure<Response>(OpenboxErrorCode.TokenRelatedErrors);

            var user = session.User;
            var currencyCode = OpenboxHelpers.ToCurrencyCode(user.Currency);

            if (currencyCode is null)
                _logger.LogWarning("Currency code not mapped from {CurrencyName}", user.Currency);

            var response = new Response(user.Id.ToString(), user.Username, currencyCode ?? -1);

            return OpenboxResultFactory.Success(response);
        }
    }

    public record Response(string MemberUid, string MemberAccount, int CurrencyCode);
}