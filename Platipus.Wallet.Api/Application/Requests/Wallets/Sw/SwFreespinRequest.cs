namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw;

using Base;
using Base.Response;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Results.Sw;
using Results.Sw.WithData;
using Services.Wallet;

public record SwFreespinRequest(
    [property: BindProperty(Name = "providerid")] int ProviderId,
    [property: BindProperty(Name = "userid")] int UserId,
    [property: BindProperty(Name = "md5")] string Md5,
    [property: BindProperty(Name = "gameid")] int GameId,
    [property: BindProperty(Name = "gameName")] string GameName,
    [property: BindProperty(Name = "roundid")] string RoundId,
    [property: BindProperty(Name = "freespin_id")] string FreespinId,
    [property: BindProperty(Name = "token")] Guid Token) : ISwMd5Request, IRequest<ISwResult<SwBalanceResponse>>
{
    public class Handler : IRequestHandler<SwFreespinRequest, ISwResult<SwBalanceResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<ISwResult<SwBalanceResponse>> Handle(
            SwFreespinRequest request,
            CancellationToken cancellationToken)
        {
            //TODO
            return SwResultFactory.Failure<SwBalanceResponse>(SwErrorCode.InternalSystemError);
        }
    }
}