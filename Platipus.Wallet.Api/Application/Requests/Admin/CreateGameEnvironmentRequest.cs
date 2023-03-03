namespace Platipus.Wallet.Api.Application.Requests.Admin;

using System.ComponentModel;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record CreateGameEnvironmentRequest(
        [property: DefaultValue("test")] string Id,
        [property: DefaultValue("https://test.platipusgaming.com/")] Uri BaseUrl,
        [property: DefaultValue("https://platipusgaming.cloud/qa/integration/vivo/test/index.html")] Uri UisBaseUrl)
    : IRequest<IResult>
{
    public class Handler : IRequestHandler<CreateGameEnvironmentRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            CreateGameEnvironmentRequest request,
            CancellationToken cancellationToken)
        {
            var environmentExist = await _context.Set<GameEnvironment>()
                .Where(e => e.Id == request.Id)
                .AnyAsync(cancellationToken);

            if (environmentExist)
                return ResultFactory.Failure(ErrorCode.EnvironmentAlreadyExists);

            var environment = new GameEnvironment(request.Id, request.BaseUrl, request.UisBaseUrl);

            _context.Add(environment);
            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}