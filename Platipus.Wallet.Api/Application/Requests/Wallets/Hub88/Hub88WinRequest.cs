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

public record Hub88WinRequest(
    string SupplierUser,
    string TransactionUuid,
    string Token,
    bool RoundClosed,
    string Round,
    string? RewardUuid,
    string RequestUuid,
    string ReferenceTransactionUuid,
    bool IsFree,
    int GameId,
    string GameCode,
    string Currency,
    string? Bet,
    int Amount,
    Hub88MetaDto? Meta) : IHub88BaseRequest, IRequest<IHub88Result<Hub88BalanceResponse>>
{
    public class Handler : IRequestHandler<Hub88WinRequest, IHub88Result<Hub88BalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IHub88Result<Hub88BalanceResponse>> Handle(
            Hub88WinRequest request,
            CancellationToken cancellationToken)
        {
            Hub88BalanceResponse response;

            if (request.IsFree)
            {
                if (request.RewardUuid is null)
                    return Hub88ResultFactory.Failure<Hub88BalanceResponse>(Hub88ErrorCode.RS_ERROR_WRONG_SYNTAX);

                var walletResult = await _wallet.AwardAsync(
                    request.Token,
                    request.Round,
                    request.TransactionUuid,
                    request.Amount / 100000m,
                    request.RewardUuid,
                    request.Currency,
                    request.RoundClosed,
                    cancellationToken: cancellationToken);

                if (walletResult.IsFailure)
                    return walletResult.ToHub88Result<Hub88BalanceResponse>();
                var data = walletResult.Data;

                response = new Hub88BalanceResponse(
                    (int)(data.Balance * 100000),
                    request.SupplierUser,
                    request.RequestUuid,
                    data.Currency);
            }
            else
            {
                var walletResult = await _wallet.WinAsync(
                    request.Token,
                    request.Round,
                    request.TransactionUuid,
                    request.Amount / 100000m,
                    request.RoundClosed,
                    request.Currency,
                    cancellationToken: cancellationToken);

                if (walletResult.IsFailure)
                    return walletResult.ToHub88Result<Hub88BalanceResponse>();
                var data = walletResult.Data;

                response = new Hub88BalanceResponse(
                    (int)(data.Balance * 100000),
                    request.SupplierUser,
                    request.RequestUuid,
                    data.Currency);
            }

            return Hub88ResultFactory.Success(response);
        }
    }
}