namespace Platipus.Wallet.Api.Application.Requests.External.Softswiss;

using Extensions;
using Infrastructure.Persistence;
using Services.SoftswissGamesApi;
using Services.SoftswissGamesApi.DTOs.Requests;

public record ExternalSoftswissRoundDetailsRequest(SoftswissRoundDetailsGameApiRequest ExternalRequest)
    : IRequest<ISoftswissResult>
{
    public class Handler : IRequestHandler<ExternalSoftswissRoundDetailsRequest, ISoftswissResult>
    {
        private readonly WalletDbContext _context;
        private readonly ISoftswissGamesApiClient _gamesApiClient;

        public Handler(WalletDbContext context, ISoftswissGamesApiClient gamesApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
        }

        public async Task<ISoftswissResult> Handle(
            ExternalSoftswissRoundDetailsRequest request,
            CancellationToken cancellationToken)
        {
            var externalRequest = request.Map(r => r.ExternalRequest);

            var casinoGamesResponse = await _gamesApiClient.RoundDetailsAsync(
                externalRequest,
                cancellationToken);

            return casinoGamesResponse;
        }
    }
}