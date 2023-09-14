namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Hub88;
using Results.Hub88.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record Hub88BetRequest(
    string SupplierUser,
    string TransactionUuid,
    string Token,
    bool RoundClosed,
    string Round,
    string? RewardUuid,
    string RequestUuid,
    bool IsFree,
    int GameId,
    string GameCode,
    string Currency,
    string? Bet,
    int Amount,
    Hub88MetaDto? Meta) : IHub88BaseRequest, IRequest<IHub88Result<Hub88BalanceResponse>>
{
    public class Handler : IRequestHandler<Hub88BetRequest, IHub88Result<Hub88BalanceResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IHub88Result<Hub88BalanceResponse>> Handle(
            Hub88BetRequest request,
            CancellationToken cancellationToken)
        {
            if (request.IsFree)
            {
                var award = await _context.Set<Award>()
                   .Where(a => a.User.Username == request.SupplierUser)
                   .FirstOrDefaultAsync(cancellationToken);

                if (award is null)
                    return Hub88ResultFactory.Failure<Hub88BalanceResponse>(Hub88ErrorCode.RS_ERROR_WRONG_SYNTAX);
            }

            var walletResult = await _wallet.BetAsync(
                request.Token,
                request.Round,
                request.TransactionUuid,
                request.Amount / 100000m,
                request.Currency,
                request.RoundClosed,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToHub88Result<Hub88BalanceResponse>();
            var data = walletResult.Data;

            var response = new Hub88BalanceResponse(
                (int)(data.Balance * 100000),
                request.SupplierUser,
                request.RequestUuid,
                data.Currency);

            return Hub88ResultFactory.Success(response);
        }
    }
}