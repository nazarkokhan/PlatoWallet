namespace Platipus.Wallet.Api.Application.Requests.External.Psw;

using System.ComponentModel;
using Infrastructure.Persistence;
using Services.UisGamesApi;
using Services.UisGamesApi.Dto;

public record UisCreateAwardRequest(
    [property: DefaultValue("test")] string Environment,
    UisCreateAwardGameApiRequest ApiRequest) : IRequest<IResult>
{
    public class Handler : IRequestHandler<UisCreateAwardRequest, IResult>
    {
        private readonly WalletDbContext _context;
        private readonly IUisGameApiClient _uisGameApiClient;

        public Handler(
            WalletDbContext context,
            IUisGameApiClient uisGameApiClient)
        {
            _context = context;
            _uisGameApiClient = uisGameApiClient;
        }

        public async Task<IResult> Handle(
            UisCreateAwardRequest request,
            CancellationToken cancellationToken)
        {
            return ResultFactory.Failure(ErrorCode.Unknown);
            // var user = await _context.Set<User>()
            //     .Where(u => u.Username == request.User)
            //     .Include(
            //         u => u.Awards
            //             .Where(a => a.Id == request.AwardId))
            //     .Include(u => u.Currency)
            //     .FirstOrDefaultAsync(cancellationToken);
            //
            // if (user is null)
            //     return ResultFactory.Failure<PswBaseResponse>(ErrorCode.InvalidUser);
            //
            // if (user.Awards.Any(a => a.Id == request.AwardId))
            //     return ResultFactory.Failure<PswBaseResponse>(ErrorCode.DuplicateAward);
            //
            // var award = new Award(request.AwardId, request.ValidUntil);
            //
            // user.Awards.Add(award);
            // _context.Update(user);
            //
            // await _context.SaveChangesAsync(cancellationToken);
            //
            // var createFreebetAwardResult = await _uisGameApiClient.CreateFreebetAwardAsync(
            //     user.CasinoId,
            //     user.Username,
            //     award.Id,
            //     user.Currency.Id,
            //     new[] { request.Game },
            //     request.ValidUntil,
            //     request.Count,
            //     cancellationToken);
            //
            // return createFreebetAwardResult;
        }
    }
}