namespace Platipus.Wallet.Api.Application.Requests.Admin;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record UpdateGameEnvironmentRequest(
        [property: DefaultValue("test")] string Id,
        [property: DefaultValue("https://test.platipusgaming.com/")] Uri BaseUrl,
        [property: DefaultValue("https://platipusgaming.cloud/qa/integration/vivo/test/index.html")] Uri UisBaseUrl)
    : IRequest<IResult>
{
    public class Handler : IRequestHandler<UpdateGameEnvironmentRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            UpdateGameEnvironmentRequest request,
            CancellationToken cancellationToken)
        {
            var environment = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (environment is null)
                return ResultFactory.Failure(ErrorCode.EnvironmentAlreadyExists);

            environment.BaseUrl = request.BaseUrl;
            environment.UisBaseUrl = request.UisBaseUrl;

            _context.Update(environment);
            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}