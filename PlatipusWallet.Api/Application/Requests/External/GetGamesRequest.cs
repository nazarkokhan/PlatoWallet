namespace PlatipusWallet.Api.Application.Requests.External;

using Base.Responses;
using Infrastructure.Persistence;
using MediatR;
using Results.Common.Result;
using Results.Common.Result.WithData;

public record GetGamesRequest(string CasinoId) : IRequest<IResult<GetGamesRequest.Response>>
{
    public class Handler : IRequestHandler<GetGamesRequest, IResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<Response>> Handle(
            GetGamesRequest request,
            CancellationToken cancellationToken)
        {
            // var casinoExist = await _context.Set<Casino>()
            //     .Where(c => c.Id == request.CasinoId)
            //     .AnyAsync(cancellationToken);
            //
            // if (!casinoExist)
            //     return ResultFactory.Failure<Response>(ErrorCode.InvalidCasinoId);
            //
            // var user = await _context.Set<User>()
            //     .Where(
            //         u => u.UserName == request.UserName &&
            //              u.CasinoId == request.CasinoId)
            //     .FirstOrDefaultAsync(cancellationToken);
            //
            // if (user is null)
            //     return ResultFactory.Failure<Response>(ErrorCode.InvalidUser);
            //
            // if (user.IsDisabled)
            //     return ResultFactory.Failure<Response>(ErrorCode.UserDisabled);
            //
            // if (user.Password != request.Password)
            //     return ResultFactory.Failure<Response>(ErrorCode.Unknown);
            
            // var session = new Session
            // {
            //     ExpirationDate = DateTime.UtcNow.AddDays(1),
            //     User = user,
            // };

            // _context.Add(session);
            // await _context.SaveChangesAsync(cancellationToken);

            var result = new Response(default);

            return ResultFactory.Success(result);
        }
    }

    public record Response(List<GetGameDto> Data) : BaseResponse;

    public record GetGameDto(
        string Id,
        string GameId,
        string Name,
        string Category);
}